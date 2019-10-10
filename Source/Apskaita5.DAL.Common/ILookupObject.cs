namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Could be used for objects that represent lookup references (e.g. to a person, an account etc.)
    /// in order they could be assigned as an <see cref="SqlParam">SQL param</see> directly.
    /// </summary>
    public interface ILookupObject
    {

        /// <summary>
        /// Gets a value indicating that the lookup object represents a null value.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets an identity of the lookup object, i.e. an id used as a foreign key.
        /// </summary>
        object GetId();

    }
}
