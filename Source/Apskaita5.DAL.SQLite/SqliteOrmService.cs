using System;
using System.Collections.Generic;
using System.Linq;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.MicroOrm;
using static Apskaita5.DAL.SQLite.Constants;

namespace Apskaita5.DAL.SQLite
{
    /// <summary>
    /// An SQLite implementation of micro ORM.
    /// </summary>
    public class SqliteOrmService : OrmServiceBase
    {

        public override string SqlImplementationId => SqliteImplementationId;


        /// <summary>
        /// Creates a new instrance of SQLite micro ORM service.
        /// </summary>
        /// <param name="agent">an SQLite agent to use for ORM services</param>
        public SqliteOrmService(SqliteAgent agent) : base(agent) { }


        protected override string GetSelectByParentIdQuery<T>(OrmEntityMap<T> map)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f =>
                string.Format("{0} AS {1}", f.DbFieldName.ToConventional(Agent), f.PropName));

            return string.Format("SELECT {0} FROM {1} WHERE {2}={3};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent), map.ParentIdFieldName.ToConventional(Agent),
                ParamPrefix + map.ParentIdFieldName);
        }

        protected override string GetSelectQuery<T>(OrmEntityMap<T> map)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName));

            return string.Format("SELECT {0} FROM {1} WHERE {2}={3};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent), map.PrimaryKeyFieldName.ToConventional(Agent),
                ParamPrefix + map.PrimaryKeyFieldName);
        }

        protected override string GetSelectAllQuery<T>(OrmEntityMap<T> map)
        {
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName));

            return string.Format("SELECT {0} FROM {1};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent));
        }

        protected override string GetInsertStatement<T>(OrmEntityMap<T> map, SqlParam[] extraParams)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var propList = new List<string>(map.GetFieldsForInsert());
            if (extraParams != null) propList.AddRange(extraParams.Select(p => p.Name.Trim()));

            var fields = string.Join(", ", propList.Select(p => p.ToConventional(Agent)).ToArray());
            var parameters = string.Join(", ", propList.Select(p => ParamPrefix + p).ToArray());

            return string.Format("INSERT INTO {0}({1}) VALUES({2});", map.TableName.ToConventional(Agent), fields, parameters);
        }

        protected override string GetUpdateStatement<T>(OrmEntityMap<T> map, int? scope)
        {
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForUpdate(scope).Select(f => string.Format("{0}={1}",
                f.ToConventional(Agent), ParamPrefix + f));

            return string.Format("UPDATE {0} SET {1} WHERE {2}={3};", map.TableName.ToConventional(Agent),
                string.Join(", ", fields), map.PrimaryKeyFieldName.ToConventional(Agent),
                ParamPrefix + map.PrimaryKeyFieldName);
        }

        protected override string GetDeleteStatement<T>(OrmEntityMap<T> map)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            return string.Format("DELETE FROM {0} WHERE {0}.{1}={2};", map.TableName.ToConventional(Agent),
                map.PrimaryKeyFieldName.ToConventional(Agent), ParamPrefix + map.PrimaryKeyFieldName);

        }

    }
}
