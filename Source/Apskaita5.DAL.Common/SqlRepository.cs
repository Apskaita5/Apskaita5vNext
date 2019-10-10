using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Reperesents an SQL repository and contains SQL queries for a particular SQL implementation 
    /// identified by unique tokens. Used to provide SQL queries for a particular SQL implementation
    /// in a transparant way, i.e. business class only needs to know the token, not a specific (raw)
    /// SQL query.
    /// </summary>
    [Serializable]
    public sealed class SqlRepository 
    {

        private string _application = string.Empty;
        private string _description = string.Empty;
        private string _extension = string.Empty;
        private string _extensionGuid = string.Empty;
        private string _sqlImplementation = string.Empty;
        private List<SqlRepositoryItem> _items = new List<SqlRepositoryItem>();


        /// <summary>
        /// Gets or sets a name of the application that the repository is meant for.
        /// </summary>
        public string Application { get => _application ?? string.Empty; set => _application = value ?? string.Empty; }

        /// <summary>
        /// Gets or sets a description of the repository (if any).
        /// </summary>
        public string Description { get => _description ?? string.Empty; set => _description = value ?? string.Empty; }

        /// <summary>
        /// Gets or sets a name of the applicatiion extension if the repository belongs to one. 
        /// Empty string otherwise.
        /// </summary>
        public string Extension { get => _extension ?? string.Empty; set => _extension = value ?? string.Empty; }

        /// <summary>
        /// Gets or sets an extension Guid if the repository belongs to the application extension. Empty string otherwise.
        /// </summary>
        public string ExtensionGuid { get => _extensionGuid ?? string.Empty; set => _extensionGuid = value ?? string.Empty; }

        /// <summary>
        /// Gets or sets a SQL implementation code, e.g. MySQL, SQLite etc.
        /// </summary>
        public string SqlImplementation { get => _sqlImplementation ?? string.Empty; set => _sqlImplementation = value ?? string.Empty; }

        /// <summary>
        /// Gets or sets a list of the repository entries.
        /// </summary>
        public List<SqlRepositoryItem> Items { get => _items; set => _items = value ?? new List<SqlRepositoryItem>(); }


        /// <summary>
        /// Creates a new empty SQL repository.
        /// </summary>
        public SqlRepository() { }

        /// <summary>
        /// Creates an SQL repository from a serialized value.
        /// </summary>
        public SqlRepository(string xmlString)
        {
            LoadXml(xmlString);
        }


        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the XML file specified.
        /// </summary>
        /// <param name="filePath">the xml file to load the data from</param>
        /// <param name="clearCurrentItems">whether to clear current collection of SQL statements 
        /// before loading new ones</param>
        /// <exception cref="ArgumentNullException">Parameter filePath is not specified.</exception>
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
        public void LoadFile(string filePath, bool clearCurrentItems = false)
        {

            if (filePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format(Properties.Resources.FileNotFound, filePath), filePath);

            try
            {
                LoadXml(File.ReadAllText(filePath, new UTF8Encoding(false)));
            }
            catch (Exception)
            {
                try
                {
                    LoadXml(File.ReadAllText(filePath, new UTF8Encoding(true)));
                }
                catch (Exception)
                {
                    try
                    {
                        LoadXml(File.ReadAllText(filePath, Encoding.Unicode));
                    }
                    catch (Exception)
                    {                        
                        try
                        {
                            LoadXml(File.ReadAllText(filePath, Encoding.ASCII));
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidDataException(string.Format(Properties.Resources.InvalidSqlRepositoryFileFormatException, 
                                filePath, ex.Message));
                        }
                    }
                }
            }

        }
        
        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the XML string specified.
        /// </summary>
        /// <param name="xmlString">an XML string that contains SqlRepository data</param>
        /// <param name="clearCurrentItems">whether to clear current collection of SQL statements 
        /// before loading new ones</param>
        /// <exception cref="ArgumentNullException">XML source string is empty.</exception>
        public void LoadXml(string xmlString, bool clearCurrentItems = false)
        {

            if (xmlString.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlString));

            var result = Utilities.DeSerializeFromXml<SqlRepository>(xmlString);

            if (clearCurrentItems) _items.Clear();
            _items.AddRange(result._items);

        }

        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the tab delimited string specified. 
        /// Fields should be arranged in the following order: Token, Query, UsedByTypes.
        /// </summary>
        /// <param name="delimitedString">a string that contains SqlRepositoryItem data</param>
        /// <param name="lineDelimiter">a string that delimits lines (SqlRepositoryItem's)</param>
        /// <param name="fieldDelimiter">a string that delimits fields (SqlRepositoryItem's properties)</param>
        /// <param name="clearCurrentItems">whether to clear current collection of SQL statements 
        /// before loading new ones</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        /// <exception cref="ArgumentNullException">Parameter lineDelimiter is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter fieldDelimiter is not specified.</exception>
        /// <exception cref="ArgumentException">Source string contains no fields.</exception>
        public void LoadDelimitedString(string delimitedString, string lineDelimiter, 
            string fieldDelimiter, bool clearCurrentItems = false)
        {
            if (delimitedString.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(delimitedString));
            if (null == lineDelimiter || lineDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(lineDelimiter));
            if (null == fieldDelimiter || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (!delimitedString.Contains(fieldDelimiter))
                throw new ArgumentException(Properties.Resources.NoFieldsInString, nameof(delimitedString));

            var newItems = delimitedString.Split(new string[] { lineDelimiter }, StringSplitOptions.RemoveEmptyEntries)
                .Where(line => !line.IsNullOrWhiteSpace()).Select(line => new SqlRepositoryItem(line, fieldDelimiter));

            if (clearCurrentItems) _items.Clear();
            _items.AddRange(newItems);

        }


        /// <summary>
        /// Writes the SqlRepository data to the xml file specified.
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
        public void SaveToFile(string filePath)
        {

            if (filePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(filePath));

            File.WriteAllText(filePath, GetXmlString(), Constants.DefaultXmlFileEncoding);

        }


        /// <summary>
        /// Gets an XML string that contains SqlRepository data.
        /// </summary>
        /// <returns>an XML string that contains SqlRepository data.</returns>
        public string GetXmlString()
        {
            return Utilities.SerializeToXml(this);
        }

        /// <summary>
        /// Gets a token - query dictionary for the repository.
        /// </summary>
        internal Dictionary<string, string> GetDictionary()
        {           
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in _items)
            {
                if (!item.Token.IsNullOrWhiteSpace())
                {
                    if (result.ContainsKey(item.Token.Trim())) throw new InvalidDataException(string.Format(
                        Properties.Resources.CannotConvertToDictionaryException, item.Token));
                    result.Add(item.Token.Trim(), item.Query);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a token - query dictionary for the repository and merges it into the base dictionary.
        /// </summary>
        /// <param name="baseDictionary">a dictionary to merge the data into</param>
        internal void MergeIntoDictionary(Dictionary<string, string> baseDictionary)
        {
            foreach (var item in GetDictionary())
            {
                if (baseDictionary.ContainsKey(item.Key)) throw new InvalidDataException(string.Format(
                    Properties.Resources.CannotMergeToDictionaryException, item.Key));
                baseDictionary.Add(item.Key, item.Value);
            }
        }


        /// <summary>
        /// Gets a list of namespaces that use the repository.
        /// </summary>
        public List<string> GetNamespaces()
        {

            var result = new List<string>();

            foreach (var item in _items
                .Where(entry => !string.IsNullOrWhiteSpace(entry.UsedByTypes))
                .SelectMany(entry => entry.UsedByTypes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)))
            {
                if (item.Contains("."))
                {
                     var nameSpace = item.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                     if (!result.Contains(nameSpace, StringComparer.OrdinalIgnoreCase))
                        result.Add(nameSpace);
                }
                else if (!result.Contains(string.Empty)) result.Add(string.Empty);                
            }

            result.Sort();

            return result;

        }

        /// <summary>
        /// Gets a list of (business) classes that use the repository.
        /// </summary>
        public List<string> GetTypes()
        {
            return _items
                .Where(entry => !string.IsNullOrWhiteSpace(entry.UsedByTypes))
                .SelectMany(entry => entry.UsedByTypes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(val => val)
                .ToList();
        }

        /// <summary>
        /// Gets a list of (business) classes that use the repository for the namespace specified.
        /// </summary>
        /// <param name="nameSpace">a namespace to filter the result by</param>
        public List<string> GetTypes(string nameSpace)
        {

            if (nameSpace.IsNullOrWhiteSpace())
                return GetTypes();

            nameSpace = nameSpace.Trim() + ".";

            return GetTypes()
                .Where(type => type.Trim().StartsWith(nameSpace, StringComparison.OrdinalIgnoreCase))
                .Select(type => type.Trim().Substring(nameSpace.Length))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(val => val)
                .ToList();

        }

        /// <summary>
        /// Gets a value indicating that the SqlRepository contains (invalid) items with null tokens.
        /// </summary>
        public bool ContainsEmptyTokens()
        {
            return _items.Any(entry => entry.Token.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Gets a list of Tokens that are not assigned to any (business) classes.
        /// Returns an empty list if no such items found.
        /// </summary>
        public List<string> GetNotUsedTokens()
        {
            return (from entry in _items where entry.UsedByTypes.IsNullOrWhiteSpace() select entry.Token).ToList();
        }

        /// <summary>
        /// Gets a list of duplicate (invalid) Tokens. Returns an empty list if no duplicate Tokens found.
        /// </summary>
        public List<string> GetDuplicateTokens()
        {   
            return _items.GroupBy(item => item.Token.Trim(), StringComparer.OrdinalIgnoreCase)
              .Where(g => g.Count() > 1)
              .Select(g => g.Key)
              .ToList();
        }
        
    }
}
