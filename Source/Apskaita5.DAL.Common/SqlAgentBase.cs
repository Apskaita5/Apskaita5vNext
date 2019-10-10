using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Apskaita5.DAL.Common.DbSchema;
using Apskaita5.DAL.Common.MicroOrm;

namespace Apskaita5.DAL.Common
{

    /// <summary>
    /// Represents a base class for a concrete SQL implementation, e.g. MySql, SQLite, etc.
    /// </summary>
    /// <remarks>On ASP .NET core should be used:
    /// - as a singleton if only one database is used;
    /// - inside a scoped wrapper that would have IHttpContextAccessor dependency in constructor and 
    /// initialize the encapsulated SQL agent using request info.
    /// On standalone application should use (static?) dictionary per database </remarks>
    public abstract class SqlAgentBase : ISqlAgent, IDisposable
    {

        #region Fields
                
        private readonly ISqlDictionary _sqlDictionary;
        protected readonly ILogger _Logger = null; 

        #endregion

        #region Properties         

        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, e.g. MySql, SQLite, etc.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets an id of the concrete SQL implementation, e.g. MySQL, SQLite.
        /// The id is used to select an appropriate SQL token dictionary.
        /// </summary>
        public abstract string SqlImplementationId { get; }

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based (e.g. SQLite),
        /// otherwise - server based (e.g. MySQL).
        /// </summary>
        public abstract bool IsFileBased { get; }

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent,
        /// e.g. root, sa, etc.
        /// </summary>
        public abstract string RootName { get; }

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation behind the SqlAgent, e.g. %, *, etc.
        /// </summary>
        public abstract string Wildcart { get; }

        /// <summary>
        /// Gets a connection string that does not include database parameter. 
        /// </summary>
        /// <remarks>Should be initialized when creating an SqlAgent instance.</remarks>
        public string BaseConnectionString { get; }

        /// <summary>
        /// Gets the current database name (string.Empty for no database).
        /// </summary>
        public string CurrentDatabase { get; }

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public abstract bool IsTransactionInProgress { get; }



        /// <summary>
        /// Gets or sets a query timeout in ms. (default is 10.000 ms)
        /// </summary>
        public int QueryTimeOut { get; set; } = 10000;

        /// <summary>
        /// Gets or sets whether a transaction is stored within SqliteAgent instance.
        /// If not, a transaction is stored within AsyncLocal storage.
        /// </summary>
        public bool UseTransactionPerInstance { get; set; } = false;

        /// <summary>
        /// Gets or sets whether a boolean type is stored as TinyInt, i.e. param values needs
        /// to be replaced: true = 1; false = 0.
        /// </summary>
        public bool BooleanStoredAsTinyInt { get; set; } = true;

        /// <summary>
        /// Gets or sets whether a Guid type is stored as Blob. Otherwise, Guid is stored as CHAR(32).
        /// </summary>
        public bool GuidStoredAsBlob { get; set; } = false;

        /// <summary>
        /// Gets or sets whether all schema names (tables, fields, indexes etc) are lower case only.
        /// </summary>
        public bool AllSchemaNamesLowerCased { get; set; } = true;

        #endregion


        /// <summary>
        /// Initializes a new SqlAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="allowEmptyConnString">whether the SqlAgent implementation can handle
        /// empty connection string (e.g. when the database parameter is the only parameter)</param>
        /// <param name="databaseName">a name of the database to use (if any)</param>
        /// <param name="sqlDictionary">an implementation of SQL dictionary to use (if any)</param>
        protected SqlAgentBase(string baseConnectionString, bool allowEmptyConnString, 
            string databaseName, ISqlDictionary sqlDictionary, ILogger logger)
        {
            
            if (baseConnectionString.IsNullOrWhiteSpace() && !allowEmptyConnString)
                throw new ArgumentNullException(nameof(baseConnectionString));

            _sqlDictionary = sqlDictionary;
            BaseConnectionString = baseConnectionString?.Trim() ?? string.Empty;
            CurrentDatabase = databaseName ?? string.Empty;
            _Logger = logger;

        }

