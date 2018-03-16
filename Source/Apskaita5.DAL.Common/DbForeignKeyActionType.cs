
namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a canonical type of the action taken by the database when a foreign key (parent)
    /// is updated or deleted.
    /// </summary>
    /// <remarks>Actions NO ACTION and SET DEFAULT is not included, because they are not universaly supported.</remarks>
    public enum DbForeignKeyActionType
    {

        /// <summary>
        /// The "RESTRICT" action means that the application is prohibited from deleting 
        /// (for ON DELETE RESTRICT) or modifying (for ON UPDATE RESTRICT) a parent key 
        /// when there exists one or more child keys mapped to it.
        /// </summary>
        Restrict = 0,

        /// <summary>
        /// A "CASCADE" action propagates the delete or update operation on the parent key 
        /// to each dependent child key. For an "ON DELETE CASCADE" action, this means that 
        /// each row in the child table that was associated with the deleted parent row is also deleted. 
        /// For an "ON UPDATE CASCADE" action, it means that the values stored in each dependent child 
        /// key are modified to match the new parent key values.
        /// </summary>
        Cascade = 1,

        /// <summary>
        /// If the configured action is "SET NULL", then when a parent key is deleted (for ON DELETE SET NULL)
        /// or modified (for ON UPDATE SET NULL), the child key columns of all rows in the child table 
        /// that mapped to the parent key are set to contain SQL NULL values.
        /// </summary>
        SetNull = 2

    }
}
