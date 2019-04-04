using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Apskaita5.DAL.Common;
using System.Threading.Tasks;
using System.Threading;

namespace Apskaita5.DAL.Sqlite
{
    public class SqliteAgent : SqlAgentBase
    {

        private const string AgentName = "SQLite connector";
        private const bool AgentIsFileBased = true;
        private const string AgentRootName = "";
        private const string AgentSqlRepositoryFileNamePrefix = "sqlite_";
        private const string AgentWildcart = "%";
        private const string ParamPrefix = "$";
        
        private static AsyncLocal<SQLiteTransaction> asyncTransaction = new AsyncLocal<SQLiteTransaction>();
        private SQLiteTransaction instanceTransaction = null;


        /// <summary>
        /// Gets a name of the SQL implementation behind the SqlAgent, i. e. SQLite connector.
        /// </summary>
        public override string Name
        {
            get { return AgentName; }
        }

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based, i.e. true.
        /// </summary>
        public override bool IsFileBased
        {
            get { return AgentIsFileBased; }
        }

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent, i.e. none.
        /// </summary>
        public override string RootName
        {
            get { return AgentRootName; }
        }

        /// <summary>
        /// Gets a prefix of the names of the files that contain SQL queries written 
        /// for the SQL implementation behind the SqlAgent, i.e. sqlite_.
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
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        public override bool IsTransactionInProgress
        {
            get { return CurrentTransaction != null; }
        }


        /// <summary>
        /// Initializes a new SqliteAgent instance.
        /// </summary>
        /// <param name="baseConnectionString">a connection string to use to connect to
        /// a database (should not include database parameter that is added by the
        /// SqlAgent implementation depending on the database chosen, should include password
        /// (if any), password replacement (if needed) should be handled by the user class)</param>
        /// <param name="sqlRepositoryPath">a path to the folder with the the files that contain SQL repositories</param>
        /// <param name="sqlTokensUsed">whether the user class is going to use SQL query tokens
        /// i.e. sqlRepositoryPath is required</param>
        public SqliteAgent(string baseConnectionString, string sqlRepositoryPath, bool sqlTokensUsed)
            : base(baseConnectionString, true, sqlRepositoryPath, sqlTokensUsed)
        {
        }


