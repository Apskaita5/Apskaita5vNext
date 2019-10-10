using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.SQLite;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.DbSchema;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using Apskaita5.DAL.Common.MicroOrm;
using static Apskaita5.DAL.SQLite.Constants;

namespace Apskaita5.DAL.SQLite
{
    public class SqliteAgent : SqlAgentBase
    {

        #region Constants

        private const string AgentName = "SQLite connector";
        private const bool AgentIsFileBased = true;
        private const string AgentRootName = "";
        private const string AgentWildcart = "%";        

        #endregion

        #region Properties

        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, i. e. SQLite connector.
        /// </summary>
        public override string Name => AgentName;

        /// <summary>
        /// Gets an id of the concrete SQL implementation, i.e. SQLite.
        /// The id is used to select an appropriate SQL token dictionary.
        /// </summary>
        public override string SqlImplementationId => SqliteImplementationId;

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based, i.e. true.
        /// </summary>
        public override bool IsFileBased => AgentIsFileBased;

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent, i.e. none.
        /// </summary>
        public override string RootName => AgentRootName;

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation behind the SqlAgent, i.e. %.
        /// </summary>
        public override string Wildcart => AgentWildcart;

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public override bool IsTransactionInProgress => (CurrentTransaction != null);

        #endregion


        /// <summary>
        /// Initializes a new SqliteAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="databaseName">a name of the database to use (if any)</param>
        /// <param name="sqlDictionary">an implementation of SQL dictionary to use (if any)</param>
        /// <param name="logger">a logger to log errors and warnings (if any)</param>
        public SqliteAgent(string baseConnectionString, string databaseName, ISqlDictionary sqlDictionary, ILogger logger)
            : base(baseConnectionString, true, databaseName, sqlDictionary, logger)
        {
            if (databaseName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(databaseName));
            if (logger != null)
            {
                SQLiteLog.Log += SQLiteLog_Log;
                SQLiteLog.Enabled = true;
            }            
        }

        /// <summary>
        /// Clones an SqliteAgent instance.
        /// </summary>
        /// <param name="agentToClone">an SqliteAgent instance to clone</param>
        private SqliteAgent(SqliteAgent agentToClone) : base(agentToClone) { }

