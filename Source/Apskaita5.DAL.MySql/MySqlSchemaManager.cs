using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.DbSchema;
using Apskaita5.DAL.Common;
using MySql.Data.MySqlClient;
using System.Linq;
using static Apskaita5.DAL.MySql.Constants;

namespace Apskaita5.DAL.MySql
{
    /// <summary>
    /// Manages MySql database schema (creates, drops, extracts schema, checks for errors against gauge schema, clones database)
    /// </summary>
    public class MySqlSchemaManager : SchemaManagerBase
    {

        #region Properties

        private const string DefaultEngine = "InnoDB";
        private const string DefaultCharset = "utf8";

        private string _engine = DefaultEngine;
        private string _charset = DefaultCharset;


        /// <summary>
        /// Gets or sets the SQL engine to use when creating a database. (default - InnoDB)
        /// </summary>
        public string Engine
        {
            get { return _engine ?? string.Empty; }
            set { _engine = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the default charset to use when creating a database. (default - utf8)
        /// </summary>
        public string Charset
        {
            get { return _charset ?? string.Empty; }
            set { _charset = value?.Trim() ?? string.Empty; }
        }

        /// <summary>
        /// Gets an id of the concrete SQL implementation, i.e. mysql.
        /// The id is used to check for SqlAgent implementation mismatch.
        /// </summary>
        public override string SqlImplementationId => MySqlImplementationId;

        /// <summary>
        /// Gets a typed (native) MySqlAgent.
        /// </summary>
        private MySqlAgent MyAgent => (MySqlAgent)Agent;

        #endregion


        /// <summary>
        /// Creates a new MySql database schema manager.
        /// </summary>
        /// <param name="agent">MySql agent to use for schema management.</param>
        public MySqlSchemaManager(MySqlAgent agent) : base(agent) { }


        #region GetDbSchemaAsync Implementation
        
        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary>  
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public override async Task<DbSchema> GetDbSchemaAsync(CancellationToken cancellationToken)
        {

            if (Agent.CurrentDatabase.IsNullOrWhitespace())
                throw new InvalidOperationException(Properties.Resources.GetSchemaExceptionNullDatabase);
            if (Agent.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.GetSchemaExceptionTransactionInProgress);

            var result = new DbSchema();

            var conn = await MyAgent.OpenConnectionAsync().ConfigureAwait(false);

            try
            {
                var dbData = await MyAgent.FetchUsingConnectionAsync(conn, "SELECT @@character_set_database, @@default_storage_engine;",
                    cancellationToken).ConfigureAwait(false);
                if (dbData.Rows.Count > 0)
                {
                    result.CharsetName = dbData.Rows[0].GetString(0);
                    result.Description = string.Format(Properties.Resources.DbSchemaDescription,
                        Agent.CurrentDatabase, dbData.Rows[0].GetString(1));
                }


                var indexDictionary = await GetIndexesAsync(conn, cancellationToken).ConfigureAwait(false);
                var fkDictionary = await GetForeignKeysAsync(conn, cancellationToken).ConfigureAwait(false);

                result.Tables = new List<DbTableSchema>();

                var tablesData = await MyAgent.FetchUsingConnectionAsync(conn, "SHOW TABLE STATUS;",
                    cancellationToken).ConfigureAwait(false);
                foreach (var row in tablesData.Rows)
                {
                    result.Tables.Add(await GetDbTableSchemaAsync(conn, row, indexDictionary, fkDictionary,
                        cancellationToken).ConfigureAwait(false));
                }

                conn.Close();

                conn.Dispose();

            }
            catch (Exception ex)
            {
                MyAgent.LogAndThrowInt(ex.WrapSqlException());
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

        private async Task<DbTableSchema> GetDbTableSchemaAsync(MySqlConnection conn, LightDataRow tableStatusRow,
            Dictionary<string, Dictionary<string, string>> indexDictionary,
            Dictionary<string, Dictionary<string, ForeignKeyData>> fkDictionary, CancellationToken cancellationToken)
        {

            var result = new DbTableSchema
            {
                Name = tableStatusRow.GetString(0).Trim(),
                EngineName = tableStatusRow.GetString(1).Trim(),
                CharsetName = tableStatusRow.GetString(14).Trim(),
                Description = tableStatusRow.GetString(17).Trim(),
                Fields = new List<DbFieldSchema>()
            };

            Dictionary<string, string> tableIndexDictionary = null;
            Dictionary<string, ForeignKeyData> tableFkDictionary = null;
            if (indexDictionary.ContainsKey(result.Name))
                tableIndexDictionary = indexDictionary[result.Name];
            if (fkDictionary.ContainsKey(result.Name))
                tableFkDictionary = fkDictionary[result.Name];


            var fieldsData = await MyAgent.FetchUsingConnectionAsync(conn,
                string.Format("SHOW FULL COLUMNS FROM {0};", result.Name), cancellationToken).ConfigureAwait(false);

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
                Name = fieldStatusRow.GetString(0).Trim(),
                NotNull = (fieldStatusRow.GetString(3).EqualsByConvention("no")),
                Unsigned = (fieldStatusRow.GetString(1).ContainsByConvention("unsigned")),
                Autoincrement = (fieldStatusRow.GetString(6).ContainsByConvention("auto_increment")),
                Description = fieldStatusRow.GetStringOrDefault(8).Trim()
            };

            var collation = fieldStatusRow.GetStringOrDefault(2);
            if (collation.EqualsByConvention("ascii_bin"))
                result.CollationType = DbFieldCollationType.ASCII_Binary;
            else if (collation.EqualsByConvention("ascii_general_ci"))
                result.CollationType = DbFieldCollationType.ASCII_CaseInsensitive;

            var rawType = fieldStatusRow.GetString(1).Trim();
            result.DataType = GetFieldType(rawType);

            var typeDetails = string.Empty;
            if (rawType.Contains("("))
                typeDetails = rawType.Substring(rawType.IndexOf("(", StringComparison.Ordinal) + 1,
                    rawType.IndexOf(")", StringComparison.Ordinal)
                    - rawType.IndexOf("(", StringComparison.Ordinal) - 1)
                    .Trim()
                    .Replace("`", "").Replace("'", "").Replace("\"", "");

            if (result.DataType == DbDataType.Char || result.DataType == DbDataType.VarChar)
            {
                if (int.TryParse(typeDetails, out int length)) result.Length = length;
            }
            else if (result.DataType == DbDataType.Decimal)
            {
                var intValues = typeDetails.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (intValues.Length > 1)
                {
                    if (int.TryParse(intValues[1].Trim(), out int length))
                        result.Length = length;
                }
            }
            else if (result.DataType == DbDataType.Enum)
            {
                result.EnumValues = typeDetails;
            }

            if (fieldStatusRow.GetString(4).EqualsByConvention("pri"))
            {
                result.IndexType = DbIndexType.Primary;
                if (fkIndex != null && fkIndex.ContainsKey(result.Name))
                {
                    result.IndexType = DbIndexType.ForeignPrimary;
                    fkIndex[result.Name].SetSchema(result);
                }
            }
            else if (fkIndex != null && fkIndex.ContainsKey(result.Name))
            {
                result.IndexType = DbIndexType.ForeignKey;
                fkIndex[result.Name].SetSchema(result);
            }
            else if (indexDictionary != null && indexDictionary.ContainsKey(result.Name))
            {
                result.IndexType = DbIndexType.Simple;
                if (fieldStatusRow.GetString(4).EqualsByConvention("uni"))
                    result.IndexType = DbIndexType.Unique;
                result.IndexName = indexDictionary[result.Name];
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
                        Properties.Resources.NativeTypeUnknownException, definition));
            }
        }

        private async Task<Dictionary<string, Dictionary<string, string>>> GetIndexesAsync(
            MySqlConnection conn, CancellationToken cancellationToken)
        {

            var indexTable = await MyAgent.FetchUsingConnectionAsync(conn, @"
                    SELECT s.TABLE_NAME, s.COLUMN_NAME, s.INDEX_NAME, s.NON_UNIQUE
                    FROM INFORMATION_SCHEMA.STATISTICS s
                    LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE c ON c.TABLE_SCHEMA = s.TABLE_SCHEMA
                    AND c.TABLE_NAME = s.TABLE_NAME AND c.COLUMN_NAME = s.COLUMN_NAME
                    WHERE s.INDEX_NAME <> 'PRIMARY' AND c.CONSTRAINT_NAME IS NULL AND s.TABLE_SCHEMA = DATABASE();",
                    cancellationToken).ConfigureAwait(false);

            var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in indexTable.Rows)
            {
                var current = GetOrCreateTableDictionary(result, row.GetString(0).Trim());
                if (!current.ContainsKey(row.GetString(1).Trim()))
                    current.Add(row.GetString(1).Trim(), row.GetString(2).Trim());
            }

            return result;

        }

