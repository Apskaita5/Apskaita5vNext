using Apskaita5.Common;
using System;

namespace Apskaita5.Domain.Core
{
    public static class CoreExtensions
    {

        public static bool IsSameIdentityAs(this IDomainEntityIdentity firstIdentity, IDomainEntityIdentity identity)
        {
            if (firstIdentity.IsNull()) throw new ArgumentNullException(nameof(firstIdentity));
            if (identity.IsNull()) throw new ArgumentNullException(nameof(identity));

            if (firstIdentity.DomainEntityType != identity.DomainEntityType) return false;

            if (firstIdentity.IsNew != identity.IsNew) return false;

            if (firstIdentity.IsNew) return ReferenceEquals(firstIdentity, identity);

            return firstIdentity.CompareTo(identity) == 0;
        }

    }
}