        private void SQLiteLog_Log(object sender, LogEventArgs e)
        {
            SQLiteErrorCode errorCode = SQLiteErrorCode.Unknown;
            try { errorCode = (SQLiteErrorCode)e.ErrorCode; }
            catch (Exception) { }
            if (errorCode == SQLiteErrorCode.Warning) _Logger.LogWarning(e.Message);
            else if (errorCode != SQLiteErrorCode.Ok) _Logger.LogError(e.Message);
        }



        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public override async Task TestConnectionAsync()
        {
            using (var result = await OpenConnectionAsync().ConfigureAwait(false))
            {                   
                result.Close();    
            }
        }
         
        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> exists.
        /// </summary>
        /// <returns>True if the database specified exists.</returns>
        public override Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(File.Exists(CurrentDatabase));
        }

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> is empty, i.e. contains no tables.
        /// </summary>
        /// <returns>True if the database contains any tables.</returns>
        public override async Task<bool> DatabaseEmptyAsync(CancellationToken cancellationToken)
        {    
            using (var conn = await OpenConnectionAsync())
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, 
                    "SELECT name, sql FROM sqlite_master WHERE type='table' AND NOT name LIKE 'sqlite_%';", 
                    null, CancellationToken.None).ConfigureAwait(false);
                conn.Close();
                return (table.Rows.Count < 1);
            }                   
        }

        /// <summary>
        /// Gets a default database schema manager (to create or drop database schema, extract schema,
        /// check for schema errors against gauge schema)
        /// </summary>
        public override ISchemaManager GetDefaultSchemaManager() => new SqliteSchemaManager(this);

        /// <summary>
        /// Gets a default micro ORM service.
        /// </summary>
        public override IOrmService GetDefaultOrmService() => new SqliteOrmService(this);

        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        public override SqlAgentBase GetCopy() => new SqliteAgent(this);

        #region Transactions

        private static AsyncLocal<SQLiteTransaction> asyncTransaction = new AsyncLocal<SQLiteTransaction>();
        private SQLiteTransaction instanceTransaction = null;


        private SQLiteTransaction CurrentTransaction
        {
            get
            {
                if (UseTransactionPerInstance) return instanceTransaction;
                return asyncTransaction.Value;
            }
            set
            {
                if (UseTransactionPerInstance) instanceTransaction = value;
                asyncTransaction.Value = value;
            }
        }


        /// <summary>
        /// Starts a new transaction.
        /// </summary> 
        /// <param name="cancellationToken">a cancelation token (if any); does nothing for SQLite implementation</param>
        /// <exception cref="InvalidOperationException">if transaction is already in progress</exception>
        protected override async Task<object> TransactionBeginAsync(CancellationToken cancellationToken)
        {
            if (IsTransactionInProgress) throw new InvalidOperationException(Properties.Resources.CannotStartTransactionException);

            var connection = await OpenConnectionAsync();

            try
            {
                var transaction = connection.BeginTransaction();
                // no point in setting AsyncLocal as it will be lost on exit
                // should use method RegisterTransactionForAsyncContext in the transaction method
                if (UseTransactionPerInstance) instanceTransaction = transaction;
                return transaction;
            }
            catch (Exception)
            {

                if (!connection.IsNull())
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        try { connection.Close(); }
                        catch (Exception) { }
                    }
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

            var typedTransaction = transaction as SQLiteTransaction;

            asyncTransaction.Value = typedTransaction ?? throw new ArgumentException(string.Format(
                Properties.Resources.InvalidTransactionTypeException, transaction.GetType().FullName), nameof(transaction));

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
                
                try { CurrentTransaction.Rollback(); }
                catch (Exception e) { LogAndThrow(ex.WrapSqlException("COMMIT", e)); }

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
                    try { CurrentTransaction.Connection.Close();}
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
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableAsync(string token, SqlParam[] parameters, 
            CancellationToken cancellationToken)
        {

            if (token.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(token));

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
        /// <returns>an array of <see cref="LightDataTable">LightDataTables</see> that contain
        /// data returned by the SQL queries.</returns>
        public override async Task<LightDataTable[]> FetchTablesAsync((string Token, SqlParam[] Parameters)[] queries,
            CancellationToken cancellationToken)
        {

            if (null == queries || queries.Length < 1) throw new ArgumentNullException(nameof(queries));
            if (queries.Any(q => q.Token.IsNullOrWhiteSpace())) throw new ArgumentException(
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
                                try { conn.Close(); }
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
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableRawAsync(string sqlQuery, SqlParam[] parameters,
            CancellationToken cancellationToken)
        {

            if (sqlQuery.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(sqlQuery));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, ReplaceParamsInRawQuery(sqlQuery, parameters), 
                    parameters, cancellationToken).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, ReplaceParamsInRawQuery(sqlQuery, parameters), 
                    parameters, cancellationToken).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        public override Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields,
            CancellationToken cancellationToken)
        {
            
            if (table.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(table));
            if (null == fields || fields.Length < 1) throw new ArgumentNullException(nameof(fields));
            if (fields.Any(field => field.IsNullOrWhiteSpace()))
                throw new ArgumentException(Properties.Resources.FieldsEmptyException, nameof(fields));

            var fieldsQuery = string.Join(", ", fields.Select(field => field.ToConventional(this)).ToArray());

            return FetchTableRawAsync(string.Format("SELECT {0} FROM {1};",
                fieldsQuery, table.ToConventional(this)), null, cancellationToken);

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

            if (insertStatementToken.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(insertStatementToken));

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

            if (insertStatement.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(insertStatement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, ReplaceParamsInRawQuery(insertStatement, parameters), 
                    parameters, CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<long>(conn, ReplaceParamsInRawQuery(insertStatement, parameters), 
                    parameters, CancellationToken.None).ConfigureAwait(false);   
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

            if (statementToken.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(statementToken));

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

            if (statement.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(statement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, ReplaceParamsInRawQuery(statement, parameters), 
                    parameters, CancellationToken.None).ConfigureAwait(false);

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                return await ExecuteCommandIntAsync<int>(conn, ReplaceParamsInRawQuery(statement, parameters), 
                    parameters, CancellationToken.None).ConfigureAwait(false);
            }

        }

        /// <summary>
        /// Executes multiple SQL statements using one database connection but without a transaction.
        /// </summary>
        /// <param name="statements">a collection of the SQL statements to execute in batch; 
        /// null or empty statements will be ignored, but at least one statement should be non empty;
        /// statements should be prepared for the current SQL agent implementation, i.e. SQLite</param>
        /// <remarks>Used when modifying databases and in other cases when transactions are not supported
        /// in order to reuse connection.</remarks>
        /// <exception cref="ArgumentNullException">Parameter statements is not specified.</exception>
        /// <exception cref="ArgumentException">At least one statement should be non empty.</exception>
        /// <exception cref="InvalidOperationException">Cannot execute batch while a transaction is in progress.</exception>
        public override async Task ExecuteCommandBatchAsync(string[] statements)
        {
            
            if (null == statements || statements.Length < 1)
                throw new ArgumentNullException(nameof(statements));
            if (statements.All(statement => statement.IsNullOrWhiteSpace()))
                throw new ArgumentException(Properties.Resources.StatementsEmptyException, nameof(statements));
            if (this.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.NoBatchInTransactionException);

            string currentStatement = string.Empty;

            using (var conn = await OpenConnectionAsync().ConfigureAwait(false))
            {
                try
                {
                    using (var command = new SQLiteCommand())
                    {

                        command.Connection = conn;
                        command.CommandTimeout = QueryTimeOut;

                        foreach (var statement in statements)
                        {
                            if (!statement.IsNullOrWhiteSpace())
                            {
                                command.CommandText = statement;
                                currentStatement = statement;
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
                    if (conn != null)
                    {
                        if (conn.State != ConnectionState.Closed)
                        {
                            try { conn.Close(); }
                            catch (Exception) { }
                        }
                        try { conn.Dispose(); }
                        catch (Exception) { }
                    }
                }

            }

        }

        #endregion
        
        internal async Task<SQLiteConnection> OpenConnectionAsync()
        {

            SQLiteConnection result;
            if (BaseConnectionString.Trim().StartsWith(";"))
            {
                result = new SQLiteConnection("Data Source=" + CurrentDatabase.Trim() + BaseConnectionString);
            }
            else
            {
                result = new SQLiteConnection("Data Source=" + CurrentDatabase.Trim() + ";" + BaseConnectionString);
            }
            
            try
            {
                
                await result.OpenAsync().ConfigureAwait(false);

                // foreign keys are disabled by default in SQLite
                using (var command = new SQLiteCommand())
                {
                    command.Connection = result;
                    command.CommandText = "PRAGMA foreign_keys = ON;";
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                HandleOpenConnectionException(result, ex);
            }

            return result;

        }

        private void HandleOpenConnectionException(SQLiteConnection conn, Exception ex)
        {

            if (conn != null)
            {
                if (conn.State != ConnectionState.Closed)
                {
                    try { conn.Close(); }
                    catch (Exception){}
                }
                try { conn.Dispose(); }
                catch (Exception){} 
            }
            
            var sqliteEx = ex as SQLiteException;
            if (sqliteEx != null)
            {

                if (sqliteEx.ErrorCode == (int)SQLiteErrorCode.Auth ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.Auth_User ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.NotADb ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.CantOpen ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.Corrupt)
                {
                    throw new SqlException(Properties.Resources.SqlExceptionPasswordInvalid, sqliteEx.ErrorCode, 
                        string.Empty, sqliteEx);
                }

                if (sqliteEx.ErrorCode == (int)SQLiteErrorCode.NotFound)
                {
                    throw new SqlException(string.Format(Properties.Resources.SqlExceptionDatabaseNotFound, 
                        CurrentDatabase, sqliteEx.Message), sqliteEx.ErrorCode, string.Empty, sqliteEx);
                }

                throw sqliteEx.WrapSqlException();

            }

            throw ex;

        }



        private void AddParams(SQLiteCommand command, SqlParam[] parameters)
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

        private string ReplaceParamsInRawQuery(string sqlQuery, SqlParam[] parameters)
        {

            if (null == parameters || parameters.Length < 1) return sqlQuery;

            return parameters.Where(parameter => parameter.ReplaceInQuery).
                Aggregate(sqlQuery, (current, parameter) => 
                    current.Replace("?" + parameter.Name.Trim(), ParamPrefix + parameter.Name.Trim()));

        }


        private async Task<T> ExecuteCommandIntAsync<T>(SQLiteConnection connection, string sqlStatement, 
            SqlParam[] parameters, CancellationToken cancellationToken)
        {

            var transaction = CurrentTransaction;
            var commandText = string.Empty;

            try
            {
                using (var command = new SQLiteCommand())
                {

                    if (transaction.IsNull())
                    {
                        command.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
                    }
                    else
                    {
                        command.Connection = transaction.Connection;
                        command.Transaction = transaction;
                    }

                    command.CommandTimeout = QueryTimeOut;
                    commandText = ReplaceParams(sqlStatement, parameters);
                    if (typeof (T) == typeof (long))
                    {
                        if (commandText.Trim().EndsWith(";"))
                        {
                            commandText = commandText + " SELECT last_insert_rowid() AS LastInsertId;";
                        }
                        else
                        {
                            commandText = commandText + "; SELECT last_insert_rowid() AS LastInsertId;";
                        }
                    }

                    command.CommandText = commandText;

                    AddParams(command, parameters);

                    if (typeof(T) == typeof(LightDataTable))
                    {
                        var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                        return (T)(Object)(await LightDataTable.CreateAsync(reader).ConfigureAwait(false));
                    }                        
                    else if (typeof (T) == typeof (long))
                    {
                        return (T)(object)(long)(await command.ExecuteScalarAsync().ConfigureAwait(false));
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(Object)(await command.ExecuteNonQueryAsync().ConfigureAwait(false));
                    }                        
                    else throw new NotSupportedException(string.Format(Properties.Resources.InvalidInternalExecuteParamException,
                        typeof (T).FullName));

                }
            }
            catch (Exception ex)
            {  

                if (!transaction.IsNull())
                {
                    try { transaction.Rollback(); }
                    catch (Exception e) { LogAndThrow(ex.WrapSqlException(commandText + (parameters?.GetDescription() ?? string.Empty), e)); }
                    finally { CleanUpTransaction(); }
                }

                LogAndThrow(ex.WrapSqlException(commandText + (parameters?.GetDescription() ?? string.Empty)));
                throw;

            }
            finally
            {
                if (transaction.IsNull() && !connection.IsNull())
                {
                    if (connection.State != ConnectionState.Closed)
                    {
                        try { connection.Close(); }
                        catch (Exception) { }
                    }
                    try { connection.Dispose(); }
                    catch (Exception) { }
                }
            }

        }

        internal async Task<LightDataTable> FetchUsingConnectionAsync(SQLiteConnection connection,
            string sqlStatement, CancellationToken cancellationToken, SqlParam[] parameters = null)
        {
            try
            {
                using (var command = new SQLiteCommand())
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


        internal void LogAndThrowInt(Exception ex)
        {
            LogAndThrow(ex);
        }


        protected override void DisposeManagedState()
        {
            try { SQLiteLog.Log -= SQLiteLog_Log; }
            catch (Exception) { }
        }

    }
}
