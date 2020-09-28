using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// a base class for description of how a business object property (field) is persisted in a database;
    /// concrete implementations per field type should be added to a business type as a static field
    /// </summary>
    /// <typeparam name="T">a type of the business object that the field belongs to</typeparam>
    public abstract class OrmFieldMapBase<T> where T : class
    {

        /// <summary>
        /// create a new description how a business object property (field) is persisted in a database;
        /// </summary>
        /// <param name="dbFieldName">a name of the database table field that the property value is persisted in</param>
        /// <param name="propName">a name of the the property that the value is managed by; match LightDataColumn name 
        /// that is returned on business object field</param>
        /// <param name="persistenceType">a value indicating how a field value is persisted in the database</param>
        /// <param name="updateScope">an update scope that updates the property value in database.
        /// Update scopes are application defined enums that convert nicely to int, e.g. Financial, Depreciation etc.
        /// If no scope is assigned the field value is updated for every scope.</param>
        /// <param name="isInitializable">a value indicating whether the property value should be set (initialized) 
        /// from the init query result</param>
        protected OrmFieldMapBase(string dbFieldName, string propName, FieldPersistenceType persistenceType,
            int? updateScope = null, bool isInitializable = false)
        {

            if (dbFieldName.IsNullOrWhiteSpace()) new ArgumentNullException(nameof(dbFieldName));
            if (propName.IsNullOrWhiteSpace()) new ArgumentNullException(nameof(propName));

            DbFieldName = dbFieldName.Trim();
            PropName = propName.Trim();
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
        internal abstract SqlParam GetParam(T instance);

        /// <summary>
        /// Sets an instance field value from the query result.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="row"></param>
        internal abstract void SetValue(T instance, LightDataRow row);

        /// <summary>
        /// Gets a database field name and an associated property name.
        /// Use it to format select query in db_field_name AS PropertyName style.
        /// </summary>
        internal (string DbFieldName, string PropName) GetSelectField()
        {
            return (DbFieldName: DbFieldName, PropName: PropName);
        }

        /// <summary>
        /// Gets a value indicating whether the field shall be updated within the scopes given.
        /// </summary>
        /// <param name="scopes">scopes that shall be updated; null scopes => update all updateable fields</param>
        internal bool IsInUpdateScope(int? scope, bool scopeIsFlag)
        {
            if (FieldPersistenceType.InsertAndUpdate != PersistenceType) return false;
            return !scope.HasValue || null == UpdateScope || (scopeIsFlag && ((scope.Value & UpdateScope) != 0))
                || (!scopeIsFlag && scope.Value == UpdateScope);
        }

        #endregion

    }
}
