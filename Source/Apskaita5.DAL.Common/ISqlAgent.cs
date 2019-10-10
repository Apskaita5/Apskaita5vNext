using System;
using System.Threading;
using System.Threading.Tasks;
using Apskaita5.DAL.Common.DbSchema;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Provides an abstraction over a concrete SQL implementation, e.g. MySql, SQLite, etc.
    /// </summary>
    public interface ISqlAgent : IDisposable
    {

        #region Properties

        /// <summary>
        /// Gets a name of the SQL implementation, e.g. MySql, SQLite, etc.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets an id of the concrete SQL implementation, e.g. MySQL, SQLite.
        /// The id is used to select an appropriate SQL token dictionary.
        /// </summary>
        string SqlImplementationId { get; }

        /// <summary>
        /// Gets a value indicationg whether the SQL engine is file based (e.g. SQLite),
        /// otherwise - server based (e.g. MySQL).
        /// </summary>
        bool IsFileBased { get; }

        /// <summary>
        /// Gets a name of the root user as defined in the SQL implementation behind the SqlAgent,
        /// e.g. root, sa, etc.
        /// </summary>
        string RootName { get; }

        /// <summary>
        /// Gets a simbol used as a wildcart for the SQL implementation, e.g. %, *, etc.
        /// </summary>
        string Wildcart { get; }

        /// <summary>
        /// Gets a connection string that does NOT include database parameter. 
        /// </summary>
        /// <remarks>Should be initialized when creating an SqlAgent instance.</remarks>
        string BaseConnectionString { get; }

        /// <summary>
        /// Gets the current database name (string.Empty for no database).
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot change database while a transaction is in progress.</exception>
        string CurrentDatabase { get; }

        /// <summary>
        /// Gets a value indicationg whether an SQL transation is in progress.
        /// </summary>
        bool IsTransactionInProgress { get; }



        /// <summary>
        /// Gets or sets a query timeout in ms. 
        /// </summary>
        int QueryTimeOut { get; set; }
  
        /// <summary>
        /// Gets or sets whether a transaction is stored within SqlAgent instance.
        /// If not, a transaction is stored within AsyncLocal storage.
        /// </summary>
        bool UseTransactionPerInstance { get; set; }

        /// <summary>
        /// Gets or sets whether a boolean type is stored as TinyInt, i.e. param values needs
        /// to be replaced: true = 1; false = 0.
        /// </summary>
        bool BooleanStoredAsTinyInt { get; set; }

        /// <summary>
        /// Gets or sets whether a Guid type is stored as Blob. Otherwise, Guid is stored as CHAR(32).
        /// </summary>
        bool GuidStoredAsBlob { get; set; }

        /// <summary>
        /// Gets or sets whether all schema names (tables, fields, indexes etc) are lower case only.
        /// </summary>
        bool AllSchemaNamesLowerCased { get; set; }
 
        #endregion

        /// <summary>
        /// Tries to open connection. If fails, throws an exception.
        /// </summary>
        Task TestConnectionAsync();

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> exists.
        /// </summary> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database exists.</returns>
        Task<bool> DatabaseExistsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Checks if the <see cref="CurrentDatabase"/> is empty, i.e. contains no tables.
        /// </summary>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>True if the database contains any tables.</returns>
        Task<bool> DatabaseEmptyAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets a clean copy (i.e. only connection and configuration data, not connection itself) of the ISqlAgent instance 
        /// in order to reuse instance data.
        /// </summary>
        ISqlAgent GetCopy();

        /// <summary>
        /// Gets a default database schema manager (to create or drop database schema, extract schema,
        /// check for schema errors against gauge schema)
        /// </summary>
        ISchemaManager GetDefaultSchemaManager();

        /// <summary>
        /// Gets a default micro ORM service.
        /// </summary>
        MicroOrm.IOrmService GetDefaultOrmService();

        #region Transactions

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> method, CancellationToken cancellationToken);

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        Task ExecuteInTransactionAsync(Func<SqlAgentBase, CancellationToken, Task> method, CancellationToken cancellationToken);

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="parameter">a custom parameter to pass to the method</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        Task ExecuteInTransactionAsync<T>(Func<SqlAgentBase, T, CancellationToken, Task> method, T parameter, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>  
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> method, CancellationToken cancellationToken);

        /// <summary>
        /// Executes given <paramref name="method">method</paramref> within an SQL transaction
        /// and returns the result of the method.
        /// Invokes Commit if the method execution is succesfull and the transaction was initiated
        /// by the invoker.
        /// </summary>
        /// <param name="method">a method to execute within an SQL transaction</param>
        /// <param name="cancellationToken">a cancelation token (if any); it is not used by the transaction
        /// infrastructure, only passed to the method that might use it and throw OperationCanceledException</param>
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<SqlAgentBase, CancellationToken, Task<TResult>> method,
            CancellationToken cancellationToken);

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
        Task<TResult> ExecuteInTransactionAsync<T, TResult>(Func<SqlAgentBase, T, CancellationToken, Task<TResult>> method, 
            T parameter, CancellationToken cancellationToken);

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
        Task<LightDataTable> FetchTableAsync(string token, SqlParam[] parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Fetches multiple data tables using SQL query tokens in the SQL repository.
        /// </summary>
        /// <param name="queries">an array of queries defined by tokens in the SQL repository
        /// and collections of SQL query parameters (null or empty array for none).</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>an array of <see cref="LightDataTable">LightDataTables</see> that contain
        /// data returned by the SQL queries.</returns>
        Task<LightDataTable[]> FetchTablesAsync((string Token, SqlParam[] Parameters)[] queries, 
            CancellationToken cancellationToken);

        /// <summary>
        /// Fetches data using raw SQL query (i.e. without using an SQL dictionary tokens).
        /// </summary>
        /// <param name="sqlQuery">an SQL query to execute</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>   
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// data returned by the SQL query.</returns>
        Task<LightDataTable> FetchTableRawAsync(string sqlQuery, SqlParam[] parameters, CancellationToken cancellationToken);

        /// <summary>
        /// Fetches the specified fields from the specified database table.
        /// </summary>
        /// <param name="table">the name of the table to fetch the fields for</param>
        /// <param name="fields">a collection of the names of the fields to fetch</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        /// <returns>a <see cref="LightDataTable">LightDataTable</see> that contains
        /// specified fields data in the specified table.</returns>
        Task<LightDataTable> FetchTableFieldsAsync(string table, string[] fields, CancellationToken cancellationToken);

        /// <summary>
        /// Executes an SQL statement, that inserts a new row, using an SQL token 
        /// in the SQL repository and returns last insert id.
        /// </summary>
        /// <param name="insertStatementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        Task<Int64> ExecuteInsertAsync(string insertStatementToken, SqlParam[] parameters);
        
        /// <summary>
        /// Executes a raw SQL statement, that inserts a new row, and returns last insert id.
        /// </summary>
        /// <param name="insertStatement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>last insert id</returns>
        Task<Int64> ExecuteInsertRawAsync(string insertStatement, SqlParam[] parameters);

        /// <summary>
        /// Executes an SQL statement using SQL query token in the SQL repository 
        /// and returns affected rows count.
        /// </summary>
        /// <param name="statementToken">a token of the SQL statement in the SQL repository</param>
        /// <param name="parameters">a collection of the SQL query parameters (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        Task<int> ExecuteCommandAsync(string statementToken, SqlParam[] parameters);

        /// <summary>
        /// Executes a raw SQL statement and returns affected rows count.
        /// </summary>
        /// <param name="statement">an SQL statement to execute</param>
        /// <param name="parameters">a collection of the SQL statement parameters (null or empty array for none)</param>
        /// <returns>affected rows count</returns>
        Task<int> ExecuteCommandRawAsync(string statement, SqlParam[] parameters);

        /// <summary>
        /// Executes multiple SQL statements using one database connection but without a transaction.
        /// </summary>
        /// <param name="statements">a collection of the SQL statements to execute in batch</param>
        /// <remarks>Used when modifying databases and in other cases when transactions are not supported
        /// in order to reuse a connection.</remarks>
        Task ExecuteCommandBatchAsync(string[] statements);

        #endregion
                 
    }

}