        private Dictionary<string, string> GetOrCreateTableDictionary(
            Dictionary<string, Dictionary<string, string>> baseDictionary, string tableName)
        {
            if (baseDictionary.ContainsKey(tableName))
                return baseDictionary[tableName];
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            baseDictionary.Add(tableName, result);
            return result;
        }

        private async Task<Dictionary<string, Dictionary<string, ForeignKeyData>>> GetForeignKeysAsync(
            MySqlConnection conn, CancellationToken cancellationToken)
        {

            var result = new Dictionary<string, Dictionary<string, ForeignKeyData>>(StringComparer.OrdinalIgnoreCase);

            var indexTable = await MyAgent.FetchUsingConnectionAsync(conn, @"
                SELECT i.TABLE_NAME, k.COLUMN_NAME, i.CONSTRAINT_NAME, k.REFERENCED_TABLE_NAME, k.REFERENCED_COLUMN_NAME 
                FROM information_schema.TABLE_CONSTRAINTS i 
                LEFT JOIN information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME 
                WHERE i.CONSTRAINT_TYPE = 'FOREIGN KEY' AND i.TABLE_SCHEMA = DATABASE();",
                cancellationToken).ConfigureAwait(false);

            foreach (var row in indexTable.Rows)
            {
                var current = GetOrCreateFKTableDictionary(result, row.GetString(0).Trim());
                if (!current.ContainsKey(row.GetString(1).Trim()))
                {
                    var fkInfo = new ForeignKeyData
                    {
                        Name = row.GetString(2).Trim(),
                        RefTable = row.GetString(3).Trim(),
                        RefField = row.GetString(4).Trim()
                    };
                    current.Add(row.GetString(1).Trim(), fkInfo);
                }
            }

            foreach (var entry in result)
            {

                var showCreateTable = await MyAgent.FetchUsingConnectionAsync(conn,
                    string.Format("SHOW CREATE TABLE {0};", entry.Key), cancellationToken).ConfigureAwait(false);
                var showCreateLines = showCreateTable.Rows[0].GetString(1).Split(new string[] { "," },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in showCreateLines)
                {
                    if (line.Trim().StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var column in entry.Value)
                        {
                            if (line.ContainsByConvention("`" + column.Value.Name + "`")
                                || line.ContainsByConvention(" " + column.Value.Name + " "))
                            {
                                column.Value.OnUpdate = DbForeignKeyActionType.Restrict;
                                if (line.ContainsByConvention("ON UPDATE CASCADE"))
                                    column.Value.OnUpdate = DbForeignKeyActionType.Cascade;
                                if (line.ContainsByConvention("ON UPDATE SET NULL"))
                                    column.Value.OnUpdate = DbForeignKeyActionType.SetNull;

                                column.Value.OnDelete = DbForeignKeyActionType.Restrict;
                                if (line.ContainsByConvention("ON DELETE CASCADE"))
                                    column.Value.OnDelete = DbForeignKeyActionType.Cascade;
                                if (line.ContainsByConvention("ON DELETE SET NULL"))
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
            var result = new Dictionary<string, ForeignKeyData>(StringComparer.OrdinalIgnoreCase);
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

            public void SetSchema(DbFieldSchema schema)
            {
                schema.IndexName = Name;
                schema.OnUpdateForeignKey = OnUpdate;
                schema.OnDeleteForeignKey = OnDelete;
                schema.RefTable = RefTable;
                schema.RefField = RefField;
            }

        }

        #endregion

        #region GetDbSchemaErrors Implementation

        /// <summary>
        /// Compares the actualSchema definition to the gaugeSchema definition and returns
        /// a list of DbSchema errors, i.e. inconsistencies found and SQL statements to repair them.
        /// </summary>
        /// <param name="gaugeSchema">the gauge schema definition to compare the actualSchema against</param>
        /// <param name="actualSchema">the schema to check for inconsistencies (and repair)</param>
        public override List<DbSchemaError> GetDbSchemaErrors(DbSchema gaugeSchema, DbSchema actualSchema)
        {

            if (gaugeSchema.IsNull()) throw new ArgumentNullException(nameof(gaugeSchema));
            if (actualSchema.IsNull()) throw new ArgumentNullException(nameof(actualSchema));
            if (Agent.IsTransactionInProgress) throw new InvalidOperationException(Properties.Resources.GetSchemaErrorsExceptionTransactionInProgress);

            var result = new List<DbSchemaError>();

            foreach (var gaugeTable in gaugeSchema.Tables)
            {

                var gaugeTableFound = false;

                foreach (var actualTable in actualSchema.Tables)
                {
                    if (actualTable.Name.EqualsByConvention(gaugeTable.Name))
                    {
                        result.AddRange(GetDbTableSchemaErrors(gaugeTable, actualTable));
                        gaugeTableFound = true;
                        break;
                    }
                }

                if (!gaugeTableFound)
                {

                    var applicableEngine = _engine;
                    if (applicableEngine.IsNullOrWhitespace()) applicableEngine = DefaultEngine;
                    var applicableCharset = _charset;
                    if (applicableCharset.IsNullOrWhitespace()) applicableCharset = DefaultCharset;

                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableMissing,
                        string.Format(Properties.Resources.DbSchemaErrorTableMissing, gaugeTable.Name),
                        gaugeTable.Name, gaugeTable.GetCreateTableStatements(
                        Agent.CurrentDatabase, applicableEngine, applicableCharset, MyAgent).ToArray()));

                }

            }

            foreach (var actualTable in actualSchema.Tables)
            {

                var actualTableFound = gaugeSchema.Tables.Any(gaugeTable => actualTable.Name.EqualsByConvention(gaugeTable.Name));

                if (!actualTableFound)
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.TableObsolete,
                        string.Format(Properties.Resources.DbSchemaErrorTableObsolete, actualTable.Name),
                        actualTable.Name, actualTable.GetDropTableStatements(Agent.CurrentDatabase, MyAgent).ToArray()));
                }

            }

