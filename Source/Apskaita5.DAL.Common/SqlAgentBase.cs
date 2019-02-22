using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a base class for a concrete SQL implementation, e.g. MySql, SQLite, etc.
    /// </summary>
    /// <remarks>Should be stored in ApplicationContext.Local context (in thread for client, 
    /// in http context on server).</remarks>
    public abstract class SqlAgentBase
    {

        private readonly string _sqlRepositoryPath = string.Empty;
        protected readonly string _baseConnectionString = string.Empty;
        protected string _currentDatabase = string.Empty;
        private int _queryTimeOut = 10000;

        private static readonly string[] ParamLetters = new string[]{"A", "B", "C", "D", "E", 
            "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "Q", "P", "R", "S", "T", "U", "V", "Z", "W"};
        private static List<string> _paramDictionary = null;


        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, e.g. MySql, SQLite, etc.
        /// </summary>
        public abstract string Name { get; }

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
        /// Gets or sets a query timeout in ms. (default is 10.000 ms)
        /// </summary>
        public int QueryTimeOut {
            get { return _queryTimeOut; }
            set { _queryTimeOut = value; }
        }

        /// <summary>
        /// Gets a prefix of the names of the files that contain SQL queries written 
        /// for the SQL implementation behind the SqlAgent, e.g. mysql_, sqlite_, etc.
        /// All XML files with this prefix is loaded into SQL dictionary at runtime. 
        /// It is needed in order to support various plugin repositories.
        /// </summary>
        public abstract string SqlRepositoryFileNamePrefix { get; }

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation behind the SqlAgent, e.g. %, *, etc.
        /// </summary>
        public abstract string Wildcart { get; }

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public abstract bool IsTransactionInProgress { get; }


        /// <summary>
        /// Gets a path to the folder with the files that contain SQL repositories. 
        /// </summary>
        /// <remarks>Should be initialized when creating an SqlAgent instance.</remarks>
        public string SqlRepositoryPath {
            get { return _sqlRepositoryPath; }
        }

        /// <summary>
        /// Gets a connection string that does not include database parameter. 
        /// </summary>
        /// <remarks>Should be initialized when creating an SqlAgent instance.</remarks>
        public string BaseConnectionString
        {
            get { return _baseConnectionString; }
        }

       /// <summary>
        /// Gets or sets the current database name (string.Empty for no database).
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot change database while a transaction is in progress.</exception>
        public string CurrentDatabase {
            get { return _currentDatabase; }
            set
            {
                if (this.IsTransactionInProgress)
                    throw new InvalidOperationException(Properties.Resources.SqlAgentBase_CannotChangeDatabase);
                _currentDatabase = value.NotNullValue().Trim();
            }
        }


        /// <summary>
        /// Initializes a new SqlAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="allowEmptyConnString">whether the SqlAgent implementation can handle
        /// empty connection string (e.g. when the database parameter is the only parameter)</param>
        /// <param name="sqlRepositoryPath">a path to the folder with the files that contain SQL repositories</param>
        /// <param name="sqlTokensUsed">whether the user class is going to use SQL query tokens
        /// i.e. sqlRepositoryPath is required</param>
        protected SqlAgentBase(string baseConnectionString, bool allowEmptyConnString, 
            string sqlRepositoryPath, bool sqlTokensUsed)
        {
            
            if (sqlRepositoryPath.IsNullOrWhiteSpace() && sqlTokensUsed)
                throw new ArgumentNullException(nameof(sqlRepositoryPath));
            if (baseConnectionString.IsNullOrWhiteSpace() && !allowEmptyConnString)
                throw new ArgumentNullException(nameof(baseConnectionString));

            _sqlRepositoryPath = sqlRepositoryPath.NotNullValue().Trim();
            _baseConnectionString = baseConnectionString.NotNullValue().Trim();

        }


        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        public SqlAgentBase GetCopy()
        {
            return this.GetCopyInt();
        }

        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        protected abstract SqlAgentBase GetCopyInt();

        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public abstract void TestConnection();

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        public void ExecuteInTransaction(Action method)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));

            }
            else
            {

                if (method == null) throw new ArgumentNullException(nameof(method));

                TransactionBegin();
                isOwner = true;

            }

            try
            {
                method();
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
            }            

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        public void ExecuteInTransaction(Action<SqlAgentBase> method)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {    
                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));
            }
            else
            {         
                if (method == null) throw new ArgumentNullException(nameof(method));
                TransactionBegin();
                isOwner = true;
            }

            try
            {
                method(this);
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="parameter">a custom parameter to pass to the method</param>
        public void ExecuteInTransaction<T>(Action<SqlAgentBase, T> method, T parameter)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {
                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));
            }
            else
            {
                if (method == null) throw new ArgumentNullException(nameof(method));
                TransactionBegin();
                isOwner = true;
            }

            try
            {
                method(this, parameter);
                if (isOwner) TransactionCommit();
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        public TResult ExecuteInTransaction<TResult>(Func<TResult> method)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (method == null) throw new ArgumentNullException(nameof(method));

                TransactionBegin();
                isOwner = true;

            }

            try
            {
                var result = method();
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
                return default(TResult);
            }

        }

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        public TResult ExecuteInTransaction<TResult>(Func<SqlAgentBase, TResult> method)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (method == null) throw new ArgumentNullException(nameof(method));

                TransactionBegin();
                isOwner = true;

            }

            try
            {
                var result = method(this);
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
                return default(TResult);
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
        public TResult ExecuteInTransaction<T, TResult>(Func<SqlAgentBase, T, TResult> method, T parameter)
        {

            bool isOwner = false;

            if (IsTransactionInProgress)
            {

                if (method == null) TransactionRollback(new ArgumentNullException(nameof(method)));
                return default(TResult);

            }
            else
            {

                if (method == null) throw new ArgumentNullException(nameof(method));

                TransactionBegin();
                isOwner = true;

            }

            try
            {
                var result = method(this, parameter);
                if (isOwner) TransactionCommit();
                return result;
            }
            catch (Exception ex)
            {
                if (!IsTransactionInProgress) throw;
                TransactionRollback(ex);
                return default(TResult);
            }

        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        protected virtual void TransactionBegin()
        {
            if (this.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.SqlAgentBase_CannotStartTransaction);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if no transaction in progress</exception>
        protected virtual void TransactionCommit()
        {
            if (!this.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.SqlAgentBase_NoTransactionToCommit);
        }

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        /// <param name="ex">an exception that caused the rollback</param>
        protected virtual void TransactionRollback(Exception ex)
        {
            if (!this.IsTransactionInProgress) 
                throw new InvalidOperationException(Properties.Resources.SqlAgentBase_NoTransactionToRollback);
        }

        /// <summary>
        /// Fetches data using SQL query token in the SQL repository.
        /// </summary>
        /// <param name="token">a token of the SQL query in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public abstract LightDataTable FetchTable(string token, SqlParam[] parameters);

        /// <summary>
        /// Fetches data using raw SQL query (parameters should be prefixed by ?).
        /// </summary>
        /// <param name="sqlQuery">an SQL query to execute (parameters should be prefixed by ?)</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public abstract LightDataTable FetchTableRaw(string sqlQuery, SqlParam[] parameters);

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        /// <remarks>Used to fetch general company data and for SQL migration functionality.</remarks>
        public abstract LightDataTable FetchTableFields(string table, string[] fields);

        /// <summary>
        /// Executes an SQL statement, that inserts a new row, using SQL query token 
        /// in the SQL repository and returns last insert id.
        /// </summary>
        /// <param name="insertStatementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public abstract Int64 ExecuteInsert(string insertStatementToken, SqlParam[] parameters);

        /// <summary>
        /// Executes a raw SQL statement, that inserts a new row, and returns last insert id.
        /// </summary>
        /// <param name="insertStatement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public abstract Int64 ExecuteInsertRaw(string insertStatement, SqlParam[] parameters);

        /// <summary>
        /// Executes an SQL statement using SQL query token in the SQL repository 
        /// and returns affected rows count.
        /// </summary>
        /// <param name="statementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public abstract int ExecuteCommand(string statementToken, SqlParam[] parameters);

        /// <summary>
        /// Executes a raw SQL statement and returns affected rows count.
        /// </summary>
        /// <param name="statement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public abstract int ExecuteCommandRaw(string statement, SqlParam[] parameters);

        /// <summary>
        /// Executes multiple SQL statements using one database connection but without a transaction.
        /// </summary>
        /// <param name="statements">a collection of the SQL statements to execute in batch</param>
        /// <remarks>Used when modifying databases and in other cases when transactions are not supported
        /// in order to reuse connection.</remarks>
        public abstract void ExecuteCommandBatch(string[] statements);


        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary>
        public abstract DbSchema GetDbSchema();

        /// <summary>
        /// Compares the current database definition to the gauge definition definition read from 
        /// the file specified and returns a list of DbSchema errors, i.e. inconsistencies found 
        /// and SQL statements to repair them.
        /// </summary>
        /// <param name="dbSchemaFolderPath">the path to the folder that contains gauge schema files
        /// (all files loaded in order to support plugins that may require their own tables)</param>
        /// <exception cref="ArgumentNullException">databaseName is not specified</exception>
        /// <exception cref="ArgumentNullException">dbSchemaFolderPath is not specified</exception>
        /// <exception cref="ArgumentException">dbSchemaFolderPath contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="PathTooLongException">The specified dbSchemaFolderPath, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified dbSchemaFolderPath is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in dbSchemaFolderPath was not found.</exception>
        /// <exception cref="NotSupportedException">dbSchemaFolderPath is in an invalid format.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public List<DbSchemaError> GetDbSchemaErrors(string dbSchemaFolderPath)
        {
            var gaugeSchema = new DbSchema();
            gaugeSchema.LoadXmlFolder(dbSchemaFolderPath);
            var actualSchema = this.GetDbSchema();
            if (gaugeSchema.Tables.Count < 1) throw new ArgumentException(Properties.Resources.SqlAgentBase_GaugeSchemaEmpty);
            return this.GetDbSchemaErrors(gaugeSchema, actualSchema);
        }

        /// <summary>
        /// Compares the actualSchema definition to the gaugeSchema definition and returns
        /// a list of DbSchema errors, i.e. inconsistencies found and SQL statements to repair them.
        /// </summary>
        /// <param name="gaugeSchema">the gauge schema definition to compare the actualSchema against</param>
        /// <param name="actualSchema">the schema to check for inconsistencies (and repair)</param>
        public abstract List<DbSchemaError> GetDbSchemaErrors(DbSchema gaugeSchema, DbSchema actualSchema);


        /// <summary>
        /// Gets an SQL script to create a database for the dbSchema specified.
        /// </summary>
        /// <param name="dbSchema">the database schema to get the create database script for</param>
        public abstract string GetCreateDatabaseSql(DbSchema dbSchema);

        /// <summary>
        /// Creates a new database using DbSchema.
        /// </summary>
        /// <param name="databaseName">a name of the new database to create</param>
        /// <param name="dbSchemaFolderPath">the path to the folder that contains gauge schema files
        /// (all files loaded in order to support plugins that may require their own tables)</param>
        /// <remarks>After creating a new database the <see cref="CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        /// <exception cref="ArgumentNullException">databaseName is not specified</exception>
        /// <exception cref="ArgumentNullException">dbSchemaFolderPath is not specified</exception>
        /// <exception cref="ArgumentException">dbSchemaFolderPath contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="PathTooLongException">The specified dbSchemaFolderPath, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified dbSchemaFolderPath is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in dbSchemaFolderPath was not found.</exception>
        /// <exception cref="NotSupportedException">dbSchemaFolderPath is in an invalid format.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void CreateDatabase(string databaseName, string dbSchemaFolderPath)
        {
            if (databaseName.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(databaseName));
            if (IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.SqlAgentBase_CannotCreateDatabase);
            var dbSchema = new DbSchema();
            dbSchema.LoadXmlFolder(dbSchemaFolderPath);
            CreateDatabase(databaseName, dbSchema);
        }

        /// <summary>
        /// A method that should do the actual new database creation.
        /// </summary>
        /// <param name="databaseName">a name of the new database to create</param>
        /// <param name="dbSchema">a DbSchema to use for the new database</param>
        /// <remarks>After creating a new database the <see cref="CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        protected abstract void CreateDatabase(string databaseName, DbSchema dbSchema);

        /// <summary>
        /// Drops (deletes) the database specified.
        /// </summary>
        /// <param name="databaseName">the name of the database to drop</param>
        public abstract void DropDatabase(string databaseName);


        /// <summary>
        /// Creates a clone of the database using another SqlAgent.
        /// </summary>
        /// <param name="cloneDatabaseName">a name of the cone database to create</param>
        /// <param name="cloneSqlAgent">an SQL agent to use when creating clone database</param>
        /// <param name="schemaToUse">a database schema to use (enforce), 
        /// if null the schema ir read from the database cloned</param>
        /// <param name="worker">a BackgroundWorker to report the progress to (if used)</param>
        public void CloneDatabase(string cloneDatabaseName, SqlAgentBase cloneSqlAgent,
            DbSchema schemaToUse, System.ComponentModel.BackgroundWorker worker)
        {

            if (_currentDatabase.IsNullOrWhiteSpace()) 
                throw new InvalidOperationException(Properties.Resources.SqlAgentBase_CurrentDatabaseNull);
            if (cloneDatabaseName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(cloneDatabaseName));
            if (cloneSqlAgent == null)
                throw new ArgumentNullException(nameof(cloneSqlAgent));

            if (worker != null)
                worker.ReportProgress(0, Properties.Resources.SqlAgentBase_FetchingDatabaseSchema);

            if (schemaToUse == null) schemaToUse = this.GetDbSchema();

            if (worker != null && worker.CancellationPending)
            {
                worker.ReportProgress(100, Properties.Resources.SqlAgentBase_DatabaseCloneCanceled);
                throw new Exception(Properties.Resources.SqlAgentBase_DatabaseCloneCanceled);
            }

            if (worker != null)
                worker.ReportProgress(0, Properties.Resources.SqlAgentBase_CreatingCloneDatabase);

            cloneSqlAgent.CreateDatabase(cloneDatabaseName, schemaToUse);

            if (worker != null && worker.CancellationPending)
            {
                worker.ReportProgress(100, Properties.Resources.SqlAgentBase_DatabaseCloneCanceledAfterCreate);
                throw new Exception(Properties.Resources.SqlAgentBase_DatabaseCloneCanceledAfterCreate);
            }

            try
            {

                cloneSqlAgent.TransactionBegin();

                cloneSqlAgent.DisableForeignKeysForCurrentTransaction();

                this.CopyData(schemaToUse, cloneSqlAgent, worker);

                cloneSqlAgent.TransactionCommit();

                if (worker != null)
                    worker.ReportProgress(100, Properties.Resources.SqlAgentBase_DatabaseCloneCompleted);

            }
            catch (Exception ex)
            {
                if (cloneSqlAgent.IsTransactionInProgress)
                    cloneSqlAgent.TransactionRollback(ex);
                throw;
            }

        }

        /// <summary>
        /// Copies table data from the current SqlAgent instance to the target SqlAgent instance.
        /// </summary>
        /// <param name="schema">a schema of the database to copy the data</param>
        /// <param name="targetSqlAgent">the target SqlAgent to copy the data to</param>
        /// <param name="worker">BackgroundWorker to report the progress to (if used)</param>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.
        /// Basicaly iterates tables, selects data, creates an IDataReader for the table and passes it to the 
        /// <see cref="InsertTableData">InsertTableData</see> method of the target SqlAgent.</remarks>
        protected abstract void CopyData(DbSchema schema, SqlAgentBase targetSqlAgent,
            System.ComponentModel.BackgroundWorker worker);

        /// <summary>
        /// Disables foreign key checks for the current transaction.
        /// </summary>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.</remarks>
        protected abstract void DisableForeignKeysForCurrentTransaction();

        /// <summary>
        /// Invokes protected <see cref="InsertTableData">InsertTableData</see>
        /// method on target SqlAgent. Used to bypass cross instance protected method
        /// access limitation.
        /// </summary>
        /// <param name="target">the SqlAgent to invoke the <see cref="InsertTableData">InsertTableData</see>
        /// method on</param>
        /// <param name="table">a schema of the table to insert the data to</param>
        /// <param name="reader">an IDataReader to read the table data from</param>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.
        /// The insert is performed using a transaction that is already initiated by the 
        /// <see cref="CloneDatabase">CloneDatabase</see>.</remarks>
        protected static void CallInsertTableData(SqlAgentBase target, DbTableSchema table, 
            IDataReader reader)
        {
            target.InsertTableData(table, reader);
        }

        /// <summary>
        /// Inserts table data from the reader to the current SqlAgent instance,
        /// </summary>
        /// <param name="table">a schema of the table to insert the data to</param>
        /// <param name="reader">an IDataReader to read the table data from.</param>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.
        /// The insert is performed using a transaction that is already initiated by the 
        /// <see cref="CloneDatabase">CloneDatabase</see>.</remarks>
        protected abstract void InsertTableData(DbTableSchema table, IDataReader reader);


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
            return SqlDictionary.GetSqlQuery(token, this);
        }

        /// <summary>
        /// Gets a new instance of the SqlSchemaError for a repairable field level error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="table">the name of the database table which field is inconsistent  (must be specified)</param>
        /// <param name="field">the name of the database field that is inconsistent</param>
        /// <param name="sqlStatementsToRepair">a collection of the SQL statements 
        /// that should be issued to repair the error (must be specified)</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        /// <exception cref="ArgumentNullException">Error table is not specified.</exception>
        /// <exception cref="ArgumentNullException">SQL statements to repair the error is not specified.</exception>
        /// <exception cref="ArgumentException">No SQL statement could be empty.</exception>
        protected DbSchemaError GetDbSchemaError(DbSchemaErrorType errorType, string description, 
            string table, string field, string[] sqlStatementsToRepair)
        {
            return new DbSchemaError(errorType,description,table,field,sqlStatementsToRepair);
        }

        /// <summary>
        /// Gets a new instance of the SqlSchemaError for a repairable table level error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="table">the name of the database table which schema is inconsistent  (must be specified)</param>
        /// <param name="sqlStatementsToRepair">a collection of the SQL statements 
        /// that should be issued to repair the error (must be specified)</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        /// <exception cref="ArgumentNullException">Error table is not specified.</exception>
        /// <exception cref="ArgumentNullException">SQL statements to repair the error is not specified.</exception>
        /// <exception cref="ArgumentException">No SQL statement could be empty.</exception>
        protected DbSchemaError GetDbSchemaError(DbSchemaErrorType errorType, string description, 
            string table, string[] sqlStatementsToRepair)
        {
            return new DbSchemaError(errorType, description, table, sqlStatementsToRepair);
        }

        /// <summary>
        /// Gets a new instance of the SqlSchemaError for a repairable database level error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="sqlStatementsToRepair">a collection of the SQL statements 
        /// that should be issued to repair the error (must be specified)</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        /// <exception cref="ArgumentNullException">Error table is not specified.</exception>
        /// <exception cref="ArgumentNullException">SQL statements to repair the error is not specified.</exception>
        /// <exception cref="ArgumentException">No SQL statement could be empty.</exception>
        protected DbSchemaError GetDbSchemaError(DbSchemaErrorType errorType, string description, 
            string[] sqlStatementsToRepair)
        {
            return new DbSchemaError(errorType,description,sqlStatementsToRepair);
        }

        /// <summary>
        /// Gets a new instance of the SqlSchemaError for an unrepairable error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="table">the name of the database table which field is inconsistent</param>
        /// <param name="field">the name of the database field that is inconsistent</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        protected DbSchemaError GetUnrepairableDbSchemaError(DbSchemaErrorType errorType, 
            string description, string table, string field)
        {
            return new DbSchemaError(errorType, description, table, field);
        }

        /// <summary>
        /// Gets a parameter name for a parameter at the index (position) specified.
        /// </summary>
        /// <param name="index">the zero based index (position) of the parameter</param>
        /// <remarks>Infrastructure for insert statement generation.</remarks>
        protected static string GetParameterName(int index)
        {

            if (_paramDictionary == null)
            {

                _paramDictionary = new List<string>();
                for (int i = 1; i <= 400; i++)
                {
                    _paramDictionary.Add(ParamLetters[(int)Math.Ceiling((i / 24.0) - 1)]
                        + ParamLetters[i - (int)(Math.Ceiling((i / 24.0) - 1) * 24 + 1)]);
                }
                _paramDictionary.Remove("AS");
                _paramDictionary.Remove("BY");
                _paramDictionary.Remove("IF");
                _paramDictionary.Remove("IN");
                _paramDictionary.Remove("IS");
                _paramDictionary.Remove("ON");
                _paramDictionary.Remove("OR");
                _paramDictionary.Remove("TO");

            }

            if (index < 0 || index + 1 > _paramDictionary.Count())
                throw new IndexOutOfRangeException();

            return _paramDictionary[index];

        }

    }
}
