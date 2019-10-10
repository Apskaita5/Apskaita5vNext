
namespace Apskaita5.DAL.Common.DbSchema
{
    /// <summary>
    /// Represents a canonical database index type.
    /// </summary>
    public enum DbIndexType
    {

        None = 0,
        Primary = 1,
        Unique = 2,
        Simple = 3,
        ForeignKey = 4,
        ForeignPrimary =5

    }
}
