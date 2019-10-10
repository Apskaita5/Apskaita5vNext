using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// describes how a business object property (field) is persisted in a database;
    /// should be added to a business type as a static field
    /// </summary>
    /// <typeparam name="T">a type of the business object that the field belongs to</typeparam>
    public class OrmFieldMap<T> where T : class
    {
        
        /// <summary>
        /// create a new description how a business object property (field) is persisted in a database;
        /// should be added to a business type as a static field
        /// </summary>
        /// <param name="dbFieldName">a name of the database table field that the property value is persisted in</param>
        /// <param name="propName">a name of the the property that the value is managed by; match LightDataColumn name 
        /// that is returned on business object field</param>
        /// <param name="valueType">a type of value in the database, i.e. as passed to SqlParam</param>
        /// <param name="valueGetter">a function to get a current value of the field</param>
        /// <param name="valueSetter">a function to set a current value of the field; the value is stored in the column
        /// with a name specified in the PropName property</param>
        /// <param name="persistenceType">a value indicating how a field value is persisted in the database</param>
        /// <param name="updateScope">an update scope that updates the property value in database.
        /// Update scopes are application defined enums that convert nicely to int, e.g. Financial, Depreciation etc.
        /// If no scope is assigned the field value is updated for every scope.</param>
        /// <param name="isInitializable">a value indicating whether the property value should be set (initialized) 
        /// from the init query result</param>
        public OrmFieldMap(string dbFieldName, string propName, Type valueType, Func<T, object> valueGetter, 
            Action<T, LightDataRow> valueSetter, FieldPersistenceType persistenceType = FieldPersistenceType.InsertAndUpdate, 
            int? updateScope = null, bool isInitializable = false)
        {

            if (dbFieldName.IsNullOrWhiteSpace()) new ArgumentNullException(nameof(dbFieldName));
            if (propName.IsNullOrWhiteSpace()) new ArgumentNullException(nameof(propName));

            DbFieldName = dbFieldName.Trim();
            PropName = propName.Trim();
            ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
            ValueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
            ValueSetter = valueSetter ?? throw new ArgumentNullException(nameof(valueSetter));
            PersistenceType = persistenceType;
            UpdateScope = updateScope;
            IsInitializable = isInitializable;

        }


        /// <summary>
        /// a name of the database table field that the property value is persisted in
        /// </summary>
        public string DbFieldName { get; }

        /// <summary>
        /// a name of the the property that the value is managed by; match LightDataColumn name 
        /// that is returned on business object fetch
        /// </summary>
        public string PropName { get; }

        /// <summary>
        /// a type of value in the database, i.e. as passed to SqlParam
        /// </summary>
        public Type ValueType { get; }

        /// <summary>
        /// a function to get a current value of the field
        /// </summary>
        public Func<T, object> ValueGetter { get; }

        /// <summary>
        /// a function to set a current value of the field; the value is stored in the column
        /// with a name specified in the PropName property
        /// </summary>
        public Action<T, LightDataRow> ValueSetter { get; }

        /// <summary>
        /// a value indicating how a field value is persisted in the database
        /// </summary>
        public FieldPersistenceType PersistenceType { get; }

        /// <summary>
        /// an update scope that updates the property value in database
        /// Update scopes are application defined enums that convert nicely to int, e.g. Financial, Depreciation etc.
        /// If no scope is assigned the field value is updated for every scope.
        /// If multiple scope combinations are used, the enum should be defined as [Flags].
        /// in that case: (a) a field should only be assigned to a single scope;
        /// (b) a bitwise check is used: (fieldScope & requestedScope) != 0;
        /// (c) <see cref="OrmIdentityMap{T}.ScopeIsFlag"/> should be set to true.
        /// </summary>
        public int? UpdateScope { get; }

        /// <summary>
        /// a value indicating whether the property value should be set (initialized) from the init query result
        /// </summary>
        public bool IsInitializable { get; }


        #region Internal Mapping Methods        

        /// <summary>
        /// Gets an <see cref="SqlParam">SQL query parameter</see> where DB field name is used as a parameter name
        /// </summary>
        /// <param name="instance">an instance of business object to get a value parameter for</param>
        internal SqlParam GetParam(T instance)
        {
            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            return new SqlParam(DbFieldName, ValueGetter(instance), ValueType);
        }

        /// <summary>
        /// Sets an instance field value from the query result.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="row"></param>
        internal void SetValue(T instance, LightDataRow row)
        {
            if (instance.IsNull()) throw new ArgumentNullException(nameof(instance));
            ValueSetter(instance, row);
        }

        /// <summary>
        /// Gets a database field name and an associated property name.
        /// Use it to format select query in db_field_name AS PropertyName style.
        /// </summary>
        internal (string DbFieldName, string PropName) GetSelectField()
        {
            return (DbFieldName : DbFieldName, PropName : PropName);
        }

        /// <summary>
        /// Gets a value indicating whether the field shall be updated within the scopes given.
        /// </summary>
        /// <param name="scopes">scopes that shall be updated; null scopes => update all updateable fields</param>
        internal bool IsInUpdateScope(int? scope, bool scopeIsFlag)
        {
            return FieldPersistenceType.InsertAndUpdate == PersistenceType && (!scope.HasValue 
                || null == UpdateScope || (scopeIsFlag && ((scope.Value & UpdateScope) != 0))
                || (!scopeIsFlag && scope.Value == UpdateScope));
        }

        #endregion

    }
}
