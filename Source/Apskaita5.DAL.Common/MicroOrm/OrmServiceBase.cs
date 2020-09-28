using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// A base class for a concrete SQL implementation, e.g. MySql, SQLite, etc.
    /// </summary>
    public abstract class OrmServiceBase : IOrmService
    {

        private ISqlAgent _agent;
        private static ConcurrentDictionary<Type, object> _maps =
            new ConcurrentDictionary<Type, object>(Environment.ProcessorCount * 2, 1000);


        /// <summary>
        /// Gets an id of the concrete SQL implementation, e.g. MySQL, SQLite.
        /// The id is used to make sure that the OrmServiceBase implementation match SqlAgentBase implementation.
        /// </summary>
        public abstract string SqlImplementationId { get; }

        /// <summary>
        /// Gets an instance of an Sql agent to use for queries and statements.
        /// </summary>
        public ISqlAgent Agent => _agent;


        /// <summary>
        /// Creates a new Orm service.
        /// </summary>
        /// <param name="agent">an instance of an Sql agent to use for queries and statements; its implementation
        /// type should match Orm service implementation type</param>
        public OrmServiceBase(ISqlAgent agent)
        {
            _agent = agent ?? throw new ArgumentNullException(nameof(agent));
            if (!_agent.SqlImplementationId.EqualsByConvention(SqlImplementationId))
                throw new ArgumentException(string.Format(Properties.Resources.SqlAgentAndOrmServiceTypeMismatchException,
                    _agent.SqlImplementationId, SqlImplementationId), nameof(agent));
        }


        #region Fetch And Load Methods

        /// <summary>
        /// Fetches a business object of type T by its primary key using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch</typeparam>
        /// <param name="id">primary key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task<T> FetchEntityAsync<T>(object id, CancellationToken cancellationToken) where T : class
        {

            if (id.IsNull()) throw new ArgumentNullException(nameof(id));

            var data = await FetchTableAsync<T>(id, cancellationToken).ConfigureAwait(false);
            if (data.Rows.Count < 1) throw new EntityNotFoundException(typeof(T), id.ToString());

            var map = GetOrCreateMap<T>();

            return map.LoadInstance(data.Rows[0]);

        }

        /// <summary>
        /// Fetches a list of child business objects of type T by parent entity id using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a (child) business objects to fetch</typeparam>
        /// <param name="parentId">parent key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task<List<T>> FetchChildEntitiesAsync<T>(object parentId, CancellationToken cancellationToken) where T : class
        {
            var data = await FetchTableByParentAsync<T>(parentId, cancellationToken).ConfigureAwait(false);

            var map = GetOrCreateMap<T>();
            var result = new List<T>();

            foreach (var row in data.Rows)
            {
                result.Add(map.LoadInstance(row));
            }

            return result;   
        }

        /// <summary>
        /// Fetches a list of all of the business objects of type T using integrated micro ORM.
        /// (Activator.CreateInstance is used to instantiate the business object)
        /// </summary>
        /// <typeparam name="T">a type of a business objects to fetch</typeparam>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task<List<T>> FetchAllEntitiesAsync<T>(CancellationToken cancellationToken) where T : class
        {

            var data = await FetchTableForAllAsync<T>(cancellationToken).ConfigureAwait(false);

            var map = GetOrCreateMap<T>();
            var result = new List<T>();

            foreach (var row in data.Rows)
            {
                result.Add(map.LoadInstance(row));
            }

            return result;

        }

        /// <summary>
        /// Fetches data for a business object of type T by its primary key using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the data for</typeparam>
        /// <param name="id">primary key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public Task<LightDataTable> FetchTableAsync<T>(object id, CancellationToken cancellationToken) where T : class
        {

            if (id.IsNull()) throw new ArgumentNullException(nameof(id));

            var map = GetOrCreateMap<T>();  
            
            var parameters = new SqlParam[] { new SqlParam(map.PrimaryKeyFieldName, id) };
            
            if (!map.FetchQueryToken.IsNullOrWhiteSpace())
            {
                return _agent.FetchTableAsync(map.FetchQueryToken, parameters, cancellationToken);
            }

            string query = map.GetOrAddSelectQuery(GetSelectQuery);             
            
            return _agent.FetchTableRawAsync(query, parameters, cancellationToken);

        }

        /// <summary>
        /// Fetches data for a business object of type T by parent entity id using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the data for</typeparam>
        /// <param name="parentId">parent key value</param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public Task<LightDataTable> FetchTableByParentAsync<T>(object parentId, CancellationToken cancellationToken) where T : class
        {
            var map = GetOrCreateMap<T>();

            if (map.ParentIdFieldName.IsNullOrWhiteSpace()) throw new InvalidOperationException(string.Format(
                "Entity {0} is not a child entity and cannot be fetched by a parent id.", typeof(T).FullName));

            SqlParam[] parameters = null; 
            if (!parentId.IsNull()) parameters = new SqlParam[] { new SqlParam(map.ParentIdFieldName, parentId) };

            if (!map.FetchByParentIdQueryToken.IsNullOrWhiteSpace())
            {
                return _agent.FetchTableAsync(map.FetchByParentIdQueryToken, parameters, cancellationToken);
            }

            string query;
            if (parentId.IsNull())
            {
                query = map.GetOrAddSelectByNullParentIdQuery(GetSelectByNullParentIdQuery);
            }
            else
            {
                query = map.GetOrAddSelectByParentIdQuery(GetSelectByParentIdQuery);
            }
            
            return _agent.FetchTableRawAsync(query, parameters, cancellationToken);  
        }

        /// <summary>
        /// Fetches data for all of the business objects of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of business objects to fetch the data for</typeparam>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public Task<LightDataTable> FetchTableForAllAsync<T>(CancellationToken cancellationToken) where T : class
        {       

            var map = GetOrCreateMap<T>();

            if (!map.FetchAllQueryToken.IsNullOrWhiteSpace())
                return _agent.FetchTableAsync(map.FetchAllQueryToken, null, cancellationToken);

            return _agent.FetchTableRawAsync(map.GetOrAddSelectAllQuery(GetSelectAllQuery), null, cancellationToken);

        }

        /// <summary>
        /// Fetches data for a business object of type T and loads its fields with the data using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch the data for</typeparam>
        /// <param name="instance">an instance of the business object to load the data into</param>
        /// <param name="id">primary key value to look for</param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task LoadObjectFieldsAsync<T>(T instance, object id, CancellationToken cancellationToken) where T : class
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            if (id.IsNull()) throw new ArgumentNullException(nameof(id));

            var data = await FetchTableAsync<T>(id, cancellationToken).ConfigureAwait(false);
            if (data.Rows.Count < 1) throw new EntityNotFoundException(typeof(T), id.ToString());

            LoadObjectFields(instance, data.Rows[0]);

        }

        /// <summary>
        /// Loads the fetched data into a business object fields using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to to load the data into</typeparam>
        /// <param name="instance">an instance of the business object to load the data into</param>
        /// <param name="databaseRow">object data that has been fetched from the database</param>
        public void LoadObjectFields<T>(T instance, LightDataRow databaseRow) where T : class
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            if (databaseRow.IsNull()) throw new ArgumentNullException(nameof(databaseRow));

            var map = GetOrCreateMap<T>();

            map.LoadValues(instance, databaseRow);

        }

        /// <summary>
        /// Gets a (trivial) select by parent id statement for business objects of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business objects to get a select statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        protected abstract string GetSelectByParentIdQuery<T>(OrmEntityMap<T> map) where T : class;

        /// <summary>
        /// Gets a (trivial) select by null parent id statement for business objects of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business objects to get a select statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        protected abstract string GetSelectByNullParentIdQuery<T>(OrmEntityMap<T> map) where T : class;

        /// <summary>
        /// Gets a (trivial) select by primary key statement for a business object of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to get a statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        protected abstract string GetSelectQuery<T>(OrmEntityMap<T> map) where T : class;

        /// <summary>
        /// Gets a (trivial) select all (table) query for business objects of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of business objects to get a statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        protected abstract string GetSelectAllQuery<T>(OrmEntityMap<T> map) where T : class;

        #endregion

        #region Initialization Methods

        /// <summary>
        /// Fetches a new business object of type T and initializes it with the database data using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to fetch</typeparam>
        /// <param name="parameters">parameters for the initialization query defined by <see cref="OrmIdentityMap{T}.InitQueryToken"/></param> 
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task<T> InitEntityAsync<T>(SqlParam[] parameters, CancellationToken cancellationToken) where T : class
        {

            var map = GetOrCreateMap<T>();

            if (map.InitQueryToken.IsNullOrWhiteSpace()) throw new NotSupportedException(
                string.Format(Properties.Resources.InitQueryTokenNullException, typeof(T).FullName));

            var data = await _agent.FetchTableAsync(map.InitQueryToken, parameters, cancellationToken);
            if (data.Rows.Count < 1) throw new EntityContextNotFoundException(typeof(T), map.InitQueryToken, parameters);

            return map.InitInstance(data.Rows[0]);

        }

        /// <summary>
        /// Fetches data for a new business object of type T initialization (context) using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of the business object to fetch initialization data for</typeparam>
        /// <param name="parameters">parameters for the initialization query defined by <see cref="OrmIdentityMap{T}.InitQueryToken"/></param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public Task<LightDataTable> FetchInitTableAsync<T>(SqlParam[] parameters, CancellationToken cancellationToken) where T : class
        {

            var map = GetOrCreateMap<T>();

            if (map.InitQueryToken.IsNullOrWhiteSpace()) throw new NotSupportedException(
                string.Format(Properties.Resources.InitQueryTokenNullException, typeof(T).FullName));

            return _agent.FetchTableAsync(map.InitQueryToken, parameters, cancellationToken);

        }

        /// <summary>
        /// Fetches data for a new business object(s) of type T initialization (context) and initializes the appropriate 
        /// business object's fields using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of the business object to fetch initialization data for</typeparam>
        /// <param name="instance">an instance of the business object to initialize fields for</param>
        /// <param name="parameters">parameters for the initialization query defined by <see cref="OrmIdentityMap{T}.InitQueryToken"/></param>
        /// <param name="cancellationToken">a cancelation token (if any)</param>
        public async Task InitObjectFieldsAsync<T>(T instance, SqlParam[] parameters, CancellationToken cancellationToken) where T : class
        {
            var data = await FetchInitTableAsync<T>(parameters, cancellationToken).ConfigureAwait(false);

            if (data.Rows.Count < 1)
            {
                var map = GetOrCreateMap<T>();
                throw new EntityContextNotFoundException(typeof(T), map.InitQueryToken, parameters);
            }

            InitObjectFields(instance, data.Rows[0]);
        }

        /// <summary>
        /// Initializes fields of a new business object(s) of type T using the data fetched. 
        /// </summary>
        /// <typeparam name="T">a type of the business object to initialize fields for</typeparam>
        /// <param name="instance">an instance of the business object to initialize fields for</param>
        /// <param name="databaseRow">object initialization (context) data that has been fetched from the database</param>
        public void InitObjectFields<T>(T instance, LightDataRow databaseRow) where T : class
        {
            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            if (databaseRow.IsNull()) throw new ArgumentNullException(nameof(databaseRow));

            var map = GetOrCreateMap<T>();

            map.InitValues(instance, databaseRow);
        }

        #endregion

        #region Insert Methods

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
        public async Task ExecuteInsertAsync<T>(T instance, string userId = null, SqlParam[] extraParameters = null) where T : class
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));

            var map = GetOrCreateMap<T>();

            map.SetAuditFieldsForInsert(instance, userId);

            if (map.PrimaryKeyAutoIncrement)
            {
                var newPrimaryKey = await _agent.ExecuteInsertRawAsync(map.GetOrAddInsertStatement(
                    GetInsertStatement, extraParameters), map.GetParamsForInsert(instance, extraParameters)).ConfigureAwait(false);
                map.SetPrimaryKeyAutoIncrementValue(instance, newPrimaryKey);
            }
            else
            {
                await _agent.ExecuteInsertRawAsync(map.GetOrAddInsertStatement(GetInsertStatement, extraParameters), 
                    map.GetParamsForInsert(instance, extraParameters)).ConfigureAwait(false);
                map.UpdatePrimaryKey(instance);
            }             

        }

        /// <summary>
        /// Gets a (trivial) insert statement for a business object of type T using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to get a statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        /// <param name="extraParams">extra parameters for insert if some of the business object data
        /// fields are not reflected as properties, e.g. operation type, parent id etc.;
        /// this kind of fields shall be insert only; name of a parameter for such a field shall
        /// match database field name</param>
        protected abstract string GetInsertStatement<T>(OrmEntityMap<T> map, SqlParam[] extraParams) where T : class;

        #endregion

        #region Update Methods

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
        public async Task<int> ExecuteUpdateAsync<T>(T instance, string userId = null, int? scope = null) where T : class
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));

            var map = GetOrCreateMap<T>();

            map.SetAuditFieldsForUpdate(instance, userId);

            var result = await _agent.ExecuteCommandRawAsync(map.GetOrAddUpdateStatement(scope, GetUpdateStatement),
                map.GetParamsForUpdate(instance, scope));

            if (map.PrimaryKeyUpdatable) map.UpdatePrimaryKey(instance);

            return result;

        }

        /// <summary>
        /// Gets an update statement for a business object of type T for a particular update scope. 
        /// </summary>
        /// <typeparam name="T">a type of a business object to get an update statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        /// <param name="scope">a scope of the update operation; a business objects can define
        /// different update scopes (different collections of properties) as an ENUM
        /// which nicely converts into int.</param>
        protected abstract string GetUpdateStatement<T>(OrmEntityMap<T> map, int? scope) where T : class;

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes a business object of type T from the database using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to delete</typeparam>
        /// <param name="instance">an instance of the business object to delete</param>
        public async Task<int> ExecuteDeleteAsync<T>(T instance) where T : class
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));

            var map = GetOrCreateMap<T>();

            var result = await _agent.ExecuteCommandRawAsync(map.GetOrAddDeleteStatement(GetDeleteStatement), 
                map.GetParamsForDelete(instance));

            map.DeletePrimaryKey(instance);

            return result;

        }

        /// <summary>
        /// Deletes a business object of type T from the database using integrated micro ORM.
        /// </summary>
        /// <typeparam name="T">a type of a business object to delete</typeparam>
        /// <param name="primaryKey">a primary key value of the business object to delete</param>
        public Task<int> ExecuteDeleteAsync<T>(object primaryKey) where T : class
        {

            if (primaryKey.IsNull()) throw new ArgumentNullException(nameof(primaryKey));

            var map = GetOrCreateMap<T>();

            return _agent.ExecuteCommandRawAsync(map.GetOrAddDeleteStatement(GetDeleteStatement), 
                new SqlParam[] { new SqlParam(map.PrimaryKeyFieldName, primaryKey) });

        }

        /// <summary>
        /// Gets a delete statement for a business object of type T.
        /// </summary>
        /// <typeparam name="T">a type of a business object to get a delete statement for</typeparam>
        /// <param name="map">a micro ORM map for a business object type</param>
        protected abstract string GetDeleteStatement<T>(OrmEntityMap<T> map) where T : class;

        #endregion

        protected OrmEntityMap<T> GetOrCreateMap<T>() where T : class
        {
            return (OrmEntityMap<T>)_maps.GetOrAdd(typeof(T), type => new OrmEntityMap<T>());
        }

    }
}
