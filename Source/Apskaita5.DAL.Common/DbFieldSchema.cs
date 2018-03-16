using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a canonical database table field specification.
    /// </summary>
    [Serializable]
    public sealed class DbFieldSchema
    {

        private readonly Guid _guid = Guid.NewGuid();
        private string _name = string.Empty;
        private DbDataType _dataType = DbDataType.Char;
        private int _length = 255;
        private bool _notNull = true;
        private bool _autoincrement = false;
        private bool _unsigned = true;
        private string _enumValues = string.Empty;
        private string _description = string.Empty;
        private DbIndexType _indexType = DbIndexType.None;
        private string _indexName = string.Empty;
        private DbForeignKeyActionType _onUpdateForeignKey = DbForeignKeyActionType.Cascade;
        private DbForeignKeyActionType _onDeleteForeignKey = DbForeignKeyActionType.Restrict;
        private string _refTable = string.Empty;
        private string _refField = string.Empty;


        /// <summary>
        /// Gets or sets the name of the database table field.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value.NotNullValue(); }
        }

        /// <summary>
        /// Gets or sets the canonical data type of the database table field.
        /// </summary>
        public DbDataType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        /// <summary>
        /// Gets or sets the length of the database table field, e.g. for CHAR field.
        /// Only applicable for some data types (char, varchar, etc.), have no effect for other types.
        /// Implementation of this field is dependent on SQL implementation.
        /// </summary>
        public int Length
        {
            get { return _length; }
            set { _length = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the database table field value should always be non null.
        /// </summary>
        public bool NotNull
        {
            get { return _notNull; }
            set { _notNull = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the database table field value 
        /// should be set automaticaly by database autoincrement function.
        /// </summary>
        public bool Autoincrement
        {
            get { return _autoincrement; }
            set { _autoincrement = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the database table field value is unsigned.
        /// Only applicable to integer types, have no effect for other types.
        /// </summary>
        public bool Unsigned
        {
            get { return _unsigned; }
            set { _unsigned = value; }
        }

        /// <summary>
        /// Gets or sets the comma separated list of enum values.
        /// Only applicable to enum type, have no effect for other types.
        /// </summary>
        public string EnumValues
        {
            get { return _enumValues; }
            set { _enumValues = value.NotNullValue(); }
        }

        /// <summary>
        /// Gets or sets the description of the database table field.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value.NotNullValue(); }
        }

        /// <summary>
        /// Gets or sets the type of the index that should be created for the database table field.
        /// </summary>
        /// <remarks>Multi column indexes are not supported.
        /// Some SQL implementations technically creates two indexes for foreign key, 
        /// however as for the canonical model they are considered as one.</remarks>
        public DbIndexType IndexType
        {
            get { return _indexType; }
            set { _indexType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the index that should be created for the database table field.
        /// </summary>
        /// <remarks>Only applicable if the <see cref="IndexType"/> is not <see cref="DbIndexType.None"/>,
        /// otherwise have no effect.</remarks>
        public string IndexName
        {
            get { return _indexName; }
            set { _indexName = value.NotNullValue(); }
        }

        /// <summary>
        /// Gets or sets the type of the action taken by the database when a foreign key (parent)
        /// is updated for the database table field.
        /// </summary>
        /// <remarks>Only applicable if the <see cref="IndexType"/> is <see cref="DbIndexType.ForeignKey"/>,
        /// otherwise have no effect.</remarks>
        public DbForeignKeyActionType OnUpdateForeignKey
        {
            get { return _onUpdateForeignKey; }
            set { _onUpdateForeignKey = value; }
        }

        /// <summary>
        /// Gets or sets the type of the action taken by the database when a foreign key (parent)
        /// is deleted for the database table field.
        /// </summary>
        /// <remarks>Only applicable if the <see cref="IndexType"/> is <see cref="DbIndexType.ForeignKey"/>,
        /// otherwise have no effect.</remarks>
        public DbForeignKeyActionType OnDeleteForeignKey
        {
            get { return _onDeleteForeignKey; }
            set { _onDeleteForeignKey = value; }
        }

        /// <summary>
        /// Gets or sets the database table that is referenced by the foreign key.
        /// </summary>
        public string RefTable
        {
            get { return _refTable; }
            set{ _refTable = value.NotNullValue(); }
        }

        /// <summary>
        /// Gets or sets the database table field that is referenced by the foreign key.
        /// </summary>
        public string RefField
        {
            get { return _refField; }
            set{ _refField = value.NotNullValue(); }
        }


        /// <summary>
        /// Initializes a new DbFieldSchema instance.
        /// </summary>
        public DbFieldSchema()
        {
        }

        internal DbFieldSchema(string source, string fieldDelimiter)
        {

            if (source.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(source));
            if (fieldDelimiter == null || fieldDelimiter.Length < 1)
                throw new ArgumentNullException(nameof(fieldDelimiter));

            _name = source.GetDelimitedField(0, fieldDelimiter);
            _dataType = source.GetDelimitedField(1, fieldDelimiter).ParseEnum(DbDataType.Char);
            _length = source.GetDelimitedField(2, fieldDelimiter).ParseInt(255);
            _notNull = source.GetDelimitedField(3, fieldDelimiter).ParseBoolean(true);
            _autoincrement = source.GetDelimitedField(4, fieldDelimiter).ParseBoolean(false);
            _unsigned = source.GetDelimitedField(5, fieldDelimiter).ParseBoolean(true);
            _enumValues = source.GetDelimitedField(6, fieldDelimiter);
            _description = source.GetDelimitedField(7, fieldDelimiter);
            _indexType = source.GetDelimitedField(8, fieldDelimiter).ParseEnum(DbIndexType.None);
            _indexName = source.GetDelimitedField(9, fieldDelimiter);
            _onUpdateForeignKey = source.GetDelimitedField(10, fieldDelimiter).ParseEnum(DbForeignKeyActionType.Cascade);
            _onDeleteForeignKey = source.GetDelimitedField(11, fieldDelimiter).ParseEnum(DbForeignKeyActionType.Restrict);

        }


        /// <summary>
        /// Gets a cannonical definition of the field.
        /// </summary>
        public string GetDefinition()
        {
            var result = string.Format("{0} {1}", _name, _dataType.ToString());
            
            if (_dataType == DbDataType.Char || _dataType == DbDataType.VarChar || _dataType == DbDataType.Decimal)
                result = string.Format("{0}({1})", result, _length.ToString());
            if (_dataType == DbDataType.Enum)
                result = string.Format("{0}({1})", result, _enumValues);

            if (_notNull) result = result + " NOT NULL";
            if (_unsigned && _dataType.IsDbDataTypeInteger()) result = result + " UNSIGNED";
            if (_autoincrement && _dataType.IsDbDataTypeInteger()) result = result + " AUTOINCREMENT";
            
            if (_indexType == DbIndexType.Primary) result = result + " PRIMARY KEY";
            if (_indexType == DbIndexType.Simple) result = string.Format("{0} INDEX {1}", result, _indexName);
            if (_indexType == DbIndexType.Unique) result = string.Format("{0} UNIQUE INDEX {1}", result, _indexName);
            if (_indexType == DbIndexType.ForeignKey) result = string.Format(
                "{0} FOREIGN KEY {1} REFERENCES {2}({3}) ON UPDATE {4} ON DELETE {5}",
                result, _indexName, _refTable, _refField, _onUpdateForeignKey.ToString().ToUpperInvariant(),
                _onDeleteForeignKey.ToString().ToUpperInvariant());


            return result;

        }


        /// <summary>
        /// Gets the list of all the data errors for the DbFieldSchema instance as a per property dictionary.
        /// </summary>
        public Dictionary<string, List<string>> GetDataErrors()
        {

            var result = new Dictionary<string, List<string>>();

            if (_name.IsNullOrWhiteSpace()) 
                GetOrCreateErrorList(nameof(Name), result).Add(Properties.Resources.DbFieldSchema_FieldNameNull);

            if (_name.IndexOf(" ", StringComparison.OrdinalIgnoreCase) >= 0) 
                GetOrCreateErrorList(nameof(Name), result).Add(Properties.Resources.DbFieldSchema_FieldNameContainsBlankSpaces);

            if (_length < 0)
                GetOrCreateErrorList(nameof(Length), result).Add(Properties.Resources.DbFieldSchema_FieldLengthNegative);

            if (_autoincrement && !Utilities.IsDbDataTypeInteger(_dataType))
                GetOrCreateErrorList(nameof(Autoincrement), result).Add(Properties.Resources.DbFieldSchema_AutoincrementInvalid);

            if (_dataType == DbDataType.Enum && _enumValues.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(EnumValues), result).Add(Properties.Resources.DbFieldSchema_EnumValuesNull);

            if (_dataType == DbDataType.Enum && !_enumValues.Contains(","))
                GetOrCreateErrorList(nameof(EnumValues), result).Add(Properties.Resources.DbFieldSchema_EnumValuesTooShort);

            if (_indexType != DbIndexType.None && _indexName.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(IndexName), result).Add(Properties.Resources.DbFieldSchema_IndexNameNull);

            if (_indexType == DbIndexType.ForeignKey && _refTable.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(RefTable), result).Add(Properties.Resources.DbFieldSchema_RefTableNull);

            if (_indexType == DbIndexType.ForeignKey && _refField.IsNullOrWhiteSpace())
                GetOrCreateErrorList(nameof(RefField), result).Add(Properties.Resources.DbFieldSchema_RefFieldNull);

            return result;

        }

        private List<string> GetOrCreateErrorList(string key, Dictionary<string, List<string>> dict)
        {
            if (!dict.ContainsKey(key)) dict.Add(key, new List<string>()); ;
            return dict[key];
        }

        /// <summary>
        /// Gets the description of all the data errors for the DbFieldSchema instance.
        /// </summary>
        public string GetDataErrorsString()
        {

            var dict = GetDataErrors();

            if (dict.Count() < 1) return string.Empty;

            var result = new List<string>();

            result.Add(string.Format(Properties.Resources.DbFieldSchema_ErrorStringHeader, _name, _dataType.ToString(), _length.ToString()));

            result.AddRange(dict.SelectMany(entry => entry.Value));

            return string.Join(Environment.NewLine, result.ToArray());

        }


        /// <summary>
        /// Technical method to support databinding. Returns a Guid 
        /// that is created for every new DbFieldSchema instance (not persisted).
        /// </summary>
        /// <returns></returns>
        public object GetIdValue()
        {
            return _guid;
        }

    }
}