        /// <summary>
        /// Gets a clean copy (i.e. only connection data, not connection itself) of the SqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        protected override SqlAgentBase GetCopyInt()
        {
            var sqlTokensUsed = (this.SqlRepositoryPath != null && !string.IsNullOrEmpty(this.SqlRepositoryPath.Trim()));
            return new SqliteAgent(_baseConnectionString, this.SqlRepositoryPath, sqlTokensUsed);
        }

        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        public override async Task TestConnectionAsync()
        {
            using (var result = await OpenConnectionAsync())
            {
                result.Close();    
            }
        }

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        /// <exception cref="InvalidOperationException">if transaction is already in progress</exception>
        protected override async Task TransactionBeginAsync()
        {
            var result = await OpenConnectionAsync();
            CurrentTransaction = result.BeginTransaction();
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
                    throw new Exception(string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}{1}{2}{3}",
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
                    throw new Exception(string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}{1}{2}{3}",
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

            if (token == null || string.IsNullOrEmpty(token.Trim())) throw new ArgumentNullException("token");

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

            if (sqlQuery == null || string.IsNullOrEmpty(sqlQuery.Trim())) 
                throw new ArgumentNullException("sqlQuery");

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<LightDataTable>(null, CurrentTransaction, 
                    ReplaceParamsInRawQuery(sqlQuery, parameters), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<LightDataTable>(conn, null, 
                    ReplaceParamsInRawQuery(sqlQuery, parameters), parameters);
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
        /// <exception cref="ArgumentNullException">Parameters table or fields are not specified.</exception>
        /// <exception cref="ArgumentException">Fields (every field in the fields array) cannot be empty.</exception>
        public override async Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields)
        {
            
            if (table == null || string.IsNullOrEmpty(table.Trim())) throw new ArgumentNullException("table");
            if (fields == null || fields.Length < 1) throw new ArgumentNullException("fields");
            if (fields.Any(field => field == null || string.IsNullOrEmpty(field.Trim())))
                throw new ArgumentException("Fields cannot be empty.", "fields");

            var fieldsQuery = string.Join(", ", fields.Select(field => field.Trim().ToLower()).ToArray());

            return await FetchTableRawAsync(string.Format("SELECT {0} FROM {1};",
                fieldsQuery, table.Trim().ToLower()), null);

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
                throw new ArgumentNullException("insertStatementToken");

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
                throw new ArgumentNullException("insertStatement");

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<long>(null, CurrentTransaction, 
                    ReplaceParamsInRawQuery(insertStatement, parameters), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<long>(conn, null, 
                    ReplaceParamsInRawQuery(insertStatement, parameters), parameters);

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

            if (statementToken == null || string.IsNullOrEmpty(statementToken.Trim())) 
                throw new ArgumentNullException("statementToken");

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

            if (statement == null || string.IsNullOrEmpty(statement.Trim())) 
                throw new ArgumentNullException("statement");

            if (this.IsTransactionInProgress)
                return await ExecuteCommandIntAsync<int>(null, CurrentTransaction, 
                    ReplaceParamsInRawQuery(statement, parameters), parameters);

            using (var conn = await OpenConnectionAsync())
            {
                return await ExecuteCommandIntAsync<int>(conn, null, ReplaceParamsInRawQuery(statement, parameters), parameters);
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
            
            if (statements == null || statements.Length < 1)
                throw new ArgumentNullException("statements");

            if (statements.All(statement => statement == null || string.IsNullOrEmpty(statement.Trim())))
                throw new ArgumentException("At least one statement should be non empty.", "statements");

            if (this.IsTransactionInProgress)
                throw new InvalidOperationException("Cannot execute batch while a transaction is in progress.");

            using (var conn = await OpenConnectionAsync())
            {
                try
                {
                    using (var command = new SQLiteCommand())
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
                            conn.Close();
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
            return await Task.FromResult(true); // because it will be created on open conn
        }

        /// <summary>
        /// Checks if the database is empty, i.e. contains no tables.
        /// </summary>
        /// <param name="databaseName">a name of the database to check (not used in SQLite implementation)</param>
        /// <returns>True if the database contains any tables.</returns>
        public override async Task<bool> DatabaseEmptyAsync(string databaseName)
        {
            using (var conn = await OpenConnectionAsync())
            {
                var table = await ExecuteCommandIntAsync<LightDataTable>(conn, null,
                    "SELECT name, sql FROM sqlite_master WHERE type='table' AND NOT name LIKE 'sqlite_%';", null);
                return (table.Rows.Count < 1);
            }                   
        }


        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot get schema while a transaction is in progress.</exception>
        /// <exception cref="InvalidOperationException">Database is not set, cannot get schema.</exception>
        public override async Task<DbSchema> GetDbSchemaAsync()
        {

            if (_currentDatabase == null || string.IsNullOrEmpty(_currentDatabase))
                throw new InvalidOperationException("Database is not set, cannot get schema.");
            if (IsTransactionInProgress) throw new InvalidOperationException("Cannot get schema while a transaction is in progress.");

            var result = new DbSchema {Tables = new List<DbTableSchema>()};

            var indexData = await FetchTableRawAsync(
                "SELECT name, tbl_name, sql FROM sqlite_master WHERE type='index' AND NOT sql IS NULL;", null);

            var tablesData = await FetchTableRawAsync(
                "SELECT name, sql FROM sqlite_master WHERE type='table' AND NOT name LIKE 'sqlite_%';", null);

            foreach (var row in tablesData.Rows)
            {
                result.Tables.Add(await GetDbTableSchemaAsync(row, indexData));
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
            
            if (gaugeSchema == null) throw new ArgumentNullException("gaugeSchema");
            if (actualSchema == null) throw new ArgumentNullException("actualSchema");
            if (IsTransactionInProgress) throw new InvalidOperationException("Cannot get schema errors while a transaction is in progress.");
            
            var result = new List<DbSchemaError>();

            foreach (var gaugeTable in gaugeSchema.Tables)
            {

                var gaugeTableFound = false;

                foreach (var actualTable in actualSchema.Tables)
                {
                    if (actualTable.Name.Trim().ToLower() == gaugeTable.Name.Trim().ToLower())
                    {
                        result.AddRange(GetDbTableSchemaErrors(gaugeTable, actualTable));
                        gaugeTableFound = true;
                        break;
                    }
                }

                if (!gaugeTableFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableMissing, string.Format("Table {0} missing.", 
                        gaugeTable.Name), gaugeTable.Name, gaugeTable.GetCreateTableStatements().ToArray()));
                }

            }

            foreach (var actualTable in actualSchema.Tables)
            {

                var actualTableFound = gaugeSchema.Tables.Any(gaugeTable => actualTable.Name.Trim().ToLower() 
                    == gaugeTable.Name.Trim().ToLower());

                if (!actualTableFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableObsolete, string.Format("Table {0} is obsolete.",
                        actualTable.Name), actualTable.Name, actualTable.GetDropTableStatements().ToArray()));
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
            var script = dbSchema.GetTablesInCreateOrder().Aggregate(new List<string>(),
                            (seed, table) =>
                            {
                                seed.AddRange(table.GetCreateTableStatements());
                                return seed;
                            });
            return string.Join(Environment.NewLine, script.ToArray());
        }

        private List<DbSchemaError> GetDbTableSchemaErrors(DbTableSchema gaugeSchema, DbTableSchema actualSchema)
        {

            var result = new List<DbSchemaError>();

            foreach (var gaugeField in gaugeSchema.Fields)
            {

                var gaugeFieldFound = false;

                foreach (var actualField in actualSchema.Fields)
                {
                    if (gaugeField.Name.Trim().ToLower() == actualField.Name.Trim().ToLower())
                    {

                        var schemasMatch = gaugeField.FieldSchemaMatch(actualField);
                        var indexMatch = gaugeField.FieldIndexMatch(actualField);
                        
                        if (!schemasMatch)
                        {
                            result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.FieldDefinitionObsolete,
                                string.Format("Table {0} field {1} schema definition is obsolete: current definition - {2}; gauge definition - {3}. SQLite does not support field schema changes.",
                                actualSchema.Name, actualField.Name, actualField.GetFieldDefinition(false),
                                gaugeField.GetFieldDefinition(false)), gaugeSchema.Name, gaugeField.Name));
                        }

                        if (!indexMatch)
                        {

                            if (actualField.IndexType == DbIndexType.None &&
                                (gaugeField.IndexType == DbIndexType.Simple ||
                                 gaugeField.IndexType == DbIndexType.Unique))
                            {
                                result.Add(GetDbSchemaError(DbSchemaErrorType.IndexMissing, 
                                    string.Format("Index {0} missing on table {1} field {2}.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name), 
                                    actualSchema.Name, actualField.Name, 
                                    gaugeField.GetAddIndexStatements(actualSchema.Name).ToArray()));
                            }
                            else if (actualField.IndexType == DbIndexType.None &&
                                gaugeField.IndexType == DbIndexType.Primary)
                            {
                                result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.IndexMissing,
                                    string.Format("Primary index missing on table {0} field {1}. SQLite does not support PRIMARY KEY changes.",
                                    actualSchema.Name, actualField.Name), actualSchema.Name, actualField.Name));
                            }
                            else if (actualField.IndexType == DbIndexType.None &&
                                gaugeField.IndexType == DbIndexType.ForeignKey)
                            {
                                result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.IndexMissing,
                                    string.Format("Foreign key {0} missing on table {1} field {2}. SQLite does not support foreign key changes.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name), 
                                    actualSchema.Name, actualField.Name));
                            }
                            else if (actualField.IndexType == DbIndexType.ForeignKey ||
                                gaugeField.IndexType == DbIndexType.ForeignKey)
                            {
                                result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.IndexObsolete,
                                    string.Format("Index {0} type is obsolete on table {1} field {2}. SQLite does not support foreign key changes.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name),
                                    actualSchema.Name, actualField.Name));
                            }
                            else if (actualField.IndexType == DbIndexType.Primary ||
                                gaugeField.IndexType == DbIndexType.Primary)
                            {
                                result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.IndexObsolete,
                                    string.Format("Index {0} type is obsolete on table {1} field {2}. SQLite does not support primary key changes.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name),
                                    actualSchema.Name, actualField.Name));
                            }
                            else if ((actualField.IndexType == DbIndexType.Simple &&
                                gaugeField.IndexType == DbIndexType.Unique) ||
                                (actualField.IndexType == DbIndexType.Unique &&
                                gaugeField.IndexType == DbIndexType.Simple))
                            {
                                var statements = actualField.GetDropIndexStatements();
                                statements.AddRange(gaugeField.GetAddIndexStatements(actualSchema.Name));
                                result.Add(GetDbSchemaError(DbSchemaErrorType.IndexObsolete,
                                    string.Format("Index {0} type is obsolete on table {1} field {2}.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name),
                                    actualSchema.Name, actualField.Name, statements.ToArray()));
                            }
                            else if ((actualField.IndexType == DbIndexType.Simple ||
                                      actualField.IndexType == DbIndexType.Unique) &&
                                     gaugeField.IndexType == DbIndexType.None)
                            {
                                var statements = actualField.GetDropIndexStatements();
                                statements.AddRange(gaugeField.GetAddIndexStatements(actualSchema.Name));
                                result.Add(GetDbSchemaError(DbSchemaErrorType.IndexObsolete,
                                    string.Format("Index {0} type is obsolete on table {1} field {2}.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name),
                                    actualSchema.Name, actualField.Name,
                                    actualField.GetDropIndexStatements().ToArray()));
                            }
                            else
                            {
                                result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.IndexObsolete,
                                    string.Format("Index {0} type is obsolete on table {1} field {2}. Change is not supported by SQLite.",
                                    gaugeField.IndexName, actualSchema.Name, actualField.Name),
                                    actualSchema.Name, actualField.Name));
                            }

                        }
                        
                        gaugeFieldFound = true;
                        break;

                    }
                }

