using System;
using System.Collections.Generic;
using System.Linq;
using Apskaita5.DAL.Common;
using Apskaita5.DAL.Common.MicroOrm;
using static Apskaita5.DAL.MySql.Constants;

namespace Apskaita5.DAL.MySql
{
    /// <summary>
    /// MySql implementation of micro ORM.
    /// </summary>
    public class MySqlOrmService : OrmServiceBase
    {

        public override string SqlImplementationId => MySqlImplementationId;


        /// <summary>
        /// Creates a new micro ORM for MySql implementation.
        /// </summary>
        /// <param name="agent">a MySql agent to use for ORM service</param>
        public MySqlOrmService(MySqlAgent agent) : base(agent) { }


        protected override string GetSelectByParentIdQuery<T>(OrmEntityMap<T> map)
        {   
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName.Trim()));

            return string.Format("SELECT {0} FROM {1} WHERE {2}={3};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent), map.ParentIdFieldName.ToConventional(Agent),
                ParamPrefix + map.ParentIdFieldName);
        }

        protected override string GetSelectByNullParentIdQuery<T>(OrmEntityMap<T> map)
        {          
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName.Trim()));

            return string.Format("SELECT {0} FROM {1} WHERE {2} IS NULL;", string.Join(", ", fields),
                map.TableName.ToConventional(Agent), map.ParentIdFieldName.ToConventional(Agent));
        }

        protected override string GetSelectQuery<T>(OrmEntityMap<T> map)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName.Trim()));

            return string.Format("SELECT {0} FROM {1} WHERE {2}={3};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent), map.PrimaryKeyFieldName.ToConventional(Agent),
                ParamPrefix + map.PrimaryKeyFieldName);
        }

        protected override string GetSelectAllQuery<T>(OrmEntityMap<T> map)
        {  
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForSelect().Select(f => string.Format("{0} AS {1}",
                f.DbFieldName.ToConventional(Agent), f.PropName.Trim()));

            return string.Format("SELECT {0} FROM {1};", string.Join(", ", fields),
                map.TableName.ToConventional(Agent));
        }

        protected override string GetInsertStatement<T>(OrmEntityMap<T> map, SqlParam[] extraParams = null)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var propList = new List<string>(map.GetFieldsForInsert());
            if (extraParams != null) propList.AddRange(extraParams.Select(p => p.Name.Trim()));

            var fields = string.Join(", ", propList.Select(p => p.ToConventional(Agent)).ToArray());
            var parameters = string.Join(", ", propList.Select(p => ParamPrefix + p).ToArray());

            return string.Format("INSERT INTO {0}({1}) VALUES({2});", map.TableName.ToConventional(Agent), fields, parameters);

        }

        protected override string GetUpdateStatement<T>(OrmEntityMap<T> map, int? scope = null)
        {

            if (map.IsNull()) throw new ArgumentNullException(nameof(map));

            var fields = map.GetFieldsForUpdate(scope).Select(f => string.Format("{0}={1}",
                f.ToConventional(Agent), ParamPrefix + f));

            return string.Format("UPDATE {0} SET {1} WHERE {2}={3};", map.TableName.ToConventional(Agent),
                string.Join(", ", fields), map.PrimaryKeyFieldName.ToConventional(Agent),
                ParamPrefix + map.PrimaryKeyUpdateWhereParamName);
        }

        protected override string GetDeleteStatement<T>(OrmEntityMap<T> map)
        {
            if (map.IsNull()) throw new ArgumentNullException(nameof(map));
            return string.Format("DELETE FROM {0} WHERE {0}.{1}={2};", map.TableName.ToConventional(Agent),
                map.PrimaryKeyFieldName.ToConventional(Agent), ParamPrefix + map.PrimaryKeyFieldName);
        }

    }
}
