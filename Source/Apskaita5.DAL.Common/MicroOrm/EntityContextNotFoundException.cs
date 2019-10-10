using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    public class EntityContextNotFoundException : Exception
    {

        public EntityContextNotFoundException(Type entityType, string query, SqlParam[] parameters)
            : base (string.Format(Properties.Resources.DbEntityContextNotFoundException, entityType.Name,
                query, parameters.GetDescription()))
        {
            EntityType = entityType;
            Query = query;
            Parameters = parameters.GetDescription();
        }

        /// <summary>
        /// Gets a type of the business object for which the initialization context failed to fetch
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets an SQL query that was used to fetch initialization context.
        /// </summary>
        public string Query { get; }

        /// <summary>
        /// Gets a description of the SQL query parameters that was used to fetch initialization context.
        /// </summary>
        public string Parameters { get; }

    }
}
