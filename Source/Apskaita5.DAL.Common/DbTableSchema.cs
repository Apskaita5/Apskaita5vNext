using System;
using System.Collections.Generic;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a canonical database table specification.
    /// </summary>
    [Serializable]
    public sealed class DbTableSchema
    {

        private string _name = string.Empty;
        private string _description = string.Empty;
        private string _charsetName = string.Empty;
        private string _engineName = string.Empty;
        private List<DbFieldSchema> _fields = new List<DbFieldSchema>();


        /// <summary>
        /// Gets or sets the name of the database table.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value.NotNullValue().Trim(); }
        }

        /// <summary>
        /// Gets or sets a description of the database table.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value.NotNullValue().Trim(); }
        }

        /// <summary>
        /// Gets or sets the fields of the database table.
        /// </summary>
        /// <remarks>Setter should not be used, it is exclusively for XML serialization support.</remarks>
        public List<DbFieldSchema> Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        /// <summary>
        /// Gets or sets the name of the charset that is applicable for the database table.
        /// Only used for information, does not affect creation or modification.
        /// </summary>
        public string CharsetName
        {
            get { return _charsetName; }
            set { _charsetName = value.NotNullValue().Trim(); }
        }

        /// <summary>
        /// Gets or sets the name of the database engine that is used for the database table.
        /// Only used for information, does not affect creation or modification.
        /// </summary>
        public string EngineName
        {
            get { return _engineName; }
            set { _engineName = value.NotNullValue().Trim(); }
        }


        /// <summary>
        /// Initializes a new DbTableSchema instance.
        /// </summary>
        public DbTableSchema()
        {
        }


        /// <summary>
        /// Gets the list of all the data errors for the DbFieldSchema instance as a per property dictionary 
        /// (not including it's child fields errors).
        /// </summary>
        public Dictionary<string, List<string>> GetDataErrors()
        {

            var result = new Dictionary<string, List<string>>();

            if (_name.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(Name), result).Add(Properties.Resources.DbTableSchema_TableNameNull);

            if (_name.IndexOf(" ", StringComparison.OrdinalIgnoreCase) >= 0)
                GetOrCreateErrorList(nameof(Name), result).Add(Properties.Resources.DbTableSchema_TableNameContainsEmptySpaces);

            if (_fields == null || _fields.Count < 1)
                GetOrCreateErrorList(nameof(Fields), result).Add(Properties.Resources.DbTableSchema_FieldListEmpty);

            return result;

        }

        private List<string> GetOrCreateErrorList(string key, Dictionary<string, List<string>> dict)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, new List<string>()); ;
            return dict[key];
        }

        /// <summary>
        /// Gets the description of all the data errors for the DbTableSchema instance (including it's child fields).
        /// </summary>
        public string GetDataErrorsString()
        {

            var dict = GetDataErrors();

            var childrenErrors = new List<string>();
            if (_fields != null)
            {
                childrenErrors.AddRange(_fields.Select(field => field.GetDataErrorsString()).
                    Where(fieldErrors => !fieldErrors.IsNullOrWhiteSpace()));
            }

            if (dict.Count() < 1 && childrenErrors.Count < 1) return string.Empty;

            var result = new List<string>();

            if (dict.Count > 0)
            {
                result.Add(string.Format(Properties.Resources.DbTableSchema_ErrorStringHeader, _name));
                result.AddRange(dict.SelectMany(entry => entry.Value));
            }

            if (childrenErrors.Count > 0)
            {
                if (result.Count > 0) result.Add(string.Empty);
                result.Add(string.Format(Properties.Resources.DbTableSchema_ErrorStringFieldsHeader, _name));
                result.AddRange(childrenErrors);
            }

            return string.Join(Environment.NewLine, result.ToArray());

        }


        /// <summary>
        /// Loads a collection of DbFieldSchema from the tab delimited string specified. 
        /// Fields should be arranged in the following order: Name, DataType, Length, NotNull, Autoincrement, 
        /// Unsigned, EnumValues, Description, IndexType, IndexName, OnUpdateForeignKey, OnDeleteForeignKey.
        /// </summary>
        /// <param name="delimitedString">a string that contains SqlRepositoryItem data</param>
        /// <param name="lineDelimiter">a string that delimits lines (SqlRepositoryItem's)</param>
        /// <param name="fieldDelimiter">a string that delimits fields (SqlRepositoryItem's properties)</param>
        /// <exception cref="ArgumentNullException">Source string is empty.</exception>
        /// <exception cref="ArgumentNullException">Parameter lineDelimiter is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter fieldDelimiter is not specified.</exception>
        /// <exception cref="ArgumentException">Source string contains no fields.</exception>
        public void LoadDelimitedString(string delimitedString, string lineDelimiter, string fieldDelimiter)
        {

            if (delimitedString.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(delimitedString));
            if (lineDelimiter == null || lineDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(lineDelimiter));
            if (fieldDelimiter == null || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            if (!delimitedString.Contains(fieldDelimiter))
                throw new ArgumentException(Properties.Resources.NoFieldsInString, nameof(delimitedString));

            if (_fields == null) _fields = new List<DbFieldSchema>();

            foreach (var line in delimitedString.Split(new string[] { lineDelimiter },
                StringSplitOptions.RemoveEmptyEntries))
            {
                if (!line.IsNullOrWhiteSpace())
                    _fields.Add(new DbFieldSchema(line, fieldDelimiter));
            }

        }

    }
}