                if (!gaugeFieldFound)
                {
                    if (gaugeField.DataType.ToBaseType() == DbDataType.Blob && gaugeField.NotNull)
                    {
                        result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.FieldMissing, 
                            string.Format("Field {0} in table {1} missing. Cannot add new NOT NULL column for BLOB type in SQLite.",
                            gaugeField.Name, gaugeSchema.Name), gaugeSchema.Name, gaugeField.Name));
                    }
                    else
                    {
                        result.Add(GetDbSchemaError(DbSchemaErrorType.FieldMissing, 
                            string.Format("Field {0} in table {1} missing.",
                            gaugeField.Name, gaugeSchema.Name), gaugeSchema.Name, gaugeField.Name,
                            gaugeField.GetAddFieldStatements(gaugeSchema.Name).ToArray()));
                    }
                }

            }

            foreach (var actualField in actualSchema.Fields)
            {

                var actualFieldFound = gaugeSchema.Fields.Any(gaugeField => actualField.Name.Trim().ToLower()
                    == gaugeField.Name.Trim().ToLower());

                if (!actualFieldFound)
                {
                    result.Add(GetUnrepairableDbSchemaError(DbSchemaErrorType.FieldObsolete, 
                        string.Format("Field {0} in table {1} is obsolete.", actualField.Name, 
                        actualSchema.Name), actualSchema.Name, actualField.Name));
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
        /// <exception cref="ArgumentException">Database schema should contain at least one table.</exception>
        protected override async Task CreateDatabaseAsync(string databaseName, DbSchema dbSchema)
        {

            if (dbSchema.Tables.Count <1) 
                throw new ArgumentException("Database schema should contain at least one table.", "dbSchema");

            var script = dbSchema.GetTablesInCreateOrder().Aggregate(new List<string>(),
                            (seed, table) =>
                            {
                                seed.AddRange(table.GetCreateTableStatements());
                                return seed;
                            });

            var oldDb = _currentDatabase;

            try
            {

                _currentDatabase = databaseName; // SQLite database will be created on connection creation

                await ExecuteCommandBatchAsync(script.ToArray());

            }
            catch (Exception)
            {
                _currentDatabase = oldDb;
                throw;
            }

        }

        /// <summary>
        /// Drops (deletes) the database specified.
        /// </summary>
        /// <param name="databaseName">the name of the database to drop, i.e. the path to the
        /// SQLite database file</param>
        /// <exception cref="ArgumentNullException">Parameter databaseName is not specified.</exception>
        /// <exception cref="InvalidOperationException">Cannot drop database while transaction is in progress.</exception>
        /// <exception cref="ArgumentException">Path to the database contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">The specified file is in use.</exception>
        /// <exception cref="NotSupportedException">path is in an invalid format</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission
        /// or the path specified a read-only file.</exception>
        public override async Task DropDatabaseAsync(string databaseName)
        {
            if (databaseName == null || string.IsNullOrEmpty(databaseName.Trim())) 
                throw new ArgumentNullException("databaseName");
            if (IsTransactionInProgress)
                throw new InvalidOperationException("Cannot drop database while transaction is in progress.");
            using (var stream = new FileStream(databaseName, FileMode.Open, FileAccess.Read, FileShare.None,
                4096, FileOptions.DeleteOnClose | FileOptions.Asynchronous))
            {
                await stream.FlushAsync();
            }
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
                    using (var command = new SQLiteCommand())
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
            await ExecuteCommandIntAsync<int>(null, CurrentTransaction, "PRAGMA foreign_keys = OFF;", null);
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
                await ExecuteCommandIntAsync<int>(null, CurrentTransaction, insertStatement, paramValues.ToArray());
            }

        }



        private async Task<SQLiteConnection> OpenConnectionAsync()
        {

            if (_currentDatabase == null || string.IsNullOrEmpty(_currentDatabase.Trim()))
                throw new InvalidOperationException("Cannot open SQLite connection without a databse specified.");

            SQLiteConnection result;
            if (_baseConnectionString.Trim().StartsWith(";"))
            {
                result = new SQLiteConnection("Data Source=" + _currentDatabase.Trim() + _baseConnectionString);
            }
            else
            {
                result = new SQLiteConnection("Data Source=" + _currentDatabase.Trim() + ";" + _baseConnectionString);
            }
            
            try
            {
                
                await result.OpenAsync();

                // foreign keys are disabled by default in SQLite
                using (var command = new SQLiteCommand())
                {
                    command.Connection = result;
                    command.CommandText = "PRAGMA foreign_keys = ON;";
                    command.ExecuteNonQuery();
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

            if (conn != null && conn.State != ConnectionState.Closed)
            {
                try
                {
                    conn.Close();
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

            var sqliteEx = ex as SQLiteException;
            if (sqliteEx != null)
            {

                if (sqliteEx.ErrorCode == (int)SQLiteErrorCode.Auth ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.Auth_User ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.NotADb ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.CantOpen ||
                    sqliteEx.ErrorCode == (int)SQLiteErrorCode.Corrupt)
                {
                    throw new SqlException("Password is invalid or SQLite file corrupted.", sqliteEx.ErrorCode, sqliteEx);
                }

                if (sqliteEx.ErrorCode == (int)SQLiteErrorCode.NotFound)
                {
                    throw new SqlException(string.Format("SQLite database file {0} not found.", _currentDatabase),
                        sqliteEx.ErrorCode, sqliteEx);
                }

                throw new SqlException(string.Format("SQLite returned error: {0}", sqliteEx.Message), 
                    sqliteEx.ErrorCode, sqliteEx);

            }

            throw new Exception(string.Format("Unknown exception occured while opening SQLite connection: {0}", 
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


        private void AddParams(SQLiteCommand command, SqlParam[] parameters)
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
                    command.Parameters.AddWithValue(ParamPrefix + parameter.Name.Trim(), value);
                }
            }

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

        private string ReplaceParamsInRawQuery(string sqlQuery, SqlParam[] parameters)
        {

            if (parameters == null || parameters.Length < 1) return sqlQuery;

            return parameters.Where(parameter => parameter.ReplaceInQuery).
                Aggregate(sqlQuery, (current, parameter) => 
                    current.Replace("?" + parameter.Name.Trim(), ParamPrefix + parameter.Name.Trim()));

        }

        private async Task<T> ExecuteCommandIntAsync<T>(SQLiteConnection connection, SQLiteTransaction transaction,
            string sqlStatement, SqlParam[] parameters)
        {

            try
            {
                using (var command = new SQLiteCommand())
                {

                    if (transaction == null)
                    {
                        command.Connection = connection;
                    }
                    else
                    {
                        command.Connection = transaction.Connection;
                        command.Transaction = transaction;
                    }

                    command.CommandTimeout = QueryTimeOut;
                    var commandText = ReplaceParams(sqlStatement, parameters);
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
                        var reader = await command.ExecuteReaderAsync();
                        return (T)(Object)(new LightDataTable(reader));
                    }                        
                    else if (typeof (T) == typeof (long))
                    {
                        return (T)(object)(long)(await command.ExecuteScalarAsync());
                    }
                    else if (typeof(T) == typeof(int))
                    {
                        return (T)(Object)(await command.ExecuteNonQueryAsync());
                    }                        
                    else throw new NotSupportedException(string.Format("Generic parameter type {0} is not supported by SQLiteAgent.ExecuteCommandInt.",
                        typeof (T).FullName));

                }
            }
            catch (Exception ex)
            {

                if (transaction != null)
                {

                    try
                    {
                        CurrentTransaction.Rollback();
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            string.Format("Critical SQL transaction error, failed to rollback the transaction.{0}{1}{2}{3}",
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
                if (transaction == null && connection != null)
                {
                    try
                    {
                        if (connection.State != ConnectionState.Closed)
                            connection.Close();
                    }
                    catch (Exception) { }
                }
            }

        }


        private async Task<DbTableSchema> GetDbTableSchemaAsync(LightDataRow tableStatusRow, LightDataTable indexData)
        {

            var result = new DbTableSchema
            {
                Name = tableStatusRow.GetString(0),
                Fields = new List<DbFieldSchema>()
            };

            var indexes = indexData.Rows.Where(
                row => row.GetStringOrDefault(1, string.Empty).Trim().ToLower() 
                    == result.Name.Trim().ToLower()).ToList();

            var foreignKeys = await FetchTableRawAsync(string.Format("PRAGMA foreign_key_list({0});", result.Name), null);

            var fieldsData = await FetchTableRawAsync(string.Format("PRAGMA table_info({0});", result.Name), null);

            var createSql = tableStatusRow.GetString(1);
            createSql = createSql.Replace("[", "").Replace("]", "").Substring(createSql.IndexOf("(") + 1);
            createSql = createSql.Substring(0, createSql.Length - 1);

            foreach (var row in fieldsData.Rows)
            {
                result.Fields.Add(this.GetDbFieldSchema(row, indexes, foreignKeys, 
                    createSql.Split(new string[]{","}, StringSplitOptions.RemoveEmptyEntries), result.Name));
            }

            return result;

        }

        private DbFieldSchema GetDbFieldSchema(LightDataRow fieldStatusRow,
            List<LightDataRow> indexes, LightDataTable foreignKeys, string[] createTableSql, 
            string tableName)
        {

            var result = new DbFieldSchema
            {
                Name = fieldStatusRow.GetString(1),
                NotNull = (fieldStatusRow.GetInt32(3) > 0),
                Unsigned = false,
                Description = string.Empty,
            };

            result.Autoincrement = createTableSql.Any(line => line.Trim().ToLower().StartsWith(
                result.Name.Trim().ToLower() + " ") && line.ToUpper().Contains("AUTOINCREMENT"));

            var rawType = fieldStatusRow.GetString(2).Trim();

            result.DataType = GetFieldType(rawType.ToLower());

            var typeDetails = "";
            if (rawType.Contains("("))
                typeDetails = rawType.Substring(rawType.IndexOf("(", StringComparison.Ordinal) + 1,
                    rawType.IndexOf(")", StringComparison.Ordinal)
                    - rawType.IndexOf("(", StringComparison.Ordinal) - 1);

            if (!string.IsNullOrEmpty(typeDetails) && (result.DataType == DbDataType.Char 
                || result.DataType == DbDataType.VarChar))
            {
                result.Length = 255;
                if (int.TryParse(typeDetails.Trim().Replace("`", "").Replace("'", "").Replace("\"", ""), out int length))
                    result.Length = length;
            }
            
            if (fieldStatusRow.GetInt32(5) > 0)
            {
                result.IndexType = DbIndexType.Primary;
            }
            else
            {

                if (!FindForeignKey(result, tableName, foreignKeys, createTableSql))
                {
                    if (!FindIndex(result, tableName, indexes)) result.IndexType = DbIndexType.None;
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
                case "date":
                    return DbDataType.Date;
                case "datetime":
                    return DbDataType.DateTime;
                case "decimal":
                    return DbDataType.Decimal;
                case "double":
                    return DbDataType.Double;
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
                case "varchar":
                    return DbDataType.VarChar;
                default:
                    throw new NotImplementedException(string.Format(
                        "SQLite database data type {0} is unknown.", definition));
            }
        }

        private static bool FindForeignKey(DbFieldSchema schema, string table, 
            LightDataTable foreignKeys, string[] createTableSql)
        {

            foreach (var fkRow in foreignKeys.Rows.Where(row => row.GetString(3).Trim().ToLower() 
                == schema.Name.Trim().ToLower()))
            {
                
                schema.IndexType = DbIndexType.ForeignKey;

                schema.RefTable = fkRow.GetString(2).Trim();
                schema.RefField = fkRow.GetString(4).Trim();

                foreach (var line in createTableSql.Where(line => line.ToUpper().Contains("CONSTRAINT") &&
                    (line.Trim().ToLower().StartsWith(schema.Name.Trim().ToLower())
                    || line.ToUpper().Contains(string.Format("FOREIGN KEY({0})",
                    schema.Name.Trim().ToUpper())))))
                {
                    var name = line.Replace("\r", " ").Replace("\n", " ").Substring(line.IndexOf("CONSTRAINT") + 11);
                    name = name.Substring(0, name.IndexOf(" ")).Trim();
                    schema.IndexName = name;
                    if (line.Trim().ToUpper().Contains("ON UPDATE CASCADE"))
                    {
                        schema.OnUpdateForeignKey = DbForeignKeyActionType.Cascade;
                    }
                    else if (line.Trim().ToUpper().Contains("ON UPDATE SET NULL"))
                    {
                        schema.OnUpdateForeignKey = DbForeignKeyActionType.SetNull;
                    }
                    else
                    {
                        schema.OnUpdateForeignKey = DbForeignKeyActionType.Restrict;
                    }
                    if (line.Trim().ToUpper().Contains("ON DELETE CASCADE"))
                    {
                        schema.OnDeleteForeignKey = DbForeignKeyActionType.Cascade;
                    }
                    else if (line.Trim().ToUpper().Contains("ON DELETE SET NULL"))
                    {
                        schema.OnDeleteForeignKey = DbForeignKeyActionType.SetNull;
                    }
                    else
                    {
                        schema.OnDeleteForeignKey = DbForeignKeyActionType.Restrict;
                    }

                }

                return true;
                
            }

            return false;

        }

        private static bool FindIndex(DbFieldSchema schema, string table, List<LightDataRow> indexes)
        {

            foreach (var row in indexes.Where(idxRow => idxRow.GetString(2).ToUpper().Contains(
                string.Format("ON {0} ({1})", table.Trim().ToUpper(), schema.Name.Trim().ToUpper()))))
            {
                schema.IndexName = row.GetString(0);
                if (row.GetString(2).ToUpper().Contains("UNIQUE"))
                {
                    schema.IndexType = DbIndexType.Unique;
                }
                else
                {
                    schema.IndexType = DbIndexType.Simple;
                }

                return true;
                
            }

            return false;

        }

    }
}
