using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    public sealed class OrmIdentityMapParentString<T> : OrmIdentityMapBase<T> where T : class
    {


        public OrmIdentityMapParentString(string tableName, string primaryKeyFieldName,
            string primaryKeyPropName, Func<T> factoryMethod, Func<T, string> primaryKeyGetter, 
            Action<T, string> primaryKeySetter, Func<T, string> updatedPrimaryKeyGetter, 
            Action<T, string> updatedPrimaryKeySetter, string fetchQueryToken = null, 
            string fetchAllQueryToken = null, string initQueryToken = null, bool scopeIsFlag = false)
            : base(tableName, null, primaryKeyFieldName, primaryKeyPropName, false, fetchQueryToken,
                  null, fetchAllQueryToken, initQueryToken, null, scopeIsFlag, factoryMethod)
        {
            PrimaryKeyGetter = primaryKeyGetter ?? throw new ArgumentNullException(nameof(primaryKeyGetter));
            PrimaryKeySetter = primaryKeySetter ?? throw new ArgumentNullException(nameof(primaryKeySetter));
            UpdatedPrimaryKeyGetter = updatedPrimaryKeyGetter ?? throw new ArgumentNullException(nameof(updatedPrimaryKeyGetter));
            UpdatedPrimaryKeySetter = updatedPrimaryKeySetter ?? throw new ArgumentNullException(nameof(updatedPrimaryKeySetter));
            PrimaryKeyUpdatable = true;
        }

        public OrmIdentityMapParentString(string tableName, string primaryKeyFieldName,
            string primaryKeyPropName, Func<T> factoryMethod, Func<T, string> primaryKeyGetter, 
            Action<T, string> primaryKeySetter, string fetchQueryToken = null, string fetchAllQueryToken = null, 
            string initQueryToken = null, bool scopeIsFlag = false)
            : base(tableName, null, primaryKeyFieldName, primaryKeyPropName, false, fetchQueryToken,
                  null, fetchAllQueryToken, initQueryToken, null, scopeIsFlag, factoryMethod)
        {
            PrimaryKeyGetter = primaryKeyGetter ?? throw new ArgumentNullException(nameof(primaryKeyGetter));
            PrimaryKeySetter = primaryKeySetter ?? throw new ArgumentNullException(nameof(primaryKeySetter));
            UpdatedPrimaryKeyGetter = null;
            UpdatedPrimaryKeySetter = null;
            PrimaryKeyUpdatable = false;
        }

        /// <summary>
        /// Gets a primary key value.
        /// </summary>
        public Func<T, string> PrimaryKeyGetter { get; }

        /// <summary>
        /// Sets a primary key value.
        /// </summary>
        public Action<T, string> PrimaryKeySetter { get; }

        /// <summary>
        /// Gets an updated primary key value.
        /// </summary>
        public Func<T, string> UpdatedPrimaryKeyGetter { get; }

        /// <summary>
        /// Sets an updated primary key value.
        /// </summary>
        public Action<T, string> UpdatedPrimaryKeySetter { get; }
        
        public override bool PrimaryKeyUpdatable { get; }


        internal override SqlParam GetPrimaryKeyParamForInsert(T instance)
        {
            var value = UpdatedPrimaryKeyGetter(instance);
            if (value.IsNullOrWhiteSpace()) throw new InvalidOperationException(string.Format(
                "Entity {0} doesn't have a primary key value assigned.", typeof(T).FullName));
            return new SqlParam(PrimaryKeyFieldName, value.Trim());
        }

        internal override SqlParam GetPrimaryKeyParamForUpdateSet(T instance)
        {
            return GetPrimaryKeyParamForInsert(instance);
        }

        internal override SqlParam GetPrimaryKeyParamForUpdateWhere(T instance, string paramName)
        {
            var value = PrimaryKeyGetter(instance);
            if (value.IsNullOrWhiteSpace()) throw new InvalidOperationException(string.Format(
                "Entity {0} doesn't have a primary key, i.e. its a new entity.", typeof(T).FullName));

            if (PrimaryKeyUpdatable) return new SqlParam(paramName, value.Trim());
            else return new SqlParam(PrimaryKeyFieldName, value.Trim());
        }

        internal override void SetPrimaryKeyAutoIncrementValue(T instance, long nid)
        {
            throw new NotSupportedException();
        }

        internal override void LoadPrimaryKeyValue(T instance, LightDataRow row)
        {
            var value = row.GetString(PrimaryKeyPropName);
            PrimaryKeySetter(instance, value);
            if (PrimaryKeyUpdatable) UpdatedPrimaryKeySetter(instance, value);
        }

        internal override void UpdatePrimaryKey(T instance)
        {
            if (PrimaryKeyUpdatable) PrimaryKeySetter(instance, UpdatedPrimaryKeyGetter(instance));
        }

        internal override void DeletePrimaryKey(T instance)
        {
            PrimaryKeySetter(instance, string.Empty);
            if (PrimaryKeyUpdatable) UpdatedPrimaryKeySetter(instance, string.Empty);
        }
    }
}
