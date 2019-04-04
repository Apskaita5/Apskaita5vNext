using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using Apskaita5.DAL.Common;
using System.Threading.Tasks;
using System.Threading;

namespace Apskaita5.DAL.MySql
{
    /// <summary>
    /// Represents an abstraction over the native MySql data access and schema methods.
    /// </summary>
    /// <remarks>Should be stored in ApplicationContext.Local context (in thread for client, 
    /// in http context on server).</remarks>
    public class MySqlAgent : SqlAgentBase
    {

        private const string AgentName = "MySql connector";
        private const bool AgentIsFileBased = false;
        private const string AgentRootName = "root";
        private const string AgentSqlRepositoryFileNamePrefix = "mysql_";
        private const string AgentWildcart = "%";
        private const string ParamPrefix = "?";
        private const string DefaultEngine = "InnoDB";
        private const string DefaultCharset = "utf8";

        private string _engine = DefaultEngine;
        private string _charset = DefaultCharset;

        private static AsyncLocal<MySqlTransaction> asyncTransaction = new AsyncLocal<MySqlTransaction>();
        private MySqlTransaction instanceTransaction = null;



        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, i. e. MySql connector.
        /// </summary>
        public override string Name
        {
            get { return AgentName; }
        }

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based, i.e. false.
        /// </summary>
        public override bool IsFileBased
        {
            get { return AgentIsFileBased; }
        }

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent, i.e. root.
        /// </summary>
        public override string RootName
        {
            get { return AgentRootName; }
        }

