using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a common space for all the constants used in the assembly.
    /// </summary>
    public static class Constants
    {

        /// <summary>
        /// Gets a default encoding used when reading or writing xml files.
        /// </summary>
        public static readonly Encoding DefaultXmlFileEncoding = new UTF8Encoding(false);

        /// <summary>
        /// an extension for <see cref="SqlRepository">SqlRepository</see> files
        /// </summary>
        public const string SqlRepositoryFileExtension = ".xml";

        /// <summary>
        /// an extension for <see cref="ApplicationRoleSchemaList">ApplicationRoleSchemaList</see> files
        /// </summary>
        public const string ApplicationRoleSchemaListFileExtension = ".xml";

        /// <summary>
        /// an extension for <see cref="DbSchema">DbSchema</see> files
        /// </summary>
        public const string DbSchemaFileExtension = ".xml";
    
    }
}
