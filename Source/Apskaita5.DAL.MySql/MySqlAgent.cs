using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Apskaita5.DAL.Common;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Apskaita5.DAL.Common.DbSchema;
using Apskaita5.DAL.Common.MicroOrm;
using static Apskaita5.DAL.MySql.Constants;

namespace Apskaita5.DAL.MySql
{
    /// <summary>
    /// Represents an abstraction over the native MySql data access and schema methods.
    /// </summary>
    /// <remarks>Should be stored in ApplicationContext.Local context (in thread for client, 
    /// in http context on server).</remarks>
    public class MySqlAgent : SqlAgentBase
    {

        #region Constants

        private const string AgentName = "MySql connector";
        private const bool AgentIsFileBased = false;
        private const string AgentRootName = "root";
        private const string AgentWildcart = "%";

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public override bool IsTransactionInProgress => (CurrentTransaction != null);

        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, i. e. MySql connector.
        /// </summary>
        public override string Name => AgentName;

        /// <summary>
        /// Gets an id of the concrete SQL implementation, i.e. mysql.
        /// The id is used to select an appropriate SQL token dictionary.
        /// </summary>
        public override string SqlImplementationId => MySqlImplementationId;

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based, i.e. false.
        /// </summary>
        public override bool IsFileBased => AgentIsFileBased;

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent, i.e. root.
        /// </summary>
        public override string RootName => AgentRootName;

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation behind the SqlAgent, i.e. %.
        /// </summary>
        public override string Wildcart => AgentWildcart;

        #endregion


        /// <summary>
        /// Initializes a new MySqlAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="databaseName">a name of the database to use (if any)</param>
        /// <param name="sqlDictionary">an implementation of SQL dictionary to use (if any)</param>
        /// <param name="logger">a logger to log errors and warnings (if any)</param>
        public MySqlAgent(string baseConnectionString, string databaseName, ISqlDictionary sqlDictionary, ILogger logger)
            : base(baseConnectionString, false, databaseName, sqlDictionary, logger)
        {
            if (logger != null)
            {
                MySqlTrace.Switch.Level = System.Diagnostics.SourceLevels.Warning;
                MySqlTrace.Listeners.Add(new MySqlTraceListener(logger, this));
            }            
        }