        /// <summary>
        /// Creates an SqlAgent clone.
        /// </summary>
        /// <param name="agentToClone">an SqlAgent to clone</param>
        protected SqlAgentBase(SqlAgentBase agentToClone)
        {

            if (agentToClone.IsNull()) throw new ArgumentNullException(nameof(agentToClone));

            BaseConnectionString = agentToClone.BaseConnectionString;
            CurrentDatabase = agentToClone.CurrentDatabase;            
            BooleanStoredAsTinyInt = agentToClone.BooleanStoredAsTinyInt;
            AllSchemaNamesLowerCased = agentToClone.AllSchemaNamesLowerCased;
            GuidStoredAsBlob = agentToClone.GuidStoredAsBlob;
            QueryTimeOut = agentToClone.QueryTimeOut;
            UseTransactionPerInstance = agentToClone.UseTransactionPerInstance;
            _sqlDictionary = agentToClone._sqlDictionary;
            _Logger = agentToClone._Logger;

        }


        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public abstract Task TestConnectionAsync();

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> exists.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database exists.</returns>
        public abstract Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> is empty, i.e. contains no tables.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database contains any tables.</returns>
        public abstract Task<bool> DatabaseEmptyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a default database schema manager (to create or drop database schema, extract schema,
        /// check for schema errors against gauge schema)
        /// </summary>
        public abstract ISchemaManager GetDefaultSchemaManager();

        /// <summary>
        /// Gets a default micro ORM service.
        /// </summary>
        public abstract IOrmService GetDefaultOrmService();

        #region Transactions        

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> method, 
            CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));

            }
            else
            {

                if (null == method) throw new ArgumentNullException(nameof(method));

                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;

            }

            try
            {
                await method(cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);                
                throw;
            }            

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param> 
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task ExecuteInTransactionAsync(Func<SqlAgentBase, CancellationToken, Task> method,
            CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {    
                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));
            }
            else
            {         
                if (null == method) throw new ArgumentNullException(nameof(method));
                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;
            }

            try
            {
                await method(this, cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);                 
                throw;
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="parameter">a custom parameter to pass to the method</param> 
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task ExecuteInTransactionAsync<T>(Func<SqlAgentBase, T, CancellationToken, Task> method, 
            T parameter, CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {
                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));
            }
            else
            {
                if (null == method) throw new ArgumentNullException(nameof(method));
                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;
            }

            try
            {
                await method(this, parameter, cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);                   
                throw;
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param> 
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> method,
            CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (null == method) throw new ArgumentNullException(nameof(method));

                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;

            }

            try
            {
                var result = await method(cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);
                throw;
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>  
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<SqlAgentBase, CancellationToken, 
            Task<TResult>> method, CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (null == method) throw new ArgumentNullException(nameof(method));

                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;

            }

            try
            {
                var result = await method(this, cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);
                throw;
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="parameter">a custom parameter to pass to the method</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        public async Task<TResult> ExecuteInTransactionAsync<T, TResult>(Func<SqlAgentBase, T, CancellationToken, 
            Task<TResult>> method, T parameter, CancellationToken cancellationToken)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (null == method) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (null == method) throw new ArgumentNullException(nameof(method));

                RegisterTransactionForAsyncContext(await TransactionBeginAsync(cancellationToken).ConfigureAwait(false));
                isOwner = true;

            }

            try
            {
                var result = await method(this, parameter, cancellationToken).ConfigureAwait(false);
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                Log(ex);
                if (isOwner && IsTransactionInProgress) TransactionRollback(ex);                
                throw;
            }

        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        protected abstract Task<object> TransactionBeginAsync(CancellationToken cancellationToken);

        /// <summary>
        /// As the TransactionBeginAsync method is Async, AsyncLocal value is lost outside of it's context
        /// and needs to be set in the context of the invoking method.
        /// </summary>
        /// <param name="transaction">a transaction that has been initiated by the TransactionBeginAsync method</param>
        protected abstract void RegisterTransactionForAsyncContext(object transaction);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if no transaction in progress</exception>
        protected abstract void TransactionCommit();

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        /// <param name="ex">an exception that caused the rollback</param>
        protected abstract void TransactionRollback(Exception ex);

        #endregion

        #region CRUD Methods
          
        /// <summary>
        /// Fetches data using SQL query token in the SQL repository.
        /// </summary>
        /// <param name="token">a token of the SQL query in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public abstract Task<LightDataTable> FetchTableAsync(string token, SqlParam[] parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Fetches multiple data tables using SQL query tokens in the SQL repository.
        /// </summary>
        /// <param name="queries">a list of queries identified by tokens in the SQL repository
        /// and collections of SQL query parameters (null or empty array for none).</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>an array of <see cref="LightDataTable">LightDataTables</see> that contain
        /// data returned by the SQL queries.</returns>
        public abstract Task<LightDataTable[]> FetchTablesAsync((string Token, SqlParam[] Parameters)[] queries, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Fetches data using raw SQL query. i.e. without SQL repository tokens.
        /// </summary>
        /// <param name="sqlQuery">an SQL query to execute</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public abstract Task<LightDataTable> FetchTableRawAsync(string sqlQuery, SqlParam[] parameters, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        public abstract Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Executes an SQL statement, that inserts a new row, using SQL query token 
        /// in the SQL repository and returns last insert id.
        /// </summary>
        /// <param name="insertStatementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public abstract Task<Int64> ExecuteInsertAsync(string insertStatementToken, SqlParam[] parameters);
        
        /// <summary>
        /// Executes a raw SQL statement, that inserts a new row, and returns last insert id.
        /// </summary>
        /// <param name="insertStatement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public abstract Task<Int64> ExecuteInsertRawAsync(string insertStatement, SqlParam[] parameters);

        /// <summary>
        /// Executes an SQL statement using SQL query token in the SQL repository 
        /// and returns affected rows count.
        /// </summary>
        /// <param name="statementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public abstract Task<int> ExecuteCommandAsync(string statementToken, SqlParam[] parameters);

        /// <summary>
        /// Executes a raw SQL statement and returns affected rows count.
        /// </summary>
        /// <param name="statement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public abstract Task<int> ExecuteCommandRawAsync(string statement, SqlParam[] parameters);

        /// <summary>
        /// Executes multiple SQL statements using one database connection but without a transaction.
        /// </summary>
        /// <param name="statements">a collection of the SQL statements to execute in batch</param>
        /// <remarks>Used when modifying databases and in other cases when transactions are not supported
        /// in order to reuse connection.</remarks>
        public abstract Task ExecuteCommandBatchAsync(string[] statements);

        #endregion

        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        public abstract SqlAgentBase GetCopy();

        ISqlAgent ISqlAgent.GetCopy()
        {
            return GetCopy();
        }

        /// <summary>
        /// Gets an SQL query identified by the token for the specific SQL implementation sqlAgent.
        /// Uses ICacheProvider specified by sqlAgent to cache the SQL dictionary.
        /// </summary>
        /// <param name="token">a token (code) that identifies an SQL query in SQL repository</param>
        /// <exception cref="ArgumentNullException">Parameter token is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter sqlAgent is not specified.</exception>
        /// <exception cref="InvalidOperationException">SQL repository path is not initialized.</exception>
        /// <exception cref="InvalidOperationException">Global cache provider ir not initialized.</exception>
        /// <exception cref="ArgumentException">SQL agent does not implement repository file prefix.</exception>
        /// <exception cref="FileNotFoundException">No repository files found or they contain no data 
        /// for the SQL agent type specified.</exception>
        /// <exception cref="Exception">Failed to load file due to missing query tokens.</exception>
        /// <exception cref="Exception">Failed to load file due to duplicate query token.</exception>
        /// <exception cref="InvalidOperationException">SQL dictionary failed to initialize for unknown reason.</exception>
        /// <exception cref="InvalidOperationException">SQL query token is unknown.</exception>
        protected string GetSqlQuery(string token)
        {
            if (_sqlDictionary.IsNull()) throw new InvalidOperationException(Properties.Resources.SqlDictionaryNotConfiguredException);
            return _sqlDictionary.GetSqlQuery(token, this);
        }
  
        /// <summary>
        /// Logs an exception (if a logger is used) and then thows it.
        /// </summary>
        /// <param name="ex">an exception to log and throw</param>
        protected void LogAndThrow(Exception ex)
        {
            if (null == ex) LogAndThrow(new ArgumentNullException(nameof(ex)));
            if (ex.GetType() == typeof(AggregateException))
                ex = ((AggregateException)ex).Flatten().InnerExceptions[0];
            _Logger?.LogError(ex, ex.Message, null);
            throw ex;
        }

        /// <summary>
        /// Logs an exception (if a logger is used).
        /// </summary>
        /// <param name="ex">an exception to log</param>
        protected void Log(Exception ex)
        {
            if (null == ex) LogAndThrow(new ArgumentNullException(nameof(ex)));
            if (ex.GetType() == typeof(AggregateException))
                ex = ((AggregateException)ex).Flatten().InnerExceptions[0];
            _Logger?.LogError(ex, ex.Message, null);
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    DisposeManagedState();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        protected abstract void DisposeManagedState();            

        #endregion

    }
}
