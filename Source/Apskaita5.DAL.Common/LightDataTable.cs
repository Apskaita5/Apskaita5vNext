using System;
using System.Collections.Generic;
using System.Data;

namespace Apskaita5.DAL.Common
{
    [Serializable]
    public sealed class LightDataTable
    {

        private List<string> _dateTimeFormats = new List<string>();
        private string _tableName = string.Empty;
        private readonly LightDataColumnCollection _columns = null;
        private readonly LightDataRowCollection _rows = null;


        /// <summary>
        /// Gets the collection of columns that belong to this table.
        /// </summary>
        /// <value>A LightDataColumnCollection that contains the collection of LightDataColumn objects for the table. 
        /// An empty collection is returned if no LightDataColumn objects exist.</value>
        /// <remarks>The LightDataColumnCollection determines the schema of a table by defining the data type of each column.</remarks>
        public LightDataColumnCollection Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the collection of rows that belong to this table.
        /// </summary>
        /// <value>A LightDataRowCollection that contains the collection of LightDataRow objects for the table. 
        /// An empty collection is returned if no LightDataRow objects exist.</value>
        public LightDataRowCollection Rows
        {
            get { return _rows; }
        }

        /// <summary>
        /// Gets or sets the name of the LightDataTable instance.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets a list of possible string DateTime formats.
        /// Should be set by the data provider (e.g. ISqlAgent) in order to support string to date conversion.
        /// </summary>
        public List<string> DateTimeFormats
        {
            get { return _dateTimeFormats; }
        }


        /// <summary>
        /// Initializes a new LightDataTable instance.
        /// </summary>
        public LightDataTable()
        {
            _columns = new LightDataColumnCollection(this);
            _rows = new LightDataRowCollection(this);

        }

        /// <summary>
        /// Initializes a new LightDataTable instance and fills it with schema and values using data reader specified.
        /// </summary>
        public LightDataTable(IDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));

            _columns = new LightDataColumnCollection(this);
            _rows = new LightDataRowCollection(this);

            if (!reader.Read())
            {
                try
                {
                    _columns.Add(reader);
                }
                catch (Exception) { }
                return;
            }

            _columns.Add(reader);
            _rows.Add(reader);

        }

        /// <summary>
        /// Initializes a new LightDataTable instance and fills it with schema and values using XML data.
        /// </summary>
        /// <param name="xmlString">an XML string to load the data from</param>
        /// <remarks>Use <see cref="GetXmlString">GetXml method</see> to convert a table to an XML string.</remarks>
        public LightDataTable(string xmlString) : this()
        {

            var result = Utilities.DeSerializeFromXml<LightDataTableProxy>(xmlString);

            _tableName = result.TableName;

            _dateTimeFormats = result.DateTimeFormats ?? new List<string>();

            foreach (var column in result.Columns)
            {
                _columns.Add(new LightDataColumn(column));
            }

            foreach (var row in result.Rows)
            {
                _rows.Add(new LightDataRow(this, row));
            }

        }


        /// <summary>
        /// Gets an XML string containing the table schema and data.
        /// </summary>
        /// <returns></returns>
        public string GetXmlString()
        {

            var result = new LightDataTableProxy
            {
                Columns = new List<LightDataColumnProxy>(),
                Rows = new List<LightDataRowProxy>(),
                TableName = _tableName
            };

            foreach (var column in _columns)
            {
                result.Columns.Add(column.GetLightDataColumnProxy());
            }

            foreach (var row in _rows)
            {
                result.Rows.Add(row.GetLightDataRowProxy());
            }

            return Utilities.SerializeToXml(result);

        }

        /// <summary>
        /// Gets a deep copy of the table.
        /// </summary>
        /// <returns></returns>
        public LightDataTable Clone()
        {
            return new LightDataTable(this.GetXmlString());
        }


        /// <summary>
        /// Gets a collection of default string DateTime formats that are used 
        /// if the <see cref="DateTimeFormats">DateTimeFormats</see> property 
        /// is not set by the data provider. 
        /// </summary>
        public static string[] GetDefaultDateTimeFormats()
        {
            return new string[] { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd", "HH:mm:ss" };
        }

    }
}