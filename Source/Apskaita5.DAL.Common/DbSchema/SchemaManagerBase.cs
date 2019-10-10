using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common.DbSchema
{   

    public abstract class SchemaManagerBase : ISchemaManager
    {

        private ISqlAgent _agent;


        /// <summary>
        /// Gets an id of the concrete SQL implementation, e.g. MySQL, SQLite.
        /// The id is used to make sure that the OrmServiceBase implementation match SqlAgentBase implementation.
        /// </summary>
        public abstract string SqlImplementationId { get; }

        /// <summary>
        /// Gets an instance of an Sql agent to use for queries and statements.
        /// </summary>
        protected ISqlAgent Agent => _agent;


        /// <summary>
        /// Creates a new Orm service.
        /// </summary>
        /// <param name="agent">an instance of an Sql agent to use for queries and statements; its implementation
        /// type should match Orm service implementation type</param>
        public SchemaManagerBase(ISqlAgent agent)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            if (!_agent.SqlImplementationId.EqualsByConvention(SqlImplementationId))
                throw new ArgumentException(string.Format(Properties.Resources.SqlAgentAndSchemaManagerTypeMismatchException,
                    _agent.SqlImplementationId, SqlImplementationId), nameof(agent));
            if (_agent.CurrentDatabase.IsNullOrWhiteSpace()) throw new ArgumentException(
                Properties.Resources.SchemaManagerRequiresDatabaseException, nameof(agent));
        }


        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public abstract Task<DbSchema> GetDbSchemaAsync(CancellationToken cancellationToken);

        #region GetDbSchemaErrorsAsync Implementation
        
        /// <summary>
        /// Compares the current database definition to the gauge definition definition read from 
        /// the file specified and returns a list of DbSchema errors, i.e. inconsistencies found 
        /// and SQL statements to repair them.
        /// </summary>
        /// <param name="dbSchemaFolderPath">the path to the folder that contains gauge schema files
        /// (all files loaded in order to support plugins that may require their own tables)</param>
        /// <param name="forExtensions">a list of extensions Guid's to include in schema;
        /// if null or empty all schema files will be included</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
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
        public async Task<List<DbSchemaError>> GetDbSchemaErrorsAsync(string dbSchemaFolderPath,
            Guid[] forExtensions, CancellationToken cancellationToken)
        {

            if (dbSchemaFolderPath.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(dbSchemaFolderPath));
            if (!System.IO.Directory.Exists(dbSchemaFolderPath)) throw new ArgumentException(string.Format(
                Properties.Resources.DbSchemaFolderDoesNotExistException, dbSchemaFolderPath), nameof(dbSchemaFolderPath));

            var gaugeSchema = new DbSchema();
            gaugeSchema.LoadXmlFolder(dbSchemaFolderPath, forExtensions);

            if (gaugeSchema.Tables.Count < 1) throw new ArgumentException(Properties.Resources.SqlAgentBase_GaugeSchemaEmpty);

            var actualSchema = await GetDbSchemaAsync(cancellationToken).ConfigureAwait(false);

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
            return new DbSchemaError(errorType, description, table, field, sqlStatementsToRepair);
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
            return new DbSchemaError(errorType, description, sqlStatementsToRepair);
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

        #endregion

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
        /// <param name="forExtensions">a list of extensions Guid's to include in schema;
        /// if null or empty all schema files will be included</param>
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
        public Task CreateDatabaseAsync(string dbSchemaFolderPath, Guid[] forExtensions = null)
        {

            if (_agent.IsTransactionInProgress) throw new InvalidOperationException(Properties.Resources.SqlAgentBase_CannotCreateDatabase);
            if (dbSchemaFolderPath.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(dbSchemaFolderPath));
            if (!System.IO.Directory.Exists(dbSchemaFolderPath)) throw new ArgumentException(string.Format(
                Properties.Resources.DbSchemaFolderDoesNotExistException, dbSchemaFolderPath), nameof(dbSchemaFolderPath));

            var dbSchema = new DbSchema();
            dbSchema.LoadXmlFolder(dbSchemaFolderPath, forExtensions);

            if (dbSchema.Tables.Count < 1) throw new ArgumentException(Properties.Resources.SqlAgentBase_GaugeSchemaEmpty);

            return CreateDatabaseAsync(dbSchema);

        }

        /// <summary>
        /// A method that should do the actual new database creation.
        /// </summary>
        /// <param name="databaseName">a name of the new database to create</param>
        /// <param name="dbSchema">a DbSchema to use for the new database</param>
        /// <remarks>After creating a new database the <see cref="CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        public abstract Task CreateDatabaseAsync(DbSchema dbSchema);

        /// <summary>
        /// Drops (deletes) the database specified.
        /// </summary>
        public abstract Task DropDatabaseAsync();

        #region Database Cloning Methods

        /// <summary>
        /// Creates a clone of the database using another SqlAgent.
        /// </summary>
        /// <param name="cloneManager">an SQL schema manager to use when creating clone database</param>
        /// <param name="schemaToUse">a database schema to use (enforce), 
        /// if null the schema ir read from the database cloned</param>
        /// <param name="ct">a cancelation token (if any)</param>
        /// <param name="progress">a progress callback (if any)</param>
        public async Task CloneDatabase(SchemaManagerBase cloneManager, DbSchema schemaToUse, 
            IProgress<DbCloneProgressArgs> progress, CancellationToken ct)
        {

            if (cloneManager.IsNull()) throw new ArgumentNullException(nameof(cloneManager));

            progress?.Report(new DbCloneProgressArgs(DbCloneProgressArgs.Stage.FetchingSchema, string.Empty, 0));

            if (schemaToUse.IsNull()) schemaToUse = await GetDbSchemaAsync(ct).ConfigureAwait(false);

            if (CloneCanceled(progress, ct)) return;

            progress?.Report(new DbCloneProgressArgs(DbCloneProgressArgs.Stage.CreatingSchema, string.Empty, 0));

            await cloneManager.CreateDatabaseAsync(schemaToUse).ConfigureAwait(false);

            if (CloneCanceled(progress, ct)) return;

            await cloneManager.Agent.ExecuteInTransactionAsync(async (cancellationToken) =>
            {
                await cloneManager.DisableForeignKeysForCurrentTransactionAsync().ConfigureAwait(false);
                await CopyData(schemaToUse, cloneManager, progress, cancellationToken).ConfigureAwait(false);
            }, ct).ConfigureAwait(false);

            if (!ct.IsCancellationRequested) progress?.Report(
                new DbCloneProgressArgs(DbCloneProgressArgs.Stage.Completed, string.Empty, 100));
            
        }

        protected static bool CloneCanceled(IProgress<DbCloneProgressArgs> progress, CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                progress?.Report(new DbCloneProgressArgs(DbCloneProgressArgs.Stage.Canceled, string.Empty, 100));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Copies table data from the current SqlAgent instance to the target SqlAgent instance.
        /// </summary>
        /// <param name="schema">a schema of the database to copy the data</param>
        /// <param name="targetManager">the target Sql schema manager to copy the data to</param>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.
        /// Basicaly iterates tables, selects data, creates an IDataReader for the table and passes it to the 
        /// <see cref="InsertTableData">InsertTableData</see> method of the target SqlAgent.</remarks>
        protected abstract Task CopyData(DbSchema schema, SchemaManagerBase targetManager,
            IProgress<DbCloneProgressArgs> progress, CancellationToken ct);

        /// <summary>
        /// Disables foreign key checks for the current transaction.
        /// </summary>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.</remarks>
        protected abstract Task DisableForeignKeysForCurrentTransactionAsync();

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
        protected static Task<long> CallInsertTableDataAsync(SchemaManagerBase target, DbTableSchema table,
            IDataReader reader, long totalRowCount, long currentRow, int currentProgress,
            IProgress<DbCloneProgressArgs> progress, CancellationToken ct)
        {
            return target.InsertTableDataAsync(table, reader, totalRowCount, currentRow, currentProgress, progress, ct);
        }

        /// <summary>
        /// Inserts table data from the reader to the current SqlAgent instance,
        /// </summary>
        /// <param name="table">a schema of the table to insert the data to</param>
        /// <param name="reader">an IDataReader to read the table data from.</param>
        /// <remarks>Required for <see cref="CloneDatabase">CloneDatabase</see> infrastructure.
        /// The insert is performed using a transaction that is already initiated by the 
        /// <see cref="CloneDatabase">CloneDatabase</see>.</remarks>
        protected abstract Task<long> InsertTableDataAsync(DbTableSchema table, IDataReader reader,
            long totalRowCount, long currentRow, int currentProgress, IProgress<DbCloneProgressArgs> progress,
            CancellationToken ct);

        private static readonly string[] ParamLetters = new string[]{"A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "Q", "P", "R", "S", "T", "U", "V", "Z", "W"};
        private static List<string> _paramDictionary = null;

        /// <summary>
        /// Gets a parameter name for a parameter at the index (position) specified.
        /// </summary>
        /// <param name="index">the zero based index (position) of the parameter</param>
        /// <remarks>Infrastructure for insert statement generation.</remarks>
        protected static string GetParameterName(int index)
        {

            if (_paramDictionary.IsNull())
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

            if (index < 0 || index + 1 > _paramDictionary.Count)
                throw new IndexOutOfRangeException();

            return _paramDictionary[index];

        }

        #endregion

    }
}
