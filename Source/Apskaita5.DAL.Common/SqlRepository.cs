using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Reperesents an SQL repository and contains SQL queries for a particular SQL implementation 
    /// identified by unique tokens. Used to provide SQL queries for a particular SQL implementation
    /// in a transparant way, i.e. business class only needs to know the token, not a specific (raw)
    /// SQL query.
    /// </summary>
    [Serializable]
    public sealed class SqlRepository : List<SqlRepositoryItem>
    {

        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the XML file specified.
        /// </summary>
        /// <param name="filePath">the xml file to load the data from</param>
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
        public void LoadFile(string filePath)
        {

            if (filePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format(Properties.Resources.FileNotFound, filePath), filePath);

            LoadXml(File.ReadAllText(filePath, Constants.DefaultXmlFileEncoding));

        }

        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the XML string specified.
        /// </summary>
        /// <param name="xmlString">an XML string that contains SqlRepository data</param>
        /// <exception cref="ArgumentNullException">XML source string is empty.</exception>
        public void LoadXml(string xmlString)
        {

            if (xmlString.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlString));

            var result = Utilities.DeSerializeFromXml<SqlRepository>(xmlString);

            this.AddRange(result);

        }

        /// <summary>
        /// Loads a collection of SqlRepositoryItem from the tab delimited string specified. 
        /// Fields should be arranged in the following order: Token, Query, UsedByTypes.
        /// </summary>
        /// <param name="delimitedString">a string that contains SqlRepositoryItem data</param>
        /// <param name="lineDelimiter">a string that delimits lines (SqlRepositoryItem's)</param>
        /// <param name="fieldDelimiter">a string that delimits fields (SqlRepositoryItem's properties)</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        /// <exception cref="ArgumentNullException">Parameter lineDelimiter is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter fieldDelimiter is not specified.</exception>
        /// <exception cref="ArgumentException">Source string contains no fields.</exception>
        public void LoadDelimitedString(string delimitedString, string lineDelimiter, 
            string fieldDelimiter)
        {
            if (delimitedString.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(delimitedString));
            if (lineDelimiter == null || lineDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(lineDelimiter));
            if (fieldDelimiter == null || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (!delimitedString.Contains(fieldDelimiter))
                throw new ArgumentException(Properties.Resources.NoFieldsInString, nameof(delimitedString));

            foreach (var line in delimitedString.Split(new string[]{lineDelimiter}, 
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.IsNullOrWhiteSpace())
                {

                    var newItem = new SqlRepositoryItem
                        {
                            Token = line.GetDelimitedField(0, fieldDelimiter),
                            Query = line.GetDelimitedField(1, fieldDelimiter),
                            UsedByTypes = line.GetDelimitedField(2, fieldDelimiter)
                        };

                    this.Add(newItem);

                }
            }

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
        /// Gets a list of namespaces that use the repository.
        /// </summary>
        public List<string> GetNamespaces()
        {

            var result = new List<string>();

            foreach (var entry in this)
            {
                if (!string.IsNullOrWhiteSpace(entry.UsedByTypes))
                {
                    foreach (var type in entry.UsedByTypes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (type.Contains("."))
                        {
                            var nameSpace = type.Split(new string[] {"."}, StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                            if (!result.Contains(nameSpace, StringComparer.OrdinalIgnoreCase)) 
                                result.Add(nameSpace);
                        }
                        else
                        {
                            if (!result.Contains(string.Empty)) result.Add(string.Empty);
                        }
                    }
                }
            }

            result.Sort();

            return result;

        }

        /// <summary>
        /// Gets a list of (business) classes that use the repository.
        /// </summary>
        public List<string> GetTypes()
        {

            var result = new List<string>();

            foreach (var entry in this)
            {
                if (!string.IsNullOrWhiteSpace(entry.UsedByTypes))
                {
                    foreach (var type in entry.UsedByTypes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!result.Contains(type.Trim(), StringComparer.OrdinalIgnoreCase))
                            result.Add(type.Trim());
                    }
                }
            }

            result.Sort();

            return result;

        }

        /// <summary>
        /// Gets a list of (business) classes that use the repository for the namespace specified.
        /// </summary>
        /// <param name="nameSpace">a namespace to filter the result by</param>
        public List<string> GetTypes(string nameSpace)
        {

            if (nameSpace.IsNullOrWhiteSpace())
                return GetTypes();

            var result = new List<string>();

            foreach (var type in this.GetTypes())
            {
                if (!string.IsNullOrWhiteSpace(type) &&
                    type.StartsWith(nameSpace.Trim() + ".", StringComparison.OrdinalIgnoreCase))
                {
                    var shortType = type.Trim().Substring((nameSpace.Trim() + ".").Length);
                    if (!result.Contains(shortType, StringComparer.OrdinalIgnoreCase))
                        result.Add(shortType);
                }

            }

            result.Sort();

            return result;

        }

        /// <summary>
        /// Gets a value indicating that the SqlRepository contains (invalid) items with null tokens.
        /// </summary>
        public bool ContainsEmptyTokens()
        {
            return this.Any(entry => entry.Token.IsNullOrWhiteSpace());
        }

        /// <summary>
        /// Gets a list of Tokens that are not assigned to any (business) classes.
        /// Returns an empty list if no such items found.
        /// </summary>
        public List<string> GetNotUsedTokens()
        {
            return (from entry in this where entry.UsedByTypes.IsNullOrWhiteSpace() select entry.Token).ToList();
        }

        /// <summary>
        /// Gets a list of duplicate (invalid) Tokens. Returns an empty list if no duplicate Tokens found.
        /// </summary>
        public List<string> GetDuplicateTokens()
        {

            var result = new List<string>();
            foreach (var firstEntry in this)
            {
                
                if (!firstEntry.Token.IsNullOrWhiteSpace())
                {

                    foreach (var secondEntry in this)
                    {
                        if (!Object.ReferenceEquals(firstEntry, secondEntry) &&
                            firstEntry.Token.Equals(secondEntry.Token, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!result.Contains(firstEntry.Token.Trim(), StringComparer.OrdinalIgnoreCase))
                                result.Add(firstEntry.Token.Trim());
                        }

                    }

                }
                
            }

            return result;

        }

    }
}
