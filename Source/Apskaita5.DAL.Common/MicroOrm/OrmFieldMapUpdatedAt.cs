using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// describes how a business object audit property (field) for last update timestamp is persisted in a database;
    /// should be added to a business type as a static field
    /// </summary>
    /// <typeparam name="T">a type of the business object that the field belongs to</typeparam>
    public sealed class OrmFieldMapUpdatedAt<T> : OrmFieldMapBase<T> where T : class
    {

        /// <summary>
        /// create a new description how a business object audit property (field) for last update timestamp is persisted in a database;
        /// </summary>
        /// <param name="dbFieldName">a name of the database table field that the property value is persisted in</param>
        /// <param name="propName">a name of the the property that the value is managed by; match LightDataColumn name 
        /// that is returned on business object field</param>
        /// /// <param name="valueGetter">a function to get a current value of the field</param>
        /// <param name="valueSetter">a function to set a current value of the field; the value is stored in the column
        /// with a name specified in the PropName property</param>
        /// <param name="persistenceType">a value indicating how a field value is persisted in the database</param>
        /// <param name="updateScope">an update scope that updates the property value in database.
        /// Update scopes are application defined enums that convert nicely to int, e.g. Financial, Depreciation etc.
        /// If no scope is assigned the field value is updated for every scope.</param>
        /// <param name="isInitializable">a value indicating whether the property value should be set (initialized) 
        /// from the init query result</param>
        public OrmFieldMapUpdatedAt(string dbFieldName, string propName, Action<T, DateTime> valueSetter, Func<T, DateTime> valueGetter)
            : base(dbFieldName, propName, FieldPersistenceType.InsertAndUpdate, null, false)
        {
            ValueGetter = valueGetter ?? throw new ArgumentNullException(nameof(valueGetter));
            ValueSetter = valueSetter ?? throw new ArgumentNullException(nameof(valueSetter));
        }


        private Func<T, DateTime> ValueGetter { get; }
        internal Action<T, DateTime> ValueSetter { get; }


        internal override SqlParam GetParam(T instance)
        {
            return new SqlParam(DbFieldName, ValueGetter(instance).ToUniversalTime());
        }

        internal override void SetValue(T instance, LightDataRow row)
        {
            ValueSetter(instance, DateTime.SpecifyKind(row.GetDateTime(PropName), DateTimeKind.Utc));
        }

        internal void InitValue(T instance)
        {
            ValueSetter(instance, Utilities.GetCurrentTimeStamp());
        }

    }
}
