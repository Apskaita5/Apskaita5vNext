using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Defines a list of the application user roles that are used by the application.
    /// </summary>
    [Serializable]
    public sealed class ApplicationRoleSchemaList : List<ApplicationRoleSchema>
    {

        /// <summary>
        /// Gets the description of all the data errors for the ApplicationRoleSchemaList instance 
        /// (including it's child fields).
        /// </summary>
        public string GetDataErrorsString()
        {

            var childrenErrors = new List<string>();
            childrenErrors.AddRange(this.Select(field => field.GetDataErrorsString()).
                Where(fieldErrors => !fieldErrors.IsNullOrWhiteSpace()));
            
            if (childrenErrors.Count < 1) return string.Empty;

            var result = new List<string>() { Apskaita5.DAL.Common.Properties.Resources.ApplicationRoleSchemaList_DataErrorsHeader };
            result.AddRange(childrenErrors);

            return string.Join(Environment.NewLine, result.ToArray());

        }


        /// <summary>
        /// Moves the child ApplicationRoleSchema to the index specified.
        /// </summary>
        /// <param name="child">the child ApplicationRoleSchema to move</param>
        /// <param name="indexToMove">the zero based index to move the child to</param>
        /// <exception cref="ArgumentNullException">Parameter child is not specified.</exception>
        /// <exception cref="ArgumentException">Child is not a child of the current list.</exception>
        /// <exception cref="IndexOutOfRangeException">indexToMove is out of the range</exception>
        public void MoveToIndex(ApplicationRoleSchema child, int indexToMove)
        {

            if (child == null) throw new ArgumentNullException(nameof(child));

            if (!this.Contains(child)) throw new ArgumentException(Apskaita5.DAL.Common.Properties.Resources.ApplicationRoleSchemaList_SchemaIsNotChild);

            if (indexToMove < 0 || indexToMove + 1 > this.Count)
                throw new IndexOutOfRangeException();

            var indexToInsert = indexToMove;
            if (indexToInsert > this.IndexOf(child)) indexToInsert -= 1;

            this.Remove(child);
            this.Insert(indexToInsert, child);

        }

        /// <summary>
        /// Sets the <see cref="ApplicationRoleSchema.VisibleIndex">VisibleIndex</see>
        /// property of the each child ApplicationRoleSchema according to it's position in the list.
        /// Should be used before saving the data if the sorting/sequencing by drag & drop is used.
        /// </summary>
        public void SetSequentialVisibleIndex()
        {
            int index = 1;
            foreach (var role in this)
            {
                if (role.IsLookUpSubrole)
                {
                    role.VisibleIndex = 0;
                }
                else
                {
                    role.VisibleIndex = index;
                    index += 1;
                }
            }
        }


        /// <summary>
        /// Loads a collection of ApplicationRoleSchema from the tab delimited string specified. 
        /// Fields should be arranged in the following order: Name, Description, IsLookUpRole, 
        /// HasSelectSubrole, HasInsertSubrole, HasUpdateSubrole, HasExecuteSubrole and RequiredLookUpRoles.
        /// </summary>
        /// <param name="delimitedString">a string that contains ApplicationRoleSchema data</param>
        /// <param name="lineDelimiter">a string that delimits lines (ApplicationRoleSchema's)</param>
        /// <param name="fieldDelimiter">a string that delimits fields (ApplicationRoleSchema's properties)</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        /// <exception cref="ArgumentNullException">Parameter lineDelimiter is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter fieldDelimiter is not specified.</exception>
        /// <exception cref="ArgumentException">Source string contains no fields.</exception>
        public void LoadDelimitedString(string delimitedString, string lineDelimiter, string fieldDelimiter)
        {

            if (delimitedString == null || string.IsNullOrEmpty(delimitedString.Trim()))
                throw new ArgumentNullException(nameof(delimitedString));
            if (lineDelimiter == null || lineDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(lineDelimiter));
            if (fieldDelimiter == null || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (!delimitedString.Contains(fieldDelimiter))
                throw new ArgumentException(Apskaita5.DAL.Common.Properties.Resources.NoFieldsInString, nameof(delimitedString));

            foreach (var line in delimitedString.Split(new string[] { lineDelimiter },
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.IsNullOrWhiteSpace())
                    this.Add(new ApplicationRoleSchema(line, fieldDelimiter));
            }

        }

        /// <summary>
        /// Loads a collection of ApplicationRoleSchema from the XML string specified.
        /// </summary>
        /// <param name="xmlSource">an XML string that contains ApplicationRoleSchemaList data</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        public void LoadXml(string xmlSource)
        {

            if (xmlSource.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlSource));

            var source = Utilities.DeSerializeFromXml<ApplicationRoleSchemaList>(xmlSource);

            this.AddRange(source);

        }

        /// <summary>
        /// Loads a collection of ApplicationRoleSchema from the XML file specified.
        /// </summary>
        /// <param name="xmlFilePath">a path to the XML file that contains ApplicationRoleSchemaList data</param>
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

            var repository = new ApplicationRoleSchemaList();
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

            this.AddRange(repository);

        }

        /// <summary>
        /// Loads a collection of ApplicationRoleSchema from the XML files that are located in the 
        /// xmlFolderPath specified (including subfolders).
        /// </summary>
        /// <param name="xmlFolderPath">a path to the folder containing XML files 
        /// that contain ApplicationRoleSchemaList data</param>
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
                if (fileExtension != null && fileExtension.ToLowerInvariant().StartsWith(
                    Constants.ApplicationRoleSchemaListFileExtension))
                {
                    result.Add(filePath);
                }
            }

            if (result.Count < 1) 
                throw new InvalidOperationException(string.Format(Apskaita5.DAL.Common.Properties.Resources.NoFilesInFolder, xmlFolderPath));

            foreach (var filePath in result)
            {
                this.LoadXmlFile(filePath);
            }

        }

        /// <summary>
        /// Saves the current collection of ApplicationRoleSchema to the XML file specified.
        /// </summary>
        /// <param name="xmlFilePath">a path to the XML file to save the data to</param>
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
        public void SaveXmlFile(string xmlFilePath)
        {

            if (xmlFilePath.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(xmlFilePath));

            File.WriteAllText(xmlFilePath, Utilities.SerializeToXml(this), Constants.DefaultXmlFileEncoding);

        }

        /// <summary>
        /// Gets the current collection of ApplicationRoleSchema data serialized into the XML string.
        /// </summary>
        public string GetXml()
        {
            return Utilities.SerializeToXml(this);
        }

    }
}