        /// <summary>
        /// Clones a MySqlAgent instance.
        /// </summary>
        /// <param name="agentToClone">a MySqlAgent instance to clone</param>
        private MySqlAgent(MySqlAgent agentToClone) : base(agentToClone) { }


        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public override async Task TestConnectionAsync()
        {
            using (var result = await OpenConnectionAsync().ConfigureAwait(false))
            {
                await result.CloseAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> exists.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database exists.</returns>
        public override async Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken)
        {

            if (CurrentDatabase.IsNullOrWhitespace()) throw new InvalidOperationException(Properties.Resources.CurrentDatabaseNullException);

            using (var conn = await OpenConnectionAsync(true).ConfigureAwait(false))
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, 
                    string.Format("SHOW DATABASES LIKE '{0}';", CurrentDatabase.Trim()), null, 
                    cancellationToken).ConfigureAwait(false);
                return (table.Rows.Count > 0);
            }                   

        }

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> is empty, i.e. contains no tables.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database contains any tables.</returns>
        public override async Task<bool> DatabaseEmptyAsync(CancellationToken cancellationToken)
        {

            if (CurrentDatabase.IsNullOrWhitespace()) throw new InvalidOperationException(Properties.Resources.CurrentDatabaseNullException);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, 
                    string.Format("SHOW TABLES FROM `{0}`;", CurrentDatabase.Trim()), null, cancellationToken).ConfigureAwait(false);
                await conn.CloseAsync();
                return (table.Rows.Count < 1);
            }                   

        }

        /// <summary>
        /// Gets a default database schema manager (to create or drop database schema, extract schema,
        /// check for schema errors against gauge schema)
        /// </summary>
        public override ISchemaManager GetDefaultSchemaManager() => new MySqlSchemaManager(this);

        /// <summary>
        /// Gets a default micro ORM service.
        /// </summary>
        public override IOrmService GetDefaultOrmService() => new MySqlOrmService(this);

        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        public override SqlAgentBase GetCopy() => new MySqlAgent(this);

        #region Transactions

        private static AsyncLocal<MySqlTransaction> asyncTransaction = new AsyncLocal<MySqlTransaction>();
        private MySqlTransaction instanceTransaction = null;


        private MySqlTransaction CurrentTransaction
        {
            get
            {
                if (UseTransactionPerInstance) return instanceTransaction;
                return asyncTransaction.Value;
            }
            set
            {
                if (UseTransactionPerInstance) instanceTransaction = value;
                else asyncTransaction.Value = value;
            }
        }


        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <exception cref="InvalidOperationException">if transaction is already in progress</exception>
        protected override async Task<object> TransactionBeginAsync(CancellationToken cancellationToken)
        {

            if (IsTransactionInProgress) throw new InvalidOperationException(Properties.Resources.CannotStartTransactionException);

            var connection = await OpenConnectionAsync();

            try
            {
                var transaction = await connection.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                // no point in setting AsyncLocal as it will be lost on exit
                // should use method RegisterTransactionForAsyncContext in the transaction method
                if (UseTransactionPerInstance) instanceTransaction = transaction;
                return transaction;
            }
            catch (Exception)
            {
                if (!connection.IsNull() && connection.State != ConnectionState.Closed)
                {
                    try { await connection.CloseAsync().ConfigureAwait(false); }
                    catch (Exception) { }
                }
                if (!connection.IsNull())
                {
                    try { connection.Dispose(); }
                    catch (Exception) { }
                }
                throw;
            }
            
        }

        /// <summary>
        /// As the TransactionBeginAsync method is Async, AsyncLocal value is lost outside of it's context
        /// and needs to be set in the context of the invoking method.
        /// </summary>
        /// <param name="transaction">a transaction that has been initiated by the TransactionBeginAsync method</param>
        protected override void RegisterTransactionForAsyncContext(object transaction)
        {

            if (UseTransactionPerInstance) return;

            if (transaction.IsNull()) throw new ArgumentNullException(nameof(transaction));

            var typedTransaction = transaction as MySqlTransaction;
            asyncTransaction.Value = typedTransaction ?? throw new ArgumentException(string.Format(
                Properties.Resources.InvalidTransactionType, transaction.GetType().FullName), nameof(transaction));

        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if no transaction in progress</exception>
        protected override void TransactionCommit()
        {

            if (!IsTransactionInProgress) throw new InvalidOperationException(Properties.Resources.NoTransactionToCommitException);

            try
            {
                CurrentTransaction.Commit();
            }
            catch (Exception ex)
            {
                
                try
                {
                    CurrentTransaction.Rollback();
                }
                catch (Exception e)
                {
                    LogAndThrow(ex.WrapSqlException("COMMIT", e));
                }

                LogAndThrow(ex.WrapSqlException("COMMIT"));

            }
            finally
            {
               CleanUpTransaction(); 
            }
        }
        
        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        /// <param name="ex">an exception that caused the rollback</param>
        protected override void TransactionRollback(Exception ex)
        {
            
            try
            {
                CurrentTransaction.Rollback();
            }
            catch (Exception e)
            {
                CleanUpTransaction();
                LogAndThrow((ex ?? new Exception(Properties.Resources.ManualRollbackException)).WrapSqlException("ROLLBACK", e));                
            }

            CleanUpTransaction();

            if (ex != null) throw ex;
        }

        private void CleanUpTransaction()
        {

            if (!IsTransactionInProgress) return;

            if (!CurrentTransaction.Connection.IsNull())
            {

                if (CurrentTransaction.Connection.State == ConnectionState.Open)
                {
                    try { CurrentTransaction.Connection.Close(); }
                    catch (Exception){}
                }

                try { CurrentTransaction.Connection.Dispose(); }
                catch (Exception) { }

            }

            try { CurrentTransaction.Dispose(); }
            catch (Exception) { }

            CurrentTransaction = null;

        }

        #endregion

        #region CRUD Methods

        /// <summary>
        /// Fetches data using SQL query token in the SQL repository.
        /// </summary>
        /// <param name="token">a token of the SQL query in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableAsync(string token, SqlParam[] parameters, 
            CancellationToken cancellationToken)
        {

            if (token.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(token));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, GetSqlQuery(token), parameters, 
                    cancellationToken).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, GetSqlQuery(token), parameters, 
                    cancellationToken).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Fetches data using SQL query tokens in the SQL repository.
        /// </summary>
        /// <param name="queries">a list of queries identified by tokens in the SQL repository
        /// and collections of SQL query parameters (null or empty array for none).</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>an array of <see cref="LightDataTable">LightDataTables</see> that contain
        /// data returned by the SQL queries.</returns>
        public override async Task<LightDataTable[]> FetchTablesAsync((string Token, SqlParam[] Parameters)[] queries, 
            CancellationToken cancellationToken)
        {

            if (null == queries || queries.Length < 1) throw new ArgumentNullException(nameof(queries));
            if (queries.Any(q => q.Token.IsNullOrWhitespace())) throw new ArgumentException(
                Properties.Resources.QueryTokenEmptyException, nameof(queries));
            
            var tasks = new List<Task<LightDataTable>>();

            if (this.IsTransactionInProgress)
            {

                foreach (var (Token, Parameters) in queries)
                {
                    tasks.Add(ExecuteCommandIntAsync<LightDataTable>(null, GetSqlQuery(Token), Parameters, cancellationToken));
                }

                return await Task.WhenAll(tasks).ConfigureAwait(false);

            }
            else
            {
                using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
                {

                    LightDataTable[] result;

                    try
                    {   
                        foreach (var (Token, Parameters) in queries) tasks.Add(FetchUsingConnectionAsync(conn, GetSqlQuery(Token), 
                            cancellationToken, Parameters));                        
                        result = (await Task.WhenAll(tasks).ConfigureAwait(false));
                    }
                    finally
                    {
                        if (conn != null)
                        {
                            if (conn.State != ConnectionState.Closed)
                            {
                                try { await conn.CloseAsync().ConfigureAwait(false); }
                                catch (Exception) { }
                            }
                            try { conn.Dispose(); }
                            catch (Exception) { }
                        }                                                
                    }

                    return result; 
                    
                }
            }            

        }

        /// <summary>
        /// Fetches data using raw SQL query (parameters should be prefixed by ?).
        /// </summary>
        /// <param name="sqlQuery">an SQL query to execute (parameters should be prefixed by ?)</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableRawAsync(string sqlQuery, SqlParam[] parameters, 
            CancellationToken cancellationToken)
        {

            if (sqlQuery.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(sqlQuery));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, sqlQuery, parameters, 
                    cancellationToken).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, sqlQuery, parameters, 
                    cancellationToken).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        public override Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields, CancellationToken cancellationToken)
        {
            
            if (table.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(table));
            if (null == fields || fields.Length < 1) throw new ArgumentNullException(nameof(fields));

            var preparedFields = new List<string>();
            foreach (var field in fields)
            {
                if (field.IsNullOrWhitespace())
                    throw new ArgumentException(Properties.Resources.FieldsEmptyException, nameof(fields));
                preparedFields.Add(field.ToConventional(this));
            }

            return FetchTableRawAsync(string.Format("SELECT {0} FROM {1};", 
                string.Join(", ", preparedFields.ToArray()), table.ToConventional(this)), 
                null, cancellationToken);

        }

        /// <summary>
        /// Executes an SQL statement, that inserts a new row, using SQL query token 
        /// in the SQL repository and returns last insert id.
        /// </summary>
        /// <param name="insertStatementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public override async Task<long> ExecuteInsertAsync(string insertStatementToken, SqlParam[] parameters)
        {

            if (insertStatementToken.IsNullOrWhitespace())
                throw new ArgumentNullException(nameof(insertStatementToken));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, GetSqlQuery(insertStatementToken), parameters, 
                    CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<long>(conn, GetSqlQuery(insertStatementToken), parameters, 
                    CancellationToken.None).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Executes a raw SQL statement, that inserts a new row, and returns last insert id.
        /// </summary>
        /// <param name="insertStatement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public override async Task<long> ExecuteInsertRawAsync(string insertStatement, SqlParam[] parameters)
        {

            if (insertStatement.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(insertStatement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, insertStatement, parameters, 
                    CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<long>(conn, insertStatement, parameters, 
                    CancellationToken.None).ConfigureAwait(false);  
            }

        }

        /// <summary>
        /// Executes an SQL statement using SQL query token in the SQL repository 
        /// and returns affected rows count.
        /// </summary>
        /// <param name="statementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public override async Task<int> ExecuteCommandAsync(string statementToken, SqlParam[] parameters)
        {

            if (statementToken.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(statementToken));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, GetSqlQuery(statementToken), parameters, 
                    CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<int>(conn, GetSqlQuery(statementToken), parameters, 
                    CancellationToken.None).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Executes a raw SQL statement and returns affected rows count.
        /// </summary>
        /// <param name="statement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public override async Task<int> ExecuteCommandRawAsync(string statement, SqlParam[] parameters)
        {

            if (statement.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(statement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, statement, parameters, 
                    CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<int>(conn, statement, parameters, 
                    CancellationToken.None).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Executes multiple SQL statements using one database connection but without a transaction.
        /// </summary>
        /// <param name="statements">a collection of the SQL statements to execute in batch</param>
        /// <remarks>Used when modifying databases and in other cases when transactions are not supported
        /// in order to reuse connection.</remarks>
        public override async Task ExecuteCommandBatchAsync(string[] statements)
        {
            
            if (null == statements || statements.Length < 1)
                throw new ArgumentNullException(nameof(statements));

            if (this.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.CannotExecuteBatchException);

            string currentStatement = string.Empty;

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                try
                {
                    using (var command = new MySqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandTimeout = QueryTimeOut;

                        foreach (var statement in statements)
                        {
                            if (!statement.IsNullOrWhitespace())
                            {
                                currentStatement = statement;
                                command.CommandText = statement;
                                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    LogAndThrow(ex.WrapSqlException(currentStatement));
                }
                finally
                {
                    if (conn != null && conn.State != ConnectionState.Closed)
                    {
                        try { await conn.CloseAsync(); }
                        catch (Exception) { }
                    }                                            
                }

            }

        }

        #endregion
        
        
        internal async Task<MySqlConnection> OpenConnectionAsync(bool withoutDatabase = false)
        {

            MySqlConnection result;
            if (CurrentDatabase.IsNullOrWhitespace() || withoutDatabase)
            {
                result = new MySqlConnection(BaseConnectionString);  
            }
            else
            {
                if (BaseConnectionString.Trim().EndsWith(";"))
                {
                    result = new MySqlConnection(BaseConnectionString + "Database=" + CurrentDatabase.Trim() + ";");
                }
                else
                {
                    result = new MySqlConnection(BaseConnectionString + ";Database=" + CurrentDatabase.Trim() + ";");
                }
            }

            try
            {
                await result.OpenAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleOpenConnectionException(result, ex).ConfigureAwait(false);
            }

            return result;

        }

        private async Task HandleOpenConnectionException(MySqlConnection conn, Exception ex)
        {   
             
            if (!conn.IsNull())
            {
                if (conn.State != ConnectionState.Closed)
                {
                    try { await conn.CloseAsync().ConfigureAwait(false); }
                    catch (Exception) { }
                }
                try { conn.Dispose(); }
                catch (Exception){}
            }

            var mySqlEx = ex as MySqlException;
            if (!mySqlEx.IsNull())
            {

                if (mySqlEx.Number == 28000 ||
                    mySqlEx.Number == 42000)
                {
                    throw new SqlException(Properties.Resources.SqlExceptionAccessDenied, mySqlEx.Number, string.Empty, mySqlEx);
                }

                if (mySqlEx.Number == 2003)
                {
                    throw new SqlException(Properties.Resources.SqlExceptionUnableToConnect,
                        mySqlEx.Number, string.Empty, mySqlEx);
                }

                throw mySqlEx.WrapSqlException();

            }

            throw ex;

        }


        private void AddParams(MySqlCommand command, SqlParam[] parameters)
        {

            command.Parameters.Clear();

            if (null == parameters || parameters.Length < 1) return;

            foreach (var p in parameters.Where(p => !p.ReplaceInQuery)) command.Parameters.AddWithValue(
                ParamPrefix + p.Name.Trim(), p.GetValue(this));

        }

        private string ReplaceParams(string sqlQuery, SqlParam[] parameters)
        {

            if (null == parameters || parameters.Length < 1) return sqlQuery;

            var result = sqlQuery;

            foreach (var parameter in parameters.Where(parameter => parameter.ReplaceInQuery))
            {
                if (parameter.Value.IsNull())
                {
                    result = result.Replace(parameter.Name.Trim(), "NULL");
                }
                else
                {
                    result = result.Replace(parameter.Name.Trim(), parameter.Value.ToString());
                }
            }

            return result;
        }


        internal void LogAndThrowInt(Exception ex) { LogAndThrow(ex); }


        private async Task<T> ExecuteCommandIntAsync<T>(MySqlConnection connection, string sqlStatement, 
            SqlParam[] parameters, CancellationToken cancellationToken)
        {

            var transaction = CurrentTransaction;

            try
            {
                using (var command = new MySqlCommand())
                {   

                    if (transaction.IsNull())
                    {
                        command.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
                    }
                    else
                    {
                        command.Connection = (MySqlConnection) transaction.Connection;
                        command.Transaction = transaction;
                    }

                    command.CommandTimeout = QueryTimeOut;
                    command.CommandText = ReplaceParams(sqlStatement, parameters);

                    AddParams(command, parameters);

                    if (typeof(T) == typeof(LightDataTable))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        return (T)(Object)(await LightDataTable.CreateAsync(reader).ConfigureAwait(false));
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        return (T)(Object)command.LastInsertedId;
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(Object)(await command.ExecuteNonQueryAsync().ConfigureAwait(false));
                    }
                    else throw new NotSupportedException(string.Format(Properties.Resources.NotSupportedExecutionTypeException,
                        typeof (T).FullName));

                }
            }
            catch (Exception ex)
            {

                if (transaction != null)
                {
                    try { transaction.Rollback(); }
                    catch (Exception e) {LogAndThrow(ex.WrapSqlException(sqlStatement + (parameters?.GetDescription() ?? string.Empty), e)); }
                    finally { CleanUpTransaction(); }   
                }

                LogAndThrow(ex.WrapSqlException(sqlStatement + (parameters?.GetDescription() ?? string.Empty)));
                throw;

            }
            finally
            {
                if (transaction.IsNull() && !connection.IsNull())
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        try { await connection.CloseAsync().ConfigureAwait(false); }
                        catch (Exception) { }
                    }
                    try { connection.Dispose(); }
                    catch (Exception) { }
                }
            } 
        }

        internal async Task<LightDataTable> FetchUsingConnectionAsync(MySqlConnection connection,
            string sqlStatement, CancellationToken cancellationToken, SqlParam[] parameters = null)
        {

            if (null == connection) throw new ArgumentNullException(nameof(connection));
            if (sqlStatement.IsNullOrWhitespace()) throw new ArgumentNullException(nameof(sqlStatement));

            try
            {
                using (var command = new MySqlCommand())
                {

                    command.Connection = connection;
                    command.CommandTimeout = QueryTimeOut;
                    command.CommandText = sqlStatement;
                    AddParams(command, parameters);

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        return await LightDataTable.CreateAsync(reader).ConfigureAwait(false);
                    }

                }
            }
            catch (Exception ex)
            {
                LogAndThrow(ex.WrapSqlException(sqlStatement));
                throw;
            }

        }

        protected override void DisposeManagedState()
        {
            var myListener = MySqlTrace.Listeners.OfType<MySqlTraceListener>().Where(l => l.BelongsTo(this));
            if (myListener != null) foreach (var item in myListener) MySqlTrace.Listeners.Remove(item);
        }

    }
}
