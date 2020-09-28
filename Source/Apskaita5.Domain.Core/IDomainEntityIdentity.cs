using System;
using System.Collections.Generic;
using System.Text;

namespace Apskaita5.Domain.Core
{
    /// <summary>
    /// Represents domain entity instance's unique identity value within the domain entity graph.
    /// </summary>
    public interface IDomainEntityIdentity : IComparable
    {
        /// <summary>
        /// Gets a type of the domain entity that is identified.
        /// </summary>
        Type DomainEntityType { get; }

        /// <summary>
        /// Gets a type of the underlying identity field (primary key: int, long, string, guid etc.).
        /// </summary>
        Type IdentityValueType { get; }

        /// <summary>
        /// Gets a value of the underlying identity field (primary key: int, long, string, guid etc.).
        /// </summary>
        object IdentityValue { get; }

        /// <summary>
        /// Returns true if the domain entity identified is a new entity, 
        /// false if it is a pre-existing entity.
        /// </summary>
        /// <remarks>
        /// An object is considered to be new if its primary identifying (key) value 
        /// doesn't correspond to data in the database. In other words, 
        /// if the data values in this particular
        /// object have not yet been saved to the database the object is considered to
        /// be new. Likewise, if the object's data has been deleted from the database
        /// then the object is considered to be new.
        /// </remarks>
        /// <returns>A value indicating if the entity identified is new.</returns>
        bool IsNew { get; }

    }
}
