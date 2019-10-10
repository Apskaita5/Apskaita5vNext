using System;

namespace Apskaita5.DAL.Common.MicroOrm
{
    [Serializable]
    public class EntityNotFoundException : Exception
    {

        /// <summary>
        /// Gets a type of the (business) entity that was not found in the database.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets an identity (primary key) that was used for the failed lookup.
        /// </summary>
        public string Id { get; }

        public EntityNotFoundException(Type entityType, string id) 
            : base(string.Format(Properties.Resources.DbEntityNotFoundException, entityType.Name, id)) {
            EntityType = entityType;
            Id = id;
        }
        
    }
}
