using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common.DbSchema
{
    /// <summary>
    /// an abstraction for database schema manager for DI
    /// </summary>
    public interface ISchemaManager
    {

        /// <summary>
        /// Gets a <see cref="DbSchema">DbSchema</see> instance (a canonical database description) 
        /// for the current database.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<DbSchema> GetDbSchemaAsync(CancellationToken cancellationToken);

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
        Task<List<DbSchemaError>> GetDbSchemaErrorsAsync(string dbSchemaFolderPath, Guid[] forExtensions,
            CancellationToken cancellationToken);

        /// <summary>
        /// Compares the actualSchema definition to the gaugeSchema definition and returns
        /// a list of DbSchema errors, i.e. inconsistencies found and SQL statements to repair them.
        /// </summary>
        /// <param name="gaugeSchema">the gauge schema definition to compare the actualSchema against</param>
        /// <param name="actualSchema">the schema to check for inconsistencies (and repair)</param>
        List<DbSchemaError> GetDbSchemaErrors(DbSchema gaugeSchema, DbSchema actualSchema);

        /// <summary>
        /// Gets an SQL script to create a database for the dbSchema specified.
        /// </summary>
        /// <param name="dbSchema">the database schema to get the create database script for</param>
        string GetCreateDatabaseSql(DbSchema dbSchema);

        /// <summary>
        /// Creates a new database using DbSchema.
        /// </summary>
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
        Task CreateDatabaseAsync(string dbSchemaFolderPath, Guid[] forExtensions = null);

        /// <summary>
        /// Creates a new database using DbSchema.
        /// </summary>
        /// <param name="dbSchema">a DbSchema to use for the new database</param>
        /// <remarks>After creating a new database the <see cref="CurrentDatabase">CurrentDatabase</see>
        /// property should be set to the new database name.</remarks>
        Task CreateDatabaseAsync(DbSchema dbSchema);

        /// <summary>
        /// Drops (deletes) the current database.
        /// </summary>
        Task DropDatabaseAsync();

    }
}
