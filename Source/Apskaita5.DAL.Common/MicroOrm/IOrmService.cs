using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// Provides an abstraction over micro ORM functionality for DI.
    /// </summary>
    public interface IOrmService
    {

        /// <summary>
        /// Gets an SqlAgent used by the service, e.g. to initiate a transaction.
        /// </summary>
        ISqlAgent Agent { get; }


        /// <summary>
        /// Fetches a business object of type T by its primary key using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch</typeparam>
        /// <param name="id">primary key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<T> FetchEntityAsync<T>(object id, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches a list of child business objects of type T by parent entity id using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a (child) business objects to fetch</typeparam>
        /// <param name="parentId">parent key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<List<T>> FetchChildEntitiesAsync<T>(object parentId, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches a list of all of the business objects of type T using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a business objects to fetch</typeparam>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<List<T>> FetchAllEntitiesAsync<T>(CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches data for a business object of type T by its primary key using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the data for</typeparam>
        /// <param name="id">primary key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<LightDataTable> FetchTableAsync<T>(object id, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches data for a business object of type T by parent entity id using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the data for</typeparam>
        /// <param name="parentId">parent key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<LightDataTable> FetchTableByParentAsync<T>(object parentId, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches data for all of the business objects of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of business objects to fetch the data for</typeparam>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<LightDataTable> FetchTableForAllAsync<T>(CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Loads business object's fields with data from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to load the data for</typeparam>
        /// <param name="instance">an instance of the business object to load the fields for</param>
        /// <param name="id">primary key value</param>  
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task LoadObjectFieldsAsync<T>(T instance, object id, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Loads business object's fields with data from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to load the data for</typeparam>
        /// <param name="instance">an instance of the business object to load the fields for</param>
        /// <param name="databaseRow">data that was fetched either by 
        /// <see cref="FetchTableAsync{T}(object, bool, CancellationToken)"/>
        /// or <see cref="FetchAllAsync{T}(CancellationToken)"/>.</param>
        void LoadObjectFields<T>(T instance, LightDataRow databaseRow) where T : class;

        /// <summary>
        /// Creates a new business object of type T and initializes it with the database data using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch</typeparam>
        /// <param name="parameters">parameters for the initialization query defined by <see cref="OrmIdentityMap{T}.InitQueryToken"/></param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<T> InitEntityAsync<T>(SqlParam[] parameters, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Fetches data for initialization of a business object of type T.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the initialization data for</typeparam>
        /// <param name="parameters">parameters for the init query defined by <see cref="OrmIdentityMap.InitQueryToken"/></param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task<LightDataTable> FetchInitTableAsync<T>(SqlParam[] parameters, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Initializes new business object's fields with data from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to initialize the data for</typeparam>
        /// <param name="instance">an instance of the business object to initialize the fields for</param>
        /// <param name="parameters">parameters for the init query defined by <see cref="OrmIdentityMap.InitQueryToken"/></param>  
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        Task InitObjectFieldsAsync<T>(T instance, SqlParam[] parameters, CancellationToken cancellationToken) where T : class;

        /// <summary>
        /// Initializes new business object's fields with data from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to init the data for</typeparam>
        /// <param name="instance">an instance of the business object to init the fields for</param>
        /// <param name="databaseRow">data that was fetched by 
        /// <see cref="FetchInitTableAsync{T}(SqlParam[], CancellationToken)"/>.</param>
        void InitObjectFields<T>(T instance, LightDataRow databaseRow) where T : class;

        /// <summary>
        /// Inserts properties of a business object of type T into the database using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to insert</typeparam>
        /// <param name="instance">an instance of business object to insert</param>
        /// <param name="userId">a user identificator for audit field InsertedBy 
        /// (only applicable if the entity implements standard audit fields)</param>
        /// <param name="extraParameters">extra parameters for insert if some of the business object data
        /// fields are not reflected as properties, e.g. operation type, parent id etc.;
        /// this kind of fields shall be insert only; name of a parameter for such a field shall
        /// match database field name</param>
        Task ExecuteInsertAsync<T>(T instance, string userId = null, SqlParam[] extraParameters = null) where T : class;

        /// <summary>
        /// Updates properties of a business object of type T in the database using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to update</typeparam>
        /// <param name="instance">an instance of business object to update</param>
        /// <param name="userId">a user identificator for audit field UpdatedBy 
        /// (only applicable if the entity implements standard audit fields)</param>
        /// <param name="scope">a scope of the update operation; a business objects can define
        /// different update scopes (different collections of properties) as an ENUM
        /// which nicely converts into int.</param>
        Task<int> ExecuteUpdateAsync<T>(T instance, string userId = null, int? scope = null) where T : class;

        /// <summary>
        /// Deletes a business object of type T from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to delete</typeparam>
        /// <param name="instance">an instance of the business object to delete</param>
        Task<int> ExecuteDeleteAsync<T>(T instance) where T : class;

        /// <summary>
        /// Deletes a business object of type T from the database.
        /// </summary>
        /// <typeparam name="T">a type of a business object to delete</typeparam>
        /// <param name="primaryKey">a primary key value of the business object to delete</param>
        Task<int> ExecuteDeleteAsync<T>(object primaryKey) where T : class;

    }
}
