using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a canonical database specification.
    /// </summary>
    [Serializable]
    public sealed class DbSchema
    {

        private string _description = string.Empty;
        private string _charsetName = string.Empty;
        private List<DbTableSchema> _tables = new List<DbTableSchema>();


        /// <summary>
        /// Gets or sets a description of the database.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value.NotNullValue().Trim();
            }
        }

        /// <summary>
        /// Gets or sets the tables of the database.
        /// </summary>
        /// <remarks>Setter should not be used, it is exclusively for XML serialization support.</remarks>
        public List<DbTableSchema> Tables
        {
            get { return _tables; }
            set { _tables = value; }
        }

        /// <summary>
        /// Gets or sets the name of the charset that is applicable for the database.
        /// Only used for information, does not affect creation or modification.
        /// </summary>
        public string CharsetName
        {
            get { return _charsetName; }
            set
            {
                _charsetName = value.NotNullValue().Trim();
            }
        }


        /// <summary>
        /// Loads a collection of DbTableSchema from the XML string specified.
        /// </summary>
        /// <param name="xmlSource">an XML string that contains DbSchema data</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        public void LoadXml(string xmlSource)
        {

            if (xmlSource.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlSource));

            var source = Utilities.DeSerializeFromXml<DbSchema>(xmlSource);

            if (_tables == null) _tables = new List<DbTableSchema>();

            _tables.AddRange(source.Tables);
            _charsetName = source._charsetName;
            _description = source._description;

        }

        /// <summary>
        /// Loads a collection of DbTableSchema from the XML file specified.
        /// </summary>
        /// <param name="xmlFilePath">a path to the XML file that contains DbTable data</param>
        /// <exception cref="ArgumentNullException">Path to the XML file is empty.</exception>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        /// <exception cref="ArgumentException">filePath contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="PathTooLongException">The specified filePath, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified filePath is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in filePath was not found.</exception>
        /// <exception cref="NotSupportedException">filePath is in an invalid format.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void LoadXmlFile(string xmlFilePath)
        {

            if (xmlFilePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlFilePath));

            var repository = new DbSchema();
            try
            {
                repository.LoadXml(System.IO.File.ReadAllText(xmlFilePath, new UTF8Encoding(false)));
            }
            catch (Exception)
            {
                try
                {
                    repository.LoadXml(System.IO.File.ReadAllText(xmlFilePath, new UTF8Encoding(true)));
                }
                catch (Exception)
                {
                    try
                    {
                        repository.LoadXml(System.IO.File.ReadAllText(xmlFilePath, Encoding.Unicode));
                    }
                    catch (Exception)
                    {
                        repository.LoadXml(System.IO.File.ReadAllText(xmlFilePath, Encoding.ASCII));
                    }
                }
            }

            if (_tables == null) _tables = new List<DbTableSchema>();

            _tables.AddRange(repository.Tables);
            _charsetName = repository._charsetName;
            _description = repository._description;

        }

        /// <summary>
        /// Loads a collection of DbTableSchema from the XML files that are located in the 
        /// xmlFolderPath specified (including subfolders).
        /// </summary>
        /// <param name="xmlFolderPath">a path to the folder containing XML files 
        /// that contain DbSchema data</param>
        /// <exception cref="ArgumentNullException">Path to the folder is empty.</exception>
        /// <exception cref="InvalidOperationException">The XML file folder specified contains no XML files.</exception>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        /// <exception cref="ArgumentException">filePath contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="PathTooLongException">The specified filePath, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified filePath is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="FileNotFoundException">The file specified in filePath was not found.</exception>
        /// <exception cref="NotSupportedException">filePath is in an invalid format.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void LoadXmlFolder(string xmlFolderPath)
        {

            if (xmlFolderPath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlFolderPath));

            var result = new List<string>();

            foreach (var filePath in Directory.GetFiles(xmlFolderPath,
                "*.*", SearchOption.AllDirectories))
            {
                var fileExtension = System.IO.Path.GetExtension(filePath);
                if (fileExtension != null && fileExtension.ToLower().StartsWith(
                    Constants.DbSchemaFileExtension))
                {
                    result.Add(filePath);
                }
            }

            if (result.Count < 1)
                throw new InvalidOperationException(string.Format(Properties.Resources.NoFilesInFolder, xmlFolderPath));

            foreach (var filePath in result)
            {
                this.LoadXmlFile(filePath);
            }

        }

        /// <summary>
        /// Writes the DbSchema data to the xml file specified.
        /// </summary>
        /// <param name="filePath">the path to the xml file to write the data to</param>
        /// <exception cref="ArgumentNullException">Parameter filePath is not specified.</exception>
        /// <exception cref="ArgumentException">filePath contains one or more invalid characters 
        /// as defined by InvalidPathChars.</exception>
        /// <exception cref="PathTooLongException">The specified filePath, file name, or both exceed 
        /// the system-defined maximum length. For example, on Windows-based platforms, 
        /// paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified filePath is invalid 
        /// (for example, it is on an unmapped drive).</exception>
        /// <exception cref="IOException">An I/O error occurred while opening the file.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission.</exception>
        /// <exception cref="NotSupportedException">filePath is in an invalid format.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void SaveXmlFile(string filePath)
        {

            if (filePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(filePath));

            File.WriteAllText(filePath, GetXml(), Constants.DefaultXmlFileEncoding);

        }

        /// <summary>
        /// Gets an XML string that contains DbSchema data.
        /// </summary>
        /// <returns>an XML string that contains DbSchema data.</returns>
        public string GetXml()
        {
            return Utilities.SerializeToXml(this);
        }


        /// <summary>
        /// Gets the list of all the data errors for the DbSchema instance as a per property dictionary 
        /// (not including it's child tables errors).
        /// </summary>
        /// <remarks>Only for consistency accross schema objects.</remarks>
        public Dictionary<string, List<string>> GetDataErrors()
        {
            var result = new Dictionary<string, List<string>>();

            if (_tables == null || _tables.Count < 1)
                GetOrCreateErrorList(nameof(Tables), result).Add(Properties.Resources.DbSchema_TableListEmpty);

            return result;
        }

        private List<string> GetOrCreateErrorList(string key, Dictionary<string, List<string>> dict)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, new List<string>()); ;
            return dict[key];
        }

        /// <summary>
        /// Gets the description of all the data errors for the DbSchema instance (including it's child fields).
        /// </summary>
        public string GetDataErrorsString()
        {

            var dict = GetDataErrors();

            var childrenErrors = new List<string>();
            if (_tables != null)
            {
                childrenErrors.AddRange(_tables.Select(table => table.GetDataErrorsString()).
                    Where(tableErrors => !tableErrors.IsNullOrWhiteSpace()));
            }

            if (dict.Count() < 1 && childrenErrors.Count < 1) return string.Empty;

            var result = new List<string>();

            if (dict.Count > 0)
            {
                result.Add(Properties.Resources.DbSchema_ErrorStringHeader);
                result.AddRange(dict.SelectMany(entry => entry.Value));
            }

            if (childrenErrors.Count > 0)
            {
                if (result.Count > 0) result.Add(string.Empty);
                result.Add(Properties.Resources.DbSchema_ErrorStringTablesHeader);
                result.Add(string.Empty);
                result.AddRange(childrenErrors);
            }

            return string.Join(Environment.NewLine, result.ToArray());

        }

    }
}