            return result;

        }

        private List<DbSchemaError> GetDbTableSchemaErrors(DbTableSchema gaugeSchema, DbTableSchema actualSchema)
        {

            var result = new List<DbSchemaError>();

            foreach (var gaugeField in gaugeSchema.Fields)
            {

                var gaugeFieldFound = false;

                foreach (var actualField in actualSchema.Fields)
                {
                    if (gaugeField.Name.EqualsByConvention(actualField.Name))
                    {

                        var schemasMatch = gaugeField.FieldSchemaMatch(actualField);
                        var indexMatch = gaugeField.FieldIndexMatch(actualField);
                        var statements = new List<string>();
                        var inconsistencyType = DbSchemaErrorType.FieldDefinitionObsolete;
                        var description = string.Empty;

                        if (!schemasMatch && !indexMatch)
                        {
                            statements.AddRange(actualField.GetDropIndexStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            statements.AddRange(gaugeField.GetAlterFieldStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            statements.AddRange(gaugeField.GetAddIndexStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            description = string.Format(Properties.Resources.DbSchemaErrorFieldAndIndexObsolete,
                                gaugeSchema.Name, actualField.Name);
                        }
                        else if (!schemasMatch)
                        {
                            statements.AddRange(gaugeField.GetAlterFieldStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            description = string.Format(Properties.Resources.DbSchemaErrorFieldObsolete,
                                gaugeSchema.Name, actualField.Name);
                        }
                        else if (!indexMatch)
                        {
                            statements.AddRange(actualField.GetDropIndexStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            statements.AddRange(gaugeField.GetAddIndexStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent));
                            inconsistencyType = DbSchemaErrorType.IndexObsolete;
                            description = string.Format(Properties.Resources.DbSchemaErrorIndexObsolete,
                                gaugeSchema.Name, actualField.Name);
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
                    result.Add(GetDbSchemaError(DbSchemaErrorType.FieldMissing,
                        string.Format(Properties.Resources.DbSchemaErrorFieldMissing,
                        gaugeField.Name, gaugeSchema.Name), gaugeSchema.Name, gaugeField.Name,
                        gaugeField.GetAddFieldStatements(Agent.CurrentDatabase, gaugeSchema.Name, MyAgent).ToArray()));
                }

            }

            foreach (var actualField in actualSchema.Fields)
            {

                if (!gaugeSchema.Fields.Any(gaugeField => actualField.Name.EqualsByConvention(gaugeField.Name)))
                {
                    result.Add(GetDbSchemaError(DbSchemaErrorType.FieldObsolete,
                        string.Format(Properties.Resources.DbSchemaErrorFieldRedundant,
                        actualField.Name, actualSchema.Name), actualSchema.Name, actualField.Name,
                        actualField.GetDropFieldStatements(Agent.CurrentDatabase, actualSchema.Name, MyAgent).ToArray()));
                }

            }

            return result;

        }

        #endregion
     
        /// <summary>
        /// Gets an SQL script to create a database for the dbSchema specified.
        /// </summary>
        /// <param name="dbSchema">the database schema to get the create database script for</param>
        public override string GetCreateDatabaseSql(DbSchema dbSchema)
        {

            if (dbSchema.IsNull()) throw new ArgumentNullException(nameof(dbSchema));

            var createScript = new List<string>
                {
                    "CREATE DATABASE DoomyDatabaseName CHARACTER SET utf8;",
                    "USE DoomyDatabaseName;"
                };

            foreach (var table in dbSchema.GetTablesInCreateOrder())
            {
                createScript.AddRange(table.GetCreateTableStatements("DoomyDatabaseName",
                    DefaultEngine, DefaultCharset, MyAgent));
            }

            return string.Join(Environment.NewLine, createScript.ToArray());

        }

        /// <summary>
        /// A method that should do the actual new database creation.
        /// </summary>
        /// <param name="dbSchema">a DbSchema to use for the new database</param>
        /// <remarks>After creating a new database the <see cref="SqlAgentBase.CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        public override async Task CreateDatabaseAsync(DbSchema dbSchema)
        {

            if (dbSchema.IsNull()) throw new ArgumentNullException(nameof(dbSchema));
            if (Agent.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.CreateDatabaseExceptionTransactionInProgress);

            var applicableEngine = _engine;
            if (applicableEngine.IsNullOrWhitespace()) applicableEngine = DefaultEngine;
            var applicableCharset = _charset;
            if (applicableCharset.IsNullOrWhitespace()) applicableCharset = DefaultCharset;

            var createScript = new List<string>
                {
                    string.Format("CREATE DATABASE {0} CHARACTER SET {1};", Agent.CurrentDatabase.Trim(), applicableCharset),
                    string.Format("USE {0};", Agent.CurrentDatabase.Trim())
                };

            foreach (var table in dbSchema.GetTablesInCreateOrder())
            {
                createScript.AddRange(table.GetCreateTableStatements(Agent.CurrentDatabase,
                    applicableEngine, applicableCharset, MyAgent));
            }

            using (var conn = await MyAgent.OpenConnectionAsync(true))
            {
                using (var command = new MySqlCommand())
                {
                    command.Connection = conn;
                    command.CommandTimeout = Agent.QueryTimeOut;

                    string currentStatement = string.Empty;

                    try
                    {
                        foreach (var statement in createScript)
                        {
                            currentStatement = statement;
                            command.CommandText = statement;
                            await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        MyAgent.LogAndThrowInt(ex.WrapSqlException(currentStatement));
                    }
                    finally
                    {
                        if (conn != null )
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

                }
            }

        }

        /// <summary>
        /// Drops (deletes) the database specified.
        /// </summary>
        public override Task DropDatabaseAsync()
        {

            if (Agent.IsTransactionInProgress)
                throw new InvalidOperationException(Properties.Resources.DropDatabaseExceptionTransactionInProgress);

            return Agent.ExecuteCommandRawAsync(string.Format("DROP DATABASE {0};", Agent.CurrentDatabase), null);

        }

        #region Database Cloning Methods

        /// <summary>
        /// Copies table data from the current SqlAgent instance to the target SqlAgent instance.
        /// </summary>
        /// <param name="schema">a schema of the database to copy the data</param>
        /// <param name="targetManager">the target Sql schema manager to copy the data to</param>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.
        /// Basicaly iterates tables, selects data, creates an IDataReader for the table and passes it to the 
        /// <see cref="InsertTableData">InsertTableData</see> method of the target SqlAgent.</remarks>
        protected override async Task CopyData(DbSchema schema, SchemaManagerBase targetManager,
            IProgress<DbCloneProgressArgs> progress, CancellationToken ct)
        {

            if (schema.IsNull()) throw new ArgumentNullException(nameof(schema));
            if (targetManager.IsNull()) throw new ArgumentNullException(nameof(targetManager));

            var currentStatement = string.Empty;

            using (var conn = await MyAgent.OpenConnectionAsync().ConfigureAwait(false))
            {
                try
                {
                    using (var command = new MySqlCommand())
                    {

                        command.Connection = conn;
                        command.CommandTimeout = Agent.QueryTimeOut;

                        progress?.Report(new DbCloneProgressArgs(DbCloneProgressArgs.Stage.FetchingRowCount, string.Empty, 0));

                        long totalRowCount = 0;
                        foreach (var table in schema.Tables)
                        {
                            currentStatement = string.Format("SELECT COUNT(*) FROM {0};", table.Name.Trim());
                            command.CommandText = currentStatement;
                            totalRowCount += (long)await command.ExecuteScalarAsync().ConfigureAwait(false);
                            if (CloneCanceled(progress, ct)) return;
                        }

                        long currentRow = 0;
                        int currentProgress = 0;

                        foreach (var table in schema.Tables)
                        {

                            var fields = string.Join(", ", table.Fields.Select(
                                field => field.Name.ToConventional(Agent)).ToArray());

                            currentStatement = string.Format("SELECT {0} FROM {1};", fields, table.Name.ToConventional(Agent));
                            command.CommandText = currentStatement;

                            using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                            {
                                currentRow = await CallInsertTableDataAsync(targetManager, table, reader,
                                    totalRowCount, currentRow, currentProgress, progress, ct).ConfigureAwait(false);
                                if (currentRow < 0) break;
                                if (totalRowCount > 0) currentProgress = (int)(100 * currentRow / (double)totalRowCount);
                            }

                        }

                    }
                }
                catch (Exception ex)
                {
                    MyAgent.LogAndThrowInt(ex.WrapSqlException(currentStatement));
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

            }

        }

        /// <summary>
        /// Disables foreign key checks for the current transaction.
        /// </summary>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.</remarks>
        protected override async Task DisableForeignKeysForCurrentTransactionAsync()
        {
            if (!Agent.IsTransactionInProgress) throw new InvalidOperationException(
                Properties.Resources.DisableForeignKeysExceptionTransactionNull);
            await Agent.ExecuteCommandRawAsync("SET FOREIGN_KEY_CHECKS = 0;", null).ConfigureAwait(false);
        }

        /// <summary>
        /// Inserts table data from the reader to the current SqlAgent instance,
        /// </summary>
        /// <param name="table">a schema of the table to insert the data to</param>
        /// <param name="reader">an IDataReader to read the table data from.</param>
        /// <remarks>Required for <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see> infrastructure.
        /// The insert is performed using a transaction that is already initiated by the 
        /// <see cref="SqlAgentBase.CloneDatabase">CloneDatabase</see>.</remarks>
        protected override async Task<long> InsertTableDataAsync(DbTableSchema table, IDataReader reader,
            long totalRowCount, long currentRow, int currentProgress, IProgress<DbCloneProgressArgs> progress,
            CancellationToken ct)
        {

            var fields = table.Fields.Select(field => field.Name.ToConventional(Agent)).ToList();

            var paramPrefixedNames = new List<string>();
            var paramNames = new List<string>();
            for (int i = 0; i < fields.Count; i++)
            {
                var paramName = GetParameterName(i);
                paramNames.Add(paramName);
                paramPrefixedNames.Add(ParamPrefix + paramName);
            }

            var insertStatement = string.Format("INSERT INTO {0}({1}) VALUES({2});",
                table.Name.ToConventional(Agent), string.Join(", ", fields.ToArray()),
                string.Join(", ", paramPrefixedNames.ToArray()));

            while (reader.Read())
            {
                var paramValues = new List<SqlParam>();
                for (int i = 0; i < fields.Count; i++)
                {
                    paramValues.Add(new SqlParam(paramNames[i], reader.GetValue(i)));
                }

                await Agent.ExecuteCommandRawAsync(insertStatement, paramValues.ToArray()).ConfigureAwait(false);

                if (CloneCanceled(progress, ct)) return -1;

                currentRow += 1;

                if (progress != null && totalRowCount > 0)
                {
                    var recalculatedProgress = (int)(100 * currentRow / (double)totalRowCount);
                    if (recalculatedProgress > currentProgress)
                    {
                        currentProgress = recalculatedProgress;
                        progress.Report(new DbCloneProgressArgs(DbCloneProgressArgs.Stage.CopyingData,
                            table.Name, currentProgress));
                    }
                }

            }

            return currentRow;

        }

        #endregion

    }
}