        /// <summary>
        /// Gets a prefix of the names of the files that contain SQL queries written 
        /// for the SQL implementation behind the SqlAgent, i.e. mysql_.
        /// All XML files with this prefix is loaded into SQL dictionary at runtime. 
        /// It is needed in order to support various plugin repositories.
        /// </summary>
        public override string SqlRepositoryFileNamePrefix
        {
            get { return AgentSqlRepositoryFileNamePrefix; }
        }

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation behind the SqlAgent, i.e. %.
        /// </summary>
        public override string Wildcart
        {
            get { return AgentWildcart; }
        }

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
                asyncTransaction.Value = value;
            }
        }

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public override bool IsTransactionInProgress
        {
            get { return CurrentTransaction != null; }
        }

        /// <summary>
        /// Gets or sets the SQL engine to use when creating a database. (default - InnoDB)
        /// </summary>
        public string Engine
        {
            get { return _engine; }
            set
            {
                if (value == null)
                {
                    _engine = string.Empty;
                }
                else
                {
                    _engine = value.Trim();
                }
            }
        }

        /// <summary>
        /// Gets or sets the default charset to use when creating a database. (default - utf8)
        /// </summary>
        public string Charset
        {
            get { return _charset; }
            set
            {
                if (value == null)
                {
                    _charset = string.Empty;
                }
                else
                {
                    _charset = value.Trim();
                }
            }
        }


        /// <summary>
        /// Initializes a new MySqlAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="sqlRepositoryPath">a path to the folder with the the files that contain SQL repositories</param>
        /// <param name="sqlTokensUsed">whether the user class is going to use SQL query tokens
        /// i.e. sqlRepositoryPath is required</param>
        public MySqlAgent(string baseConnectionString, string sqlRepositoryPath, bool sqlTokensUsed)
            : base(baseConnectionString, false, sqlRepositoryPath, sqlTokensUsed) { }


        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        protected override SqlAgentBase GetCopyInt()
        {
            var sqlTokensUsed = (this.SqlRepositoryPath != null && !string.IsNullOrEmpty(this.SqlRepositoryPath.Trim()));
            return new MySqlAgent(_baseConnectionString, this.SqlRepositoryPath, sqlTokensUsed);
        }

        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public override async Task TestConnectionAsync()
        {
            using (var result = await OpenConnectionAsync())
            {
                await result.CloseAsync();    
            }
        }

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if transaction is already in progress</exception>
        protected override async Task TransactionBeginAsync()
        {
            var result = await OpenConnectionAsync();
            CurrentTransaction = await result.BeginTransactionAsync();
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if no transaction in progress</exception>
        protected override void TransactionCommit()
        {

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
                    throw new Exception(string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}Commit exception: {1}{2}Rollback exception: {3}",
                        Environment.NewLine, ex.Message, Environment.NewLine, e.Message), ex);
                }

                throw;

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
                if (ex == null)
                {
                    throw new Exception(string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}{1}",
                        Environment.NewLine, e.Message));
                }
                else
                {
                    throw new Exception(string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}Initial exception: {1}{2}Rollback exception: {3}",
                        Environment.NewLine, ex.Message, Environment.NewLine, e.Message), ex);   
                }
            }
            finally
            {
                CleanUpTransaction();
            }

            if (ex != null) throw ex;

        }

        /// <summary>
        /// Fetches data using SQL query token in the SQL repository.
        /// </summary>
        /// <param name="token">a token of the SQL query in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableAsync(string token, SqlParam[] parameters)
        {

            if (token == null || string.IsNullOrEmpty(token.Trim())) throw new ArgumentNullException(nameof(token));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, CurrentTransaction, 
                    GetSqlQuery(token), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, null, GetSqlQuery(token), parameters);
            }

        }

        /// <summary>
        /// Fetches data using SQL query tokens in the SQL repository.
        /// </summary>
        /// <param name="queries">a list of queries where the key is a token of the SQL query in the SQL repository
        /// and the value is a collection of the SQL query parameters (null or empty array for none)</param>
        /// <returns>an array of <see cref="LightDataTable">LightDataTables</see> that contain
        /// data returned by the SQL queries.</returns>
        public override async Task<LightDataTable[]> FetchTablesAsync(KeyValuePair<string, SqlParam[]>[] queries)
        {

            if (queries == null || queries.Length < 1) throw new ArgumentNullException(nameof(queries));
            foreach (var query in queries)
            {
                if (query.Key == null || query.Key.Trim().Length < 1)
                    throw new ArgumentException("Query token cannot be empty.", nameof(queries));
            }

            var tasks = new List<Task<LightDataTable>>();

            if (this.IsTransactionInProgress)
            {

                foreach (var query in queries)
                {
                    tasks.Add(ExecuteCommandIntAsync<LightDataTable>(null, CurrentTransaction,
                        GetSqlQuery(query.Key), query.Value));
                }

                return await Task.WhenAll(tasks);

            }
            else
            {
                using (var conn = await OpenConnectionAsync())
                {
                    foreach (var query in queries)
                    {
                        tasks.Add(ExecuteCommandIntAsync<LightDataTable>(conn, null, 
                            GetSqlQuery(query.Key), query.Value));
                    }

                    return await Task.WhenAll(tasks);
                }
            }            

        }

        /// <summary>
        /// Fetches data using raw SQL query (parameters should be prefixed by ?).
        /// </summary>
        /// <param name="sqlQuery">an SQL query to execute (parameters should be prefixed by ?)</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        public override async Task<LightDataTable> FetchTableRawAsync(string sqlQuery, SqlParam[] parameters)
        {

            if (sqlQuery == null || string.IsNullOrEmpty(sqlQuery.Trim())) throw new ArgumentNullException(nameof(sqlQuery));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, CurrentTransaction, sqlQuery, parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, null, sqlQuery, parameters);
            }

        }

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        /// <remarks>Used to fetch general company data.</remarks>
        public override async Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields)
        {
            
            if (table == null || string.IsNullOrEmpty(table.Trim())) throw new ArgumentNullException(nameof(table));
            if (fields == null || fields.Length < 1) throw new ArgumentNullException(nameof(fields));

            var preparedFields = new List<string>();
            foreach (var field in fields)
            {
                if (field == null || string.IsNullOrEmpty(field.Trim())) 
                    throw new ArgumentException("Fields cannot be empty.", "fields");
                preparedFields.Add(field.Trim().ToLower());
            }

            return await FetchTableRawAsync(string.Format("SELECT {0} FROM {1};", 
                string.Join(", ", preparedFields.ToArray()), table.Trim().ToLower()), null);

        }

        /// <summary>
        /// Executes an SQL statement, that inserts a new row, using SQL query token 
        /// in the SQL repository and returns last insert id.
        /// </summary>
        /// <param name="insertStatementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public override async Task<long> ExecuteInsertAsync(string insertStatementToken, SqlParam[] parameters)
        {

            if (insertStatementToken == null || string.IsNullOrEmpty(insertStatementToken.Trim())) 
                throw new ArgumentNullException(nameof(insertStatementToken));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, CurrentTransaction, 
                    GetSqlQuery(insertStatementToken), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<long>(conn, null, GetSqlQuery(insertStatementToken), parameters);
            }

        }

        /// <summary>
        /// Executes a raw SQL statement, that inserts a new row, and returns last insert id.
        /// </summary>
        /// <param name="insertStatement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        public override async Task<long> ExecuteInsertRawAsync(string insertStatement, SqlParam[] parameters)
        {

            if (insertStatement == null || string.IsNullOrEmpty(insertStatement.Trim()))
                throw new ArgumentNullException(nameof(insertStatement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, CurrentTransaction, insertStatement, parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<long>(conn, null, insertStatement, parameters);

            }

        }

        /// <summary>
        /// Executes an SQL statement using SQL query token in the SQL repository 
        /// and returns affected rows count.
        /// </summary>
        /// <param name="statementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public override async Task<int> ExecuteCommandAsync(string statementToken, SqlParam[] parameters)
        {

            if (statementToken == null || string.IsNullOrEmpty(statementToken.Trim())) throw new ArgumentNullException(nameof(statementToken));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, CurrentTransaction, GetSqlQuery(statementToken), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<int>(conn, null, GetSqlQuery(statementToken), parameters);
            }

        }

        /// <summary>
        /// Executes a raw SQL statement and returns affected rows count.
        /// </summary>
        /// <param name="statement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters 
        /// (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        public override async Task<int> ExecuteCommandRawAsync(string statement, SqlParam[] parameters)
        {

            if (statement == null || string.IsNullOrEmpty(statement.Trim())) throw new ArgumentNullException(nameof(statement));

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, CurrentTransaction, statement, parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<int>(conn, null, statement, parameters);
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
            
            if (statements == null || statements.Length < 1)
                throw new ArgumentNullException(nameof(statements));

            if (this.IsTransactionInProgress)
                throw new InvalidOperationException("Cannot execute batch while a transaction is in progress.");

            using (var conn = await OpenConnectionAsync())
            {
                try
                {
                    using (var command = new MySqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandTimeout = QueryTimeOut;

                        foreach (var statement in statements)
                        {
                            if (statement != null && !string.IsNullOrEmpty(statement.Trim()))
                            {
                                command.CommandText = statement;
                                await command.ExecuteNonQueryAsync();
                            }
                        }

                    }
                }
                finally
                {
                    try
                    {
                        if (conn != null && conn.State != ConnectionState.Closed)
                            await conn.CloseAsync();
                    }
                    catch (Exception) { }
                }

            }

        }

        /// <summary>
        /// Checks if the database specified exists.
        /// </summary>
        /// <param name="databaseName">a name of the database to check</param>
        /// <returns>True if the database specified exists.</returns>
        public override async Task<bool> DatabaseExistsAsync(string databaseName)
        {
            using (var conn = await OpenConnectionAsync())
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, null,
                    string.Format("SHOW DATABASES LIKE '{0}';", databaseName), null);
                return (table.Rows.Count > 0);
            }                   
        }

        /// <summary>
        /// Checks if the database is empty, i.e. contains no tables.
        /// </summary>
        /// <param name="databaseName">a name of the database to check</param>
        /// <returns>True if the database contains any tables.</returns>
        public override async Task<bool> DatabaseEmptyAsync(string databaseName)
        {
            using (var conn = await OpenConnectionAsync())
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, null,
                    string.Format("SHOW TABLES FROM `{0}`;", databaseName), null);
                return (table.Rows.Count < 1);
            }                   
        }


        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary>
        public override async Task<DbSchema> GetDbSchemaAsync()
        {

            if (_currentDatabase == null || string.IsNullOrEmpty(_currentDatabase))
                throw new InvalidOperationException("Database is not set, cannot get schema.");
            if (IsTransactionInProgress)
                throw new InvalidOperationException("Cannot get schema while a transaction is in progress.");

            var result = new DbSchema();

            var conn = await OpenConnectionAsync();

            try
            {
                var dbData = await FetchUsingConnectionAsync(conn, "SELECT @@character_set_database, @@default_storage_engine;");

                if (dbData.Rows.Count > 0)
                {
                    result.CharsetName = dbData.Rows[0].GetString(0);
                    result.Description = string.Format("DB schema for the mysql database {0} using engine {1}.",
                        _currentDatabase, dbData.Rows[0].GetString(1));
                }

                var indexDictionary = await GetIndexesAsync(conn);
                var fkDictionary = await GetForeignKeysAsync(conn);

                result.Tables = new List<DbTableSchema>();

                var tablesData = await FetchUsingConnectionAsync(conn, "SHOW TABLE STATUS;");
                foreach (var row in tablesData.Rows)
                {
                    result.Tables.Add(await GetDbTableSchemaAsync(conn, row, indexDictionary, fkDictionary));
                }

                conn.Close();

                conn.Dispose();

            }
            catch (Exception)
            {
                if (conn != null)
                {
                    try
                    {
                        if (conn.State != ConnectionState.Closed) await conn.CloseAsync();
                    }
                    catch (Exception) { }
                    try
                    {
                        conn.Dispose();
                    }
                    catch (Exception){}
                }
                throw;
            }
            
            return result;

        }

        /// <summary>
        /// Compares the actualSchema definition to the gaugeSchema definition and returns
        /// a list of DbSchema errors, i.e. inconsistencies found and SQL statements to repair them.
        /// </summary>
        /// <param name="gaugeSchema">the gauge schema definition to compare the actualSchema against</param>
        /// <param name="actualSchema">the schema to check for inconsistencies (and repair)</param>
        public override List<DbSchemaError> GetDbSchemaErrors(DbSchema gaugeSchema, DbSchema actualSchema)
        {
            
            if (gaugeSchema == null) throw new ArgumentNullException(nameof(gaugeSchema));
            if (actualSchema == null) throw new ArgumentNullException(nameof(actualSchema));
            if (IsTransactionInProgress) throw new InvalidOperationException("Cannot get schema errors while a transaction is in progress.");
            
            var result = new List<DbSchemaError>();

            foreach (var gaugeTable in gaugeSchema.Tables)
            {

                var gaugeTableFound = false;

                foreach (var actualTable in actualSchema.Tables)
                {
                    if (actualTable.Name.Trim().ToUpperInvariant() == gaugeTable.Name.Trim().ToUpperInvariant())
                    {
                        result.AddRange(GetDbTableSchemaErrors(gaugeTable, actualTable));
                        gaugeTableFound = true;
                        break;
                    }
                }

                if (!gaugeTableFound)
                {
                    var applicableEngine = _engine;
                    if (string.IsNullOrEmpty(applicableEngine.Trim())) applicableEngine = DefaultEngine;
                    var applicableCharset = _charset;
                    if (string.IsNullOrEmpty(applicableCharset.Trim())) applicableCharset = DefaultCharset;
                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableMissing, string.Format("Table {0} missing.", 
                        gaugeTable.Name), gaugeTable.Name, gaugeTable.GetCreateTableStatements(
                        _currentDatabase, applicableEngine, applicableCharset).ToArray()));
                }

            }

            foreach (var actualTable in actualSchema.Tables)
            {

                var actualTableFound = gaugeSchema.Tables.Any(gaugeTable => actualTable.Name.Trim().ToUpperInvariant() 
                    == gaugeTable.Name.Trim().ToUpperInvariant());

                if (!actualTableFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableObsolete, string.Format("Table {0} is obsolete.",
                        actualTable.Name), actualTable.Name, actualTable.GetDropTableStatements(_currentDatabase).ToArray()));
                }

            }

            return result;

        }

        /// <summary>
        /// Gets an SQL script to create a database for the dbSchema specified.
        /// </summary>
        /// <param name="dbSchema">the database schema to get the create database script for</param>
        public override string GetCreateDatabaseSql(DbSchema dbSchema)
        {
            
            var createScript = new List<string>
                {
                    "CREATE DATABASE DoomyDatabaseName CHARACTER SET utf8;",
                    "USE DoomyDatabaseName;"
                };

            foreach (var table in dbSchema.GetTablesInCreateOrder())
            {
                createScript.AddRange(table.GetCreateTableStatements("DoomyDatabaseName", 
                    DefaultEngine, DefaultCharset));
            }

            return string.Join(Environment.NewLine, createScript.ToArray());

        }

        private List<DbSchemaError> GetDbTableSchemaErrors(DbTableSchema gaugeSchema, DbTableSchema actualSchema)
        {

            var result = new List<DbSchemaError>();

            foreach (var gaugeField in gaugeSchema.Fields)
            {

                var gaugeFieldFound = false;

                foreach (var actualField in actualSchema.Fields)
                {
                    if (gaugeField.Name.Trim().ToUpperInvariant() == actualField.Name.Trim().ToUpperInvariant())
                    {

                        var schemasMatch = gaugeField.FieldSchemaMatch(actualField);
                        var indexMatch = gaugeField.FieldIndexMatch(actualField);
                        var statements = new List<string>();
                        var inconsistencyType = DbSchemaErrorType.FieldDefinitionObsolete;
                        var description = string.Empty;

                        if (!schemasMatch && !indexMatch)
                        {
                            statements.AddRange(actualField.GetDropIndexStatements(_currentDatabase, actualSchema.Name));
                            statements.AddRange(gaugeField.GetAlterFieldStatements(_currentDatabase, actualSchema.Name));
                            statements.AddRange(gaugeField.GetAddIndexStatements(_currentDatabase, actualSchema.Name));
                            description = string.Format("Table {0} field {1} schema and index definitions are obsolete.",
                                gaugeSchema.Name, actualField.Name);
                        }
                        else
                        {
                            if (!schemasMatch)
                            {
                                statements.AddRange(gaugeField.GetAlterFieldStatements(_currentDatabase, actualSchema.Name));
                                description = string.Format("Table {0} field {1} schema definition is obsolete.",
                                    gaugeSchema.Name, actualField.Name);
                            }
                            if (!indexMatch)
                            {
                                statements.AddRange(actualField.GetDropIndexStatements(_currentDatabase, actualSchema.Name));
                                statements.AddRange(gaugeField.GetAddIndexStatements(_currentDatabase, actualSchema.Name));
                                inconsistencyType=DbSchemaErrorType.IndexObsolete;
                                description = string.Format("Table {0} field {1} index definition is obsolete.",
                                    gaugeSchema.Name, actualField.Name);
                            }
                        }

                        if (!indexMatch || !schemasMatch)
                        {
                            result.Add(GetDbSchemaError(inconsistencyType, description, gaugeSchema.Name,
                                gaugeField.Name, statements.ToArray()));
                        }

                        gaugeFieldFound = true;
                        break;

                    }
                }

                if (!gaugeFieldFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.FieldMissing, string.Format("Field {0} in table {1} missing.",
                        gaugeField.Name, gaugeSchema.Name), gaugeSchema.Name, gaugeField.Name,
                        gaugeField.GetAddFieldStatements(_currentDatabase, gaugeSchema.Name).ToArray()));
                }

            }

            foreach (var actualField in actualSchema.Fields)
            {

                var actualFieldFound = gaugeSchema.Fields.Any(gaugeField => actualField.Name.Trim().ToLower()
                    == gaugeField.Name.Trim().ToLower());

                if (!actualFieldFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.FieldObsolete, string.Format("Field {0} in table {1} is obsolete.",
                        actualField.Name, actualSchema.Name), actualSchema.Name, actualField.Name,
                        actualField.GetDropFieldStatements(_currentDatabase, actualSchema.Name).ToArray()));
                }

            }

            return result;

        }


        /// <summary>
        /// A method that should do the actual new database creation.
        /// </summary>
        /// <param name="databaseName">a name of the new database to create</param>
        /// <param name="dbSchema">a DbSchema to use for the new database</param>
        /// <remarks>After creating a new database the <see cref="SqlAgentBase.CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        protected override async Task CreateDatabaseAsync(string databaseName, DbSchema dbSchema)
        {

            var applicableEngine = _engine;
            if (string.IsNullOrEmpty(applicableEngine.Trim())) applicableEngine = DefaultEngine;
            var applicableCharset = _charset;
            if (string.IsNullOrEmpty(applicableCharset.Trim())) applicableCharset = DefaultCharset;

            var createScript = new List<string>
                {
                    string.Format("CREATE DATABASE {0} CHARACTER SET {1};", databaseName, applicableCharset),
                    string.Format("USE {0};", databaseName)
                };

            foreach (var table in dbSchema.GetTablesInCreateOrder())
            {
                createScript.AddRange(table.GetCreateTableStatements(databaseName, 
                    applicableEngine, applicableCharset));
            }

            await this.ExecuteCommandBatchAsync(createScript.ToArray());

            _currentDatabase = databaseName;

        }

        /// <summary>
        /// Drops (deletes) the database specified.
        /// </summary>
        /// <param name="databaseName">the name of the database to drop</param>
        public override async Task DropDatabaseAsync(string databaseName)
        {
            if (databaseName == null || string.IsNullOrEmpty(databaseName.Trim())) 
                throw new ArgumentNullException(nameof(databaseName));
            if (IsTransactionInProgress)
                throw new InvalidOperationException("Cannot drop database while transaction is in progress.");
            await ExecuteCommandRawAsync(string.Format("DROP DATABASE {0};", databaseName), null);
        }


        /// <summary>
        /// Copies table data from the current SqlAgent instance to the target SqlAgent instance.
        /// </summary>
        /// <param name="schema">a schema of the database to copy the data</param>
        /// <param name="targetSqlAgent">the target SqlAgent to copy the data to</param>
        /// <param name="worker">BackgroundWorker to report the progress to (if used)</param>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.
        /// Basicaly iterates tables, selects data, creates an IDataReader for the table and passes it to the 
        /// <see cref="InsertTableData">InsertTableData</see> method of the target SqlAgent.</remarks>
        protected override void CopyData(DbSchema schema, SqlAgentBase targetSqlAgent,
            System.ComponentModel.BackgroundWorker worker)
        {

            using (var conn = OpenConnectionAsync().Result)
            {
                try
                {
                    using (var command = new MySqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandTimeout = QueryTimeOut;

                        int percentsPerTable = (schema.Tables.Count / 90);

                        foreach (var table in schema.Tables)
                        {

                            if (worker != null)
                                worker.ReportProgress((10 + (schema.Tables.IndexOf(table) * percentsPerTable)),
                                    "Cloning table {0} data...");

                            var fields = string.Join(", ", table.Fields.Select(
                                field => field.Name.Trim().ToLower()).ToArray());
                            
                            command.CommandText = string.Format("SELECT {0} FROM {1};",
                                fields, table.Name.Trim().ToLower());

                            using (IDataReader reader = command.ExecuteReader())
                            {
                                CallInsertTableDataAsync(targetSqlAgent, table, reader).Wait();
                            }

                            if (worker != null && worker.CancellationPending)
                            {
                                worker.ReportProgress(100, "Clone has been canceled by the user. WARNING. The target database has already been creted.");
                                throw new Exception("Clone has been canceled by the user. WARNING. The target database has already been creted.");
                            }

                        }

                    }
                }
                finally
                {
                    try
                    {
                        if (conn != null && conn.State != ConnectionState.Closed)
                            conn.Close();
                    }
                    catch (Exception) { }
                }

            }

        }

        /// <summary>
        /// Disables foreign key checks for the current transaction.
        /// </summary>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.</remarks>
        protected override async Task DisableForeignKeysForCurrentTransactionAsync()
        {
            await ExecuteCommandIntAsync<int>(null, CurrentTransaction, "SET FOREIGN_KEY_CHECKS = 0;", null);
        }

        /// <summary>
        /// Inserts table data from the reader to the current SqlAgent instance,
        /// </summary>
        /// <param name="table">a schema of the table to insert the data to</param>
        /// <param name="reader">an IDataReader to read the table data from.</param>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.
        /// The insert is performed using a transaction that is already initiated by the 
        /// <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see>.</remarks>
        protected override async Task InsertTableDataAsync(DbTableSchema table, IDataReader reader)
        {

            var fields = table.Fields.Select(field => field.Name.Trim().ToLower()).ToList();
            
            var paramPrefixedNames = new List<string>();
            var paramNames = new List<string>();
            for (int i = 0; i < fields.Count; i++)
            {
                var paramName = GetParameterName(i);
                paramNames.Add(paramName);
                paramPrefixedNames.Add(ParamPrefix + paramName);
            }

            var insertStatement = string.Format("INSERT INTO {0}({1}) VALUES({2});",
                table.Name.ToLower(), string.Join(", ", fields.ToArray()),
                string.Join(", ", paramPrefixedNames.ToArray()));

            while (reader.Read())
            {
                var paramValues = new List<SqlParam>();
                for (int i = 0; i < fields.Count; i++)
                {
                    paramValues.Add(new SqlParam(paramNames[i], reader.GetValue(i)));
                }
                await this.ExecuteCommandIntAsync<int>(null, CurrentTransaction, insertStatement, paramValues.ToArray());
            }

        }



        private async Task<MySqlConnection> OpenConnectionAsync()
        {

            MySqlConnection result;
            if (_currentDatabase == null || string.IsNullOrEmpty(_currentDatabase.Trim()))
            {
                result = new MySqlConnection(_baseConnectionString);
            }
            else
            {
                if (_baseConnectionString.Trim().EndsWith(";"))
                {
                    result = new MySqlConnection(_baseConnectionString + "Database=" + _currentDatabase.Trim() + ";");
                }
                else
                {
                    result = new MySqlConnection(_baseConnectionString + ";Database=" + _currentDatabase.Trim() + ";");
                }
            }

            try
            {
                await result.OpenAsync();
            }
            catch (Exception ex)
            {
                await HandleOpenConnectionException(result, ex);
            }

            return result;

        }

        private async Task HandleOpenConnectionException(MySqlConnection conn, Exception ex)
        {

            if (conn != null && conn.State != ConnectionState.Closed)
            {
                try
                {
                    await conn.CloseAsync();
                }
                catch (Exception){}
            }

            if (conn != null)
            {
                try
                {
                    conn.Dispose();
                }
                catch (Exception){}
            }

            var mySqlEx = ex as MySqlException;
            if (mySqlEx != null)
            {

                if (mySqlEx.Number == 28000 ||
                    mySqlEx.Number == 42000)
                {
                    throw new SqlException("Access denied.",mySqlEx.Number, mySqlEx);
                }

                if (mySqlEx.Number == 2003)
                {
                    throw new SqlException("Unable to connect to the MySql server. Server is down, host address is invalid or connection is blocked by a firewall.",
                        mySqlEx.Number, mySqlEx);
                }

                throw new SqlException(string.Format("MySql server returned error: {0}", mySqlEx.Message), 
                    mySqlEx.Number, mySqlEx);

            }

            throw new Exception(string.Format("Unknown exception occured while opening connection to the MySql server: {0}", 
                ex.Message), ex);

        }

        private void CleanUpTransaction()
        {

            if (CurrentTransaction == null) return;

            if (CurrentTransaction.Connection != null)
            {

                if (CurrentTransaction.Connection.State == ConnectionState.Open)
                {
                    try
                    {
                        CurrentTransaction.Connection.Close();
                    }
                    catch (Exception){}
                }

                try
                {
                    CurrentTransaction.Connection.Dispose();
                }
                catch (Exception) { }

            }

            try
            {
                CurrentTransaction.Dispose();
            }
            catch (Exception) { }

            CurrentTransaction = null;

        }


        private void AddParams(MySqlCommand command, SqlParam[] parameters)
        {

            command.Parameters.Clear();

            if (parameters == null || parameters.Length < 1) return;

            foreach (var parameter in parameters)
            {
                if (!parameter.ReplaceInQuery)
                {
                    var value = parameter.Value;
                    if (parameter.Value != null && BooleanStoredAsTinyInt && parameter.Value.GetType() == typeof(bool))
                    {
                        if ((bool)value) value = 1;
                        else value = 0;
                    }
                    command.Parameters.Add(new MySqlParameter
                    {
                        ParameterName = ParamPrefix + parameter.Name.Trim(),
                        DbType = GetNativeDbType(parameter),                    
                        Value = value,
                    });

                }
            }

        }

        private DbType GetNativeDbType(SqlParam parameter)
        {

            Type valueType;

            if (parameter.Value == null)
            {
                if (parameter.ValueType == null)
                    return DbType.String;
                valueType = parameter.ValueType;
            }
            else
            {
                valueType = parameter.Value.GetType();
            }

            if (valueType == typeof(Byte))
                return DbType.Byte;
            if (valueType == typeof(Int16))
                return DbType.Int16;
            if (valueType == typeof(Int32))
                return DbType.Int32;
            if (valueType == typeof(Int64))
                return DbType.Int64;
            if (valueType == typeof(UInt16))
                return DbType.UInt16;
            if (valueType == typeof(UInt32))
                return DbType.UInt32;
            if (valueType == typeof(UInt64))
                return DbType.UInt64;
            if (valueType == typeof(Byte[]))
                return DbType.Binary;
            if (valueType == typeof(SByte))
                return DbType.SByte;
            if (valueType == typeof(Boolean))
                return DbType.Boolean;
            if (valueType == typeof(DateTime))
                return DbType.DateTime;
            if (valueType == typeof(Decimal))
                return DbType.Decimal;
            if (valueType == typeof(Single))
                return DbType.Single;
            if (valueType == typeof(Double))
                return DbType.Double;
            if (valueType == typeof(Guid))
                return DbType.Guid;
            if (valueType == typeof(DateTimeOffset))
                return DbType.DateTimeOffset;
            
            return DbType.String;

        }

        private string ReplaceParams(string sqlQuery, SqlParam[] parameters)
        {

            if (parameters == null || parameters.Length < 1) return sqlQuery;

            var result = sqlQuery;

            foreach (var parameter in parameters.Where(parameter => parameter.ReplaceInQuery))
            {
                if (parameter.Value == null)
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

        private async Task<T> ExecuteCommandIntAsync<T>(MySqlConnection connection, MySqlTransaction transaction,
            string sqlStatement, SqlParam[] parameters)
        {

            try
            {
                using (var command = new MySqlCommand())
                {

                    if (transaction == null)
                    {
                        command.Connection = connection;
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
                        var reader = await command.ExecuteReaderAsync();
                        return (T)(Object)(new LightDataTable(reader));
                    }
                    else if (typeof(T) == typeof(long))
                    {
                        await command.ExecuteNonQueryAsync();
                        return (T)(Object)command.LastInsertedId;
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(Object)(await command.ExecuteNonQueryAsync());
                    }
                    else throw new NotSupportedException(string.Format("Generic parameter type {0} is not supported by MySqlAgent.ExecuteCommandInt.",
                        typeof (T).FullName));

                }
            }
            catch (Exception ex)
            {

                if (transaction != null)
                {

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            string.Format(
                                "Critical SQL transaction error, failed to rollback the transaction.{0}{1}{2}{3}",
                                Environment.NewLine, ex.Message, Environment.NewLine, e.Message), ex);
                    }
                    finally
                    {
                        CleanUpTransaction();
                    }

                }

                throw;
            }
            finally
            {
                if (transaction == null)
                {
                    try
                    {
                        if (connection != null && connection.State != ConnectionState.Closed)
                            await connection.CloseAsync();
                    }
                    catch (Exception) { }
                }
            }

        }

        private async Task<LightDataTable> FetchUsingConnectionAsync(MySqlConnection connection, string sqlStatement)
        {

            using (var command = new MySqlCommand())
            {

                command.Connection = connection;
                command.CommandTimeout = QueryTimeOut;
                command.CommandText = sqlStatement;

                using (var reader = await command.ExecuteReaderAsync())
                {
                    return new LightDataTable(reader);
                }

            }

        }


        private async Task<DbTableSchema> GetDbTableSchemaAsync(MySqlConnection conn, LightDataRow tableStatusRow,
            Dictionary<string, Dictionary<string, string>> indexDictionary,
            Dictionary<string, Dictionary<string, ForeignKeyData>> fkDictionary)
        {

            var result = new DbTableSchema
            {
                Name = tableStatusRow.GetString(0),
                EngineName = tableStatusRow.GetString(1),
                CharsetName = tableStatusRow.GetString(14),
                Description = tableStatusRow.GetString(17),
                Fields = new List<DbFieldSchema>()
            };

            Dictionary<string, string> tableIndexDictionary = null;
            Dictionary<string, ForeignKeyData> tableFkDictionary = null;
            if (indexDictionary.ContainsKey(result.Name.Trim().ToLower()))
                tableIndexDictionary = indexDictionary[result.Name.Trim().ToLower()];
            if (fkDictionary.ContainsKey(result.Name.Trim().ToLower()))
                tableFkDictionary = fkDictionary[result.Name.Trim().ToLower()];


            var fieldsData = await FetchUsingConnectionAsync(conn,  
                string.Format("SHOW FULL COLUMNS FROM {0};", result.Name));

            foreach (var row in fieldsData.Rows)
            {
                result.Fields.Add(this.GetDbFieldSchema(row, tableIndexDictionary, tableFkDictionary));
            }

            return result;

        }

        private DbFieldSchema GetDbFieldSchema(LightDataRow fieldStatusRow,
            Dictionary<string, string> indexDictionary, Dictionary<string, ForeignKeyData> fkIndex)
        {

            var result = new DbFieldSchema
            {
                Name = fieldStatusRow.GetString(0),
                NotNull = (fieldStatusRow.GetString(3).Trim().ToLower() == "no"),
                Unsigned = (fieldStatusRow.GetString(1).Trim().ToLower().Contains("unsigned")),
                Autoincrement = (fieldStatusRow.GetString(6).Trim().ToLower().Contains("auto_increment")),
                Description = fieldStatusRow.GetString(8)
            };

            var rawType = fieldStatusRow.GetString(1).Trim();

            result.DataType = GetFieldType(rawType.ToLower());

            var typeDetails = "";
            if (rawType.Contains("("))
                typeDetails = rawType.Substring(rawType.IndexOf("(", StringComparison.Ordinal) + 1,
                    rawType.IndexOf(")", StringComparison.Ordinal)
                    - rawType.IndexOf("(", StringComparison.Ordinal) - 1);

            if (result.DataType == DbDataType.Char || result.DataType == DbDataType.VarChar)
            {
                if (int.TryParse(typeDetails.Trim().Replace("`", "").Replace("'", "").Replace("\"", ""), out int length))
                    result.Length = length;
            }
            if (result.DataType == DbDataType.Decimal)
            {
                var intValues = typeDetails.Trim().Replace("`", "").Replace("'", "").Replace("\"", "").Split(
                    new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (intValues.Length > 1)
                {
                    if (int.TryParse(intValues[1].Trim(), out int length))
                        result.Length = length;
                }
            }
            if (result.DataType == DbDataType.Enum)
            {
                result.EnumValues = typeDetails.Trim().Replace("`", "").Replace("'", "").Replace("\"", "");
            }

            if (fieldStatusRow.GetString(4).Trim().ToLower() == "pri")
            {
                result.IndexType = DbIndexType.Primary;
            }
            else
            {
                if (fkIndex != null && fkIndex.ContainsKey(result.Name.Trim().ToLower()))
                {
                    result.IndexType = DbIndexType.ForeignKey;
                    result.IndexName = fkIndex[result.Name.Trim().ToLower()].Name;
                    result.OnUpdateForeignKey = fkIndex[result.Name.Trim().ToLower()].OnUpdate;
                    result.OnDeleteForeignKey = fkIndex[result.Name.Trim().ToLower()].OnDelete;
                    result.RefTable = fkIndex[result.Name.Trim().ToLower()].RefTable;
                    result.RefField = fkIndex[result.Name.Trim().ToLower()].RefField;
                }
                else
                {
                    if (indexDictionary != null && indexDictionary.ContainsKey(result.Name.Trim().ToLower()))
                    {
                        result.IndexType = DbIndexType.Simple;
                        if (fieldStatusRow.GetString(4).Trim().ToLower() == "uni")
                            result.IndexType = DbIndexType.Unique;
                        result.IndexName = indexDictionary[result.Name.Trim().ToLower()];
                    }
                }
            }

            return result;

        }

        private static DbDataType GetFieldType(string definition)
        {

            var nativeName = definition;
            if (nativeName.Contains("("))
                nativeName = nativeName.Substring(0, nativeName.IndexOf("(", StringComparison.Ordinal));
            if (nativeName.Contains(" "))
                nativeName = nativeName.Substring(0, nativeName.IndexOf(" ", StringComparison.Ordinal));
            nativeName = nativeName.Trim().ToLower();

            switch (nativeName)
            {
                case "blob":
                    return DbDataType.Blob;
                case "longblob":
                    return DbDataType.BlobLong;
                case "mediumblob":
                    return DbDataType.BlobMedium;
                case "tinyblob":
                    return DbDataType.BlobTiny;
                case "char":
                    return DbDataType.Char;
                case "date":
                    return DbDataType.Date;
                case "datetime":
                    return DbDataType.DateTime;
                case "decimal":
                    return DbDataType.Decimal;
                case "double":
                    return DbDataType.Double;
                case "enum":
                    return DbDataType.Enum;
                case "float":
                    return DbDataType.Float;
                case "int":
                    return DbDataType.Integer;
                case "integer":
                    return DbDataType.Integer;
                case "bigint":
                    return DbDataType.IntegerBig;
                case "mediumint":
                    return DbDataType.IntegerMedium;
                case "smallint":
                    return DbDataType.IntegerSmall;
                case "tinyint":
                    return DbDataType.IntegerTiny;
                case "real":
                    return DbDataType.Real;
                case "text":
                    return DbDataType.Text;
                case "longtext":
                    return DbDataType.TextLong;
                case "mediumtext":
                    return DbDataType.TextMedium;
                case "time":
                    return DbDataType.Time;
                case "timestamp":
                    return DbDataType.TimeStamp;
                case "varchar":
                    return DbDataType.VarChar;
                default:
                    throw new NotImplementedException(string.Format(
                        "MySql database data type {0} is unknown.", definition));
            }
        }

        private async Task<Dictionary<string, Dictionary<string, string>>> GetIndexesAsync(MySqlConnection conn)
        {

            var result = new Dictionary<string, Dictionary<string, string>>();

            var indexTable = await FetchUsingConnectionAsync(conn, @"
                SELECT s.TABLE_NAME, s.COLUMN_NAME, s.INDEX_NAME, s.NON_UNIQUE
                FROM INFORMATION_SCHEMA.STATISTICS s
                LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE c ON c.TABLE_SCHEMA = s.TABLE_SCHEMA
                AND c.TABLE_NAME = s.TABLE_NAME AND c.COLUMN_NAME = s.COLUMN_NAME
                WHERE s.INDEX_NAME <> 'PRIMARY' AND c.CONSTRAINT_NAME IS NULL AND s.TABLE_SCHEMA = DATABASE();");

            foreach (var row in indexTable.Rows)
            {
                var current = GetOrCreateTableDictionary(result, row.GetString(0).Trim().ToLower());
                if (!current.ContainsKey(row.GetString(1).Trim().ToLower()))
                    current.Add(row.GetString(1).Trim().ToLower(), row.GetString(2).Trim().ToLower());
            }

            return result;

        }

        private Dictionary<string, string> GetOrCreateTableDictionary(
            Dictionary<string, Dictionary<string, string>> baseDictionary, string tableName)
        {
            if (baseDictionary.ContainsKey(tableName))
                return baseDictionary[tableName];
            var result = new Dictionary<string, string>();
            baseDictionary.Add(tableName, result);
            return result;
        }

        private async Task<Dictionary<string, Dictionary<string, ForeignKeyData>>> GetForeignKeysAsync(MySqlConnection conn)
        {

            var result = new Dictionary<string, Dictionary<string, ForeignKeyData>>();

            var indexTable = await FetchUsingConnectionAsync(conn, @"
                SELECT i.TABLE_NAME, k.COLUMN_NAME, i.CONSTRAINT_NAME, k.REFERENCED_TABLE_NAME, k.REFERENCED_COLUMN_NAME 
                FROM information_schema.TABLE_CONSTRAINTS i 
                LEFT JOIN information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME 
                WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY' AND i.TABLE_SCHEMA = DATABASE();");

            foreach (var row in indexTable.Rows)
            {
                var current = GetOrCreateFKTableDictionary(result, row.GetString(0).Trim().ToLower());
                if (!current.ContainsKey(row.GetString(1).Trim().ToLower()))
                {
                    var fkInfo = new ForeignKeyData
                    {
                        Name = row.GetString(2).Trim().ToLower(),
                        RefTable = row.GetString(3).Trim().ToLower(),
                        RefField = row.GetString(4).Trim().ToLower()
                    };
                    current.Add(row.GetString(1).Trim().ToLower(), fkInfo);
                }
            }

            foreach (var entry in result)
            {

                var showCreateTable = await FetchUsingConnectionAsync(conn, string.Format("SHOW CREATE TABLE {0};", entry.Key));
                var showCreateLines = showCreateTable.Rows[0].GetString(1).Split(new string[] { "," },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in showCreateLines)
                {
                    if (line.Trim().ToUpper().StartsWith("CONSTRAINT"))
                    {
                        foreach (var column in entry.Value)
                        {
                            if (line.Trim().ToLower().Contains("`" + column.Value.Name + "`") 
                                || line.Trim().ToLower().Contains(" " + column.Value.Name + " "))
                            {
                                column.Value.OnUpdate = DbForeignKeyActionType.Restrict;
                                if (line.Trim().ToUpper().Contains("ON UPDATE CASCADE"))
                                    column.Value.OnUpdate = DbForeignKeyActionType.Cascade;
                                if (line.Trim().ToUpper().Contains("ON UPDATE SET NULL"))
                                    column.Value.OnUpdate = DbForeignKeyActionType.SetNull;

                                column.Value.OnDelete = DbForeignKeyActionType.Restrict;
                                if (line.Trim().ToUpper().Contains("ON DELETE CASCADE"))
                                    column.Value.OnDelete = DbForeignKeyActionType.Cascade;
                                if (line.Trim().ToUpper().Contains("ON DELETE SET NULL"))
                                    column.Value.OnDelete = DbForeignKeyActionType.SetNull;

                                break;

                            }
                        }
                    }
                }


            }

            return result;

        }

        private Dictionary<string, ForeignKeyData> GetOrCreateFKTableDictionary(
            Dictionary<string, Dictionary<string, ForeignKeyData>> baseDictionary, string tableName)
        {
            if (baseDictionary.ContainsKey(tableName))
                return baseDictionary[tableName];
            var result = new Dictionary<string, ForeignKeyData>();
            baseDictionary.Add(tableName, result);
            return result;
        }
                

        private class ForeignKeyData
        {
            public string Name { get; set; }
            public string RefTable { get; set; }
            public string RefField { get; set; }
            public DbForeignKeyActionType OnUpdate { get; set; }
            public DbForeignKeyActionType OnDelete { get; set; } 
        }

    }
}
