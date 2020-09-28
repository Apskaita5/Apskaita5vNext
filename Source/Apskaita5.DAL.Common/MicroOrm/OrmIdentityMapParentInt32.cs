using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    public sealed class OrmIdentityMapParentInt32<T> : OrmIdentityMapBase<T> where T : class
    {


        public OrmIdentityMapParentInt32(string tableName, string primaryKeyFieldName,
            string primaryKeyPropName, Func<T> factoryMethod, Func<T, int?> primaryKeyGetter, 
            Action<T, int?> primaryKeySetter, Func<T, int?> updatedPrimaryKeyGetter,
            Action<T, int?> updatedPrimaryKeySetter, string fetchQueryToken = null, 
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

        public OrmIdentityMapParentInt32(string tableName, string primaryKeyFieldName,
            string primaryKeyPropName, Func<T> factoryMethod, Func<T, int?> primaryKeyGetter, Action<T, int?> primaryKeySetter,
            string fetchQueryToken = null, string fetchAllQueryToken = null, string initQueryToken = null,
            bool scopeIsFlag = false)
            : base(tableName, null, primaryKeyFieldName, primaryKeyPropName, false, fetchQueryToken,
                  null, fetchAllQueryToken, initQueryToken, null, scopeIsFlag, factoryMethod)
        {
            PrimaryKeyGetter = primaryKeyGetter ?? throw new ArgumentNullException(nameof(primaryKeyGetter));
            PrimaryKeySetter = primaryKeySetter ?? throw new ArgumentNullException(nameof(primaryKeySetter));
            PrimaryKeyUpdatable = false;
        }


        /// <summary>
        /// Gets a primary key value.
        /// </summary>
        public Func<T, int?> PrimaryKeyGetter { get; }

        /// <summary>
        /// Sets a primary key value.
        /// </summary>
        public Action<T, int?> PrimaryKeySetter { get; }


        /// <summary>
        /// Gets an updated primary key value.
        /// </summary>
        public Func<T, int?> UpdatedPrimaryKeyGetter { get; }

        /// <summary>
        /// Sets an updated primary key value.
        /// </summary>
        public Action<T, int?> UpdatedPrimaryKeySetter { get; }

        public override bool PrimaryKeyUpdatable { get; }


        internal override SqlParam GetPrimaryKeyParamForInsert(T instance)
        {
            Func<T, int?> getter;
            if (PrimaryKeyUpdatable) getter = UpdatedPrimaryKeyGetter;
            else getter = PrimaryKeyGetter;
            var value = getter(instance);
            if (!value.HasValue) throw new InvalidOperationException(string.Format(
                "Entity {0} doesn't have a primary key value assigned.", typeof(T).FullName));
            return new SqlParam(PrimaryKeyFieldName, value.Value);
        }

        internal override SqlParam GetPrimaryKeyParamForUpdateSet(T instance)
        {
            return GetPrimaryKeyParamForInsert(instance);
        }

        internal override SqlParam GetPrimaryKeyParamForUpdateWhere(T instance, string paramName)
        {   
            var value = PrimaryKeyGetter(instance);
            if (!value.HasValue) throw new InvalidOperationException(string.Format(
                "Entity {0} doesn't have a primary key value assigned.", typeof(T).FullName));

            if (PrimaryKeyUpdatable) return new SqlParam(paramName, value.Value);
            else return new SqlParam(PrimaryKeyFieldName, value.Value);  
        }

        internal override void SetPrimaryKeyAutoIncrementValue(T instance, long nid)
        {
            throw new NotSupportedException();
        }

        internal override void LoadPrimaryKeyValue(T instance, LightDataRow row)
        {
            var value = row.GetInt32(PrimaryKeyPropName);
            PrimaryKeySetter(instance, value);
            if (PrimaryKeyUpdatable) UpdatedPrimaryKeySetter(instance, value);
        }

        internal override void UpdatePrimaryKey(T instance)
        {
            if (PrimaryKeyUpdatable) PrimaryKeySetter(instance, UpdatedPrimaryKeyGetter(instance));
        }

        internal override void DeletePrimaryKey(T instance)
        {
            PrimaryKeySetter(instance, null);
            if (PrimaryKeyUpdatable) UpdatedPrimaryKeySetter(instance, null);
        }
                
    }
}
