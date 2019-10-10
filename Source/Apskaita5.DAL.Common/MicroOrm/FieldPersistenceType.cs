namespace Apskaita5.DAL.Common.MicroOrm
{
    /// <summary>
    /// Describes fow a business object field is persisted in the database
    /// </summary>
    public enum FieldPersistenceType
    {

        /// <summary>
        /// only read a field value on fetch, never insert or update
        /// </summary>
        Readonly = 0,

        /// <summary>
        /// only read a field value on fetch and insert field value for a new business object, never update
        /// </summary>
        InsertOnly = 1,

        /// <summary>
        /// all CRUD operations
        /// </summary>
        InsertAndUpdate = 2

    }
}
