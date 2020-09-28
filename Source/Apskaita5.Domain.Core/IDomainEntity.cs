namespace Apskaita5.Domain.Core
{
    /// <summary>
    /// This is the core interface implemented by all domain entity classes.
    /// Domain entity must have some unique identity value (primary key) that
    /// uniquely identifies an entity within the application database graph
    /// or indicates that the entity is new, not yet added to the application database graph.
    /// </summary>
    public interface IDomainEntity
    {
        /// <summary>
        /// Gets an identity object that uniquely identifies the domain entity instance
        /// within the application database graph or indicates that the entity is new,
        /// not yet added to the application database graph.
        /// </summary>
        IDomainEntityIdentity Id { get; }
    }
}
