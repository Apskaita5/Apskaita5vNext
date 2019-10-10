using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// Describes how a business entity (identity plus fields) is persisted in a database table.
    /// </summary>
    /// <typeparam name="T">a type of the business object that is described</typeparam>
    /// <remarks>Only meant for use by <see cref="SqlAgentBase"/> implementations.</remarks>
    public class OrmEntityMap<T> where T : class
    {

        private readonly OrmIdentityMap<T> _identity;
        private readonly List<OrmFieldMap<T>> _fields;        


        /// <summary>
        /// Creates a new mapping description for a type of business object.
        /// </summary>
        public OrmEntityMap()
        {

            var staticFieldsInfo = typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

            var identityFieldInfo = staticFieldsInfo.FirstOrDefault(f => f.FieldType == typeof(OrmIdentityMap<T>));
            if (identityFieldInfo.IsNull()) throw new NotSupportedException(string.Format(
                Properties.Resources.MicroOrmIsNotSupportedByTypeException, typeof(T).FullName));
            _identity = (OrmIdentityMap<T>)identityFieldInfo.GetValue(null);

            _fields = staticFieldsInfo.Where(f => f.FieldType == typeof(OrmFieldMap<T>))
                .Select(f => (OrmFieldMap<T>)f.GetValue(null)).ToList();

            if (null == _fields || _fields.Count < 1) throw new NotSupportedException(
                string.Format(Properties.Resources.NoFieldMapsForMicroOrmException, typeof(T).FullName));

            _updateStatements = new ConcurrentDictionary<int, string>(Environment.ProcessorCount * 2, 10);

        }


        #region Info Props        

        /// <summary>
        /// Gets a name of the database table that the business object is persisted in.
        /// </summary>
        public string TableName => _identity.TableName; 

        /// <summary>
        /// Gets a name of the database table field that persists business object parent id value.
        /// Used only for select by parent id functionality.
        /// </summary>
        public string ParentIdFieldName => _identity.ParentIdFieldName; 

        /// <summary>
        /// Gets a primary key field name.
        /// </summary>
        public string PrimaryKeyFieldName => _identity.PrimaryKeyFieldName;

        /// <summary>
        /// Gets a fetch query token if custom query should be used.
        /// </summary>
        public string FetchQueryToken => _identity.FetchQueryToken;

        /// <summary>
        /// Gets a fetch by parent id query token if a custom query should be used.
        /// </summary>
        public string FetchByParentIdQueryToken => _identity.FetchByParentIdQueryToken;

        /// <summary>
        /// Gets a fetch all (table) query token if a custom query should be used.
        /// </summary>
        public string FetchAllQueryToken => _identity.FetchAllQueryToken;

        /// <summary>
        /// Gets an init query token to fetch initial values for a new business object (if init required).
        /// </summary>
        public string InitQueryToken => _identity.InitQueryToken;

        #endregion

        #region SqlAgent Managed Props And Methods

        private string _selectQuery;
        private string _selectByParentIdQuery;
        private string _selectAllQuery;
        private string _insertStatement;
        private string _deleteStatement;
        private readonly ConcurrentDictionary<int, string> _updateStatements;
        private string _defaultUpdateStatement;

        /// <summary>
        /// Gets or sets a select query used to fetch object's field values by its primary key.
        /// </summary>
        /// <param name="selectQueryFactory">a factory method to create a select by primary key query</param>
        public string GetOrAddSelectQuery(Func<OrmEntityMap<T>, string> selectQueryFactory)
        {
            if (null == selectQueryFactory) throw new ArgumentNullException(nameof(selectQueryFactory));
            if (_selectQuery.IsNullOrWhiteSpace()) _selectQuery = selectQueryFactory(this);
            return _selectQuery;
        }

        /// <summary>
        /// Gets or sets a select query used to fetch object's field values by its parent id.
        /// </summary>
        /// <param name="selectByParentIdQueryFactory">a factory method to create a select by parent id query</param>
        public string GetOrAddSelectByParentIdQuery(Func<OrmEntityMap<T>, string> selectByParentIdQueryFactory)
        {
            if (null == selectByParentIdQueryFactory) throw new ArgumentNullException(nameof(selectByParentIdQueryFactory));
            if (_selectByParentIdQuery.IsNullOrWhiteSpace()) _selectByParentIdQuery = selectByParentIdQueryFactory(this);
            return _selectByParentIdQuery;
        }

        /// <summary>
        /// Gets or sets a select query used to fetch object field values for all of the objects of type T in the database.
        /// </summary>
        /// <param name="selectAllQueryFactory">a factory method to create a select all query</param>
        public string GetOrAddSelectAllQuery(Func<OrmEntityMap<T>, string> selectAllQueryFactory)
        {
            if (null == selectAllQueryFactory) throw new ArgumentNullException(nameof(selectAllQueryFactory));
            if (_selectAllQuery.IsNullOrWhiteSpace()) _selectAllQuery = selectAllQueryFactory(this);
            return _selectAllQuery;
        }

        /// <summary>
        /// Gets or sets an insert statement used to insert object's field values into the database.
        /// </summary>
        /// <param name="insertStatementFactory">a factory method to create an insert statement</param>
        public string GetOrAddInsertStatement(Func<OrmEntityMap<T>, SqlParam[], string> insertStatementFactory, 
            SqlParam[] extraParameters)
        {
            if (null == insertStatementFactory) throw new ArgumentNullException(nameof(insertStatementFactory));
            if (_insertStatement.IsNullOrWhiteSpace()) _insertStatement = insertStatementFactory(this, extraParameters);
            return _insertStatement;
        }

        /// <summary>
        /// Gets or sets a delete statement used to delete object from the database.
        /// </summary>
        /// <param name="deleteStatementFactory">a factory method to create a delete statement</param>
        public string GetOrAddDeleteStatement(Func<OrmEntityMap<T>, string> deleteStatementFactory)
        {
            if (null == deleteStatementFactory) throw new ArgumentNullException(nameof(deleteStatementFactory));
            if (_deleteStatement.IsNullOrWhiteSpace()) _deleteStatement = deleteStatementFactory(this);
            return _deleteStatement;
        }

        /// <summary>
        /// Gets an update statement for given scopes. If there is no update statement for given scopes,
        /// adds a statement using the given update statement factory method.
        /// </summary>
        /// <param name="scope">a <see cref="FieldMap{T}.UpdateScopes">scope</see> of the update operation</param>
        /// <param name="updateStatementFactory">a method to create a new update statement</param>
        public string GetOrAddUpdateStatement(int? scope, Func<OrmEntityMap<T>, int?, string> updateStatementFactory)
        {
            if (updateStatementFactory.IsNull()) throw new ArgumentNullException(nameof(updateStatementFactory));
            if (!scope.HasValue)
            {
                if (_defaultUpdateStatement.IsNullOrWhiteSpace())
                    _defaultUpdateStatement = updateStatementFactory(this, scope);
                return _defaultUpdateStatement;
            }
            return _updateStatements.GetOrAdd(scope.Value, s => updateStatementFactory(this, scope));
        }

        #endregion

        #region Mapping Methods

        //SELECT

        /// <summary>
        /// Gets a collection of database field name and associated property name pairs for trivial select operation.
        /// Use it to format select query in db_field_name AS PropertyName style.
        /// </summary>
        public (string DbFieldName, string PropName)[] GetFieldsForSelect()
        {
            var result = new List<(string DbFieldName, string PropName)>()
            { (DbFieldName : _identity.PrimaryKeyFieldName, PropName: _identity.PrimaryKeyPropName) };
            result.AddRange(_fields.Select(f => f.GetSelectField()));
            return result.ToArray();
        }

        //INSERT

        /// <summary>
        /// Gets a collection of database table fields for insert operation.
        /// </summary>
        public string[] GetFieldsForInsert()
        {
            var result = _fields.Where(f => f.PersistenceType != FieldPersistenceType.Readonly).
                Select(f => f.DbFieldName).ToList();
            if (!_identity.PrimaryKeyAutoIncrement) result.Add(_identity.PrimaryKeyFieldName);
            return result.ToArray();
        }

        /// <summary>
        /// Gets a collection of <see cref="SqlParam">SqlParams</see> for insert statement.
        /// </summary>
        /// <param name="instance">an instance of the business object to get the param values for</param>
        public SqlParam[] GetParamsForInsert(T instance, SqlParam[] extraParameters = null)
        {

            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));

            var result = _fields.Where(f => f.PersistenceType != FieldPersistenceType.Readonly).
                Select(f => f.GetParam(instance)).ToList();
            if (!_identity.PrimaryKeyAutoIncrement) result.Add(GetPrimaryKeyParam(instance));
            if (null != extraParameters) result.AddRange(extraParameters);

            return result.ToArray();
        }

        /// <summary>
        /// Sets a new primary key that was returned by the database after insert.
        /// </summary>
        /// <param name="instance">an instance of the business object to set the primary key for</param>
        /// <param name="newPrimaryKey">new primary key value</param>
        internal void SetNewPrimaryKey(T instance, long newPrimaryKey)
        {
            if (_identity.PrimaryKeyAutoIncrement) _identity.NewPrimaryKeySetter(instance, newPrimaryKey);
        }

        //UPDATE       

        /// <summary>
        /// Gets a collection of database table fields for update operation of a particular scope.
        /// </summary>
        /// <param name="scope">a <see cref="OrmFieldMap{T}.UpdateScope">scope</see> of the update operation;
        /// use null for full update</param>
        public string[] GetFieldsForUpdate(int? scope)
        {
            return _fields.Where(f => f.IsInUpdateScope(scope, _identity.ScopeIsFlag)).Select(f => f.DbFieldName).ToArray();
        }

        /// <summary>
        /// Gets a collection of <see cref="SqlParam">SqlParams</see> for update statement.
        /// </summary>
        /// <param name="instance">an instance of the business object to get the params for</param>
        /// <param name="scope">a <see cref="OrmFieldMap{T}.UpdateScopes">scope</see> of the update operation;
        /// use null for full update</param>
        public SqlParam[] GetParamsForUpdate(T instance, int? scope)
        {
            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));

            var result = _fields.Where(f => f.IsInUpdateScope(scope, _identity.ScopeIsFlag))
                .Select(f => f.GetParam(instance)).ToList();
            result.Add(GetPrimaryKeyParam(instance));

            return result.ToArray();
        }

        //DELETE

        /// <summary>
        /// Gets a collection of <see cref="SqlParam">SqlParams</see> for delete statement,
        /// i.e. single param for primary key in array.
        /// </summary>
        /// <param name="instance">an instance of the business object to get the params for</param>
        public SqlParam[] GetParamsForDelete(T instance)
        {
            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            return new SqlParam[] { GetPrimaryKeyParam(instance) };
        }

        //LOAD

        /// <summary>
        /// Loads a business object field values using data fetched from the database.
        /// </summary>
        /// <param name="instance">an instance of business object to load the values for</param>
        /// <param name="row">data fetched from the database by an autogenerated fetch query or
        /// a query defined by <see cref="FetchQueryToken"/></param>
        public void LoadValues(T instance, LightDataRow row)
        {   
            foreach (var f in _fields) f.SetValue(instance, row);
            _identity.PrimaryKeySetter(instance, row);
        }

        /// <summary>
        /// Initializes a business object field values using data fetched from the database.
        /// </summary>
        /// <param name="instance">an instance of business object to initialize the values for</param>
        /// <param name="row">data fetched from the database by a query defined by <see cref="InitQueryToken"/></param>
        public void InitValues(T instance, LightDataRow row)
        {
            foreach (var f in _fields.Where(f => f.IsInitializable)) f.SetValue(instance, row);
        }

        #endregion

        private SqlParam GetPrimaryKeyParam(T instance)
        {
            return new SqlParam(_identity.PrimaryKeyFieldName, _identity.PrimaryKeyGetter(instance));
        }

    }

}
