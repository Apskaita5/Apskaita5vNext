using System;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a DbSchema error, i.e. inconsistency of (real and gauge) DbSchema.
    /// </summary>
    /// <remarks>Accounting.DAL should implement a wrapper/duplicate class ReadOnlyBase{}
    /// in order to make use of CSLA communication and to allow for custom DbSchema errors
    /// that could be repaired by a special method and not only SQL statements.</remarks>
    public sealed class DbSchemaError
    {

        private readonly Guid _guid = Guid.NewGuid();
        private readonly DbSchemaErrorType _errorType = DbSchemaErrorType.FieldMissing;
        private readonly string _description = string.Empty;
        private readonly string _table = string.Empty;
        private readonly string _field = string.Empty;
        private readonly bool _isRepairable = true;
        private readonly string[] _sqlStatementsToRepair = null;


        /// <summary>
        /// Gets the type of the DbSchema error.
        /// </summary>
        public DbSchemaErrorType ErrorType
        {
            get { return _errorType; }
        }

        /// <summary>
        /// Gets the description of the DbSchema error.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Gets the name of the database table which schema is inconsistent or which field schema is inconsistent.
        /// </summary>
        public string Table
        {
            get { return _table; }
        }

        /// <summary>
        /// Gets the name of the database field which schema is inconsistent (if any).
        /// </summary>
        public string Field
        {
            get { return _field; }
        }

        /// <summary>
        /// Gets a value indicating whether the DbSchema error can be repaired (by issuing the SqlStatementsToRepair).
        /// </summary>
        public bool IsRepairable
        {
            get { return _isRepairable; }
        }

        /// <summary>
        /// Gets the SQL statements that should be issued to repair the error.
        /// </summary>
        public string[] SqlStatementsToRepair
        {
            get { return _sqlStatementsToRepair; }
        }


        /// <summary>
        /// Initializes a new instance of the SqlSchemaError for a repairable field level error.
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
        internal DbSchemaError(DbSchemaErrorType errorType, string description, string table,
            string field, string[] sqlStatementsToRepair)
        {
            if (description.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(description));
            if (table.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(table));
            if (sqlStatementsToRepair == null || sqlStatementsToRepair.Length < 1) 
                throw new ArgumentNullException(nameof(sqlStatementsToRepair));

            ValidateSqlStatements(sqlStatementsToRepair);

            _errorType = errorType;
            _description = description.Trim();
            _table = table.Trim().ToLowerInvariant();
            _field = field.NotNullValue().Trim().ToLowerInvariant();
            _sqlStatementsToRepair = sqlStatementsToRepair;
            _isRepairable = true;

        }

        /// <summary>
        /// Initializes a new instance of the SqlSchemaError for a repairable table level error.
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
        internal DbSchemaError(DbSchemaErrorType errorType, string description, string table,
            string[] sqlStatementsToRepair):this(errorType,description,table,string.Empty,sqlStatementsToRepair){}

        /// <summary>
        /// Initializes a new instance of the SqlSchemaError for a repairable database level error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="sqlStatementsToRepair">a collection of the SQL statements 
        /// that should be issued to repair the error (must be specified)</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        /// <exception cref="ArgumentNullException">Error table is not specified.</exception>
        /// <exception cref="ArgumentNullException">SQL statements to repair the error is not specified.</exception>
        /// <exception cref="ArgumentException">No SQL statement could be empty.</exception>
        internal DbSchemaError(DbSchemaErrorType errorType, string description, string[] sqlStatementsToRepair)
        {
            if (description.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(description));
            if (sqlStatementsToRepair == null || sqlStatementsToRepair.Length < 1)
                throw new ArgumentNullException(nameof(sqlStatementsToRepair));

            ValidateSqlStatements(sqlStatementsToRepair);

            _errorType = errorType;
            _description = description.Trim();
            _table = string.Empty;
            _field = string.Empty;
            _sqlStatementsToRepair = sqlStatementsToRepair;
            _isRepairable = true;

        }

        /// <summary>
        /// Initializes a new instance of the SqlSchemaError for an unrepairable error.
        /// </summary>
        /// <param name="errorType">a type of the error (inconsistency)</param>
        /// <param name="description">a description of the error (inconsistency) (must be specified)</param>
        /// <param name="table">the name of the database table which field is inconsistent</param>
        /// <param name="field">the name of the database field that is inconsistent</param>
        /// <exception cref="ArgumentNullException">Error description is not specified.</exception>
        internal DbSchemaError(DbSchemaErrorType errorType, string description, string table, string field)
        {
            if (description.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(description));
            
            _errorType = errorType;
            _description = description.Trim();
            _table = table.NotNullValue().Trim().ToLowerInvariant();
            _field = field.NotNullValue().Trim().ToLowerInvariant();
            _sqlStatementsToRepair = new string[]{};
            _isRepairable = false;

        }


        /// <summary>
        /// Technical method to support databinding. Returns a Guid 
        /// that is created for every new DbSchemaError instance (not persisted).
        /// </summary>
        /// <returns></returns>
        public Object GetObjectId()
        {
            return _guid;
        }


        private void ValidateSqlStatements(string[] sqlStatements)
        {
            if (sqlStatements.Any(sqlStatement => sqlStatement.IsNullOrWhiteSpace()))
            {
                throw new ArgumentException(Properties.Resources.DbSchemaError_EmptySqlStatementInList, nameof(sqlStatements));
            }
        }

    }
}
