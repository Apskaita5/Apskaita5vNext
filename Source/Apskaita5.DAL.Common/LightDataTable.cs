using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Apskaita5.DAL.Common
{
    [Serializable]
    public sealed class LightDataTable
    {

        private readonly List<string> _dateTimeFormats = new List<string>();
        private string _tableName = string.Empty;


        /// <summary>
        /// Gets the collection of columns that belong to this table.
        /// </summary>
        /// <value>A LightDataColumnCollection that contains the collection of LightDataColumn objects for the table. 
        /// An empty collection is returned if no LightDataColumn objects exist.</value>
        /// <remarks>The LightDataColumnCollection determines the schema of a table by defining the data type of each column.</remarks>
        public LightDataColumnCollection Columns { get; }

        /// <summary>
        /// Gets the collection of rows that belong to this table.
        /// </summary>
        /// <value>A LightDataRowCollection that contains the collection of LightDataRow objects for the table. 
        /// An empty collection is returned if no LightDataRow objects exist.</value>
        public LightDataRowCollection Rows { get; }

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
            Columns = new LightDataColumnCollection(this);
            Rows = new LightDataRowCollection(this);

        }

        /// <summary>
        /// Initializes a new LightDataTable instance and fills it with schema and values using data reader specified.
        /// </summary>
        public LightDataTable(IDataReader reader) : this()
        {
            if (reader.IsNull()) throw new ArgumentNullException(nameof(reader));

            if (!reader.Read())
            {
                try { Columns.Add(reader); }
                catch (Exception) { }
                return;
            }

            Columns.Add(reader);
            Rows.Add(reader); 
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
                Columns.Add(new LightDataColumn(column));
            }

            foreach (var row in result.Rows)
            {
                Rows.Add(new LightDataRow(this, row));
            }

        }


        public static async Task<LightDataTable> CreateAsync(DbDataReader reader)
        {

            if (reader.IsNull()) throw new ArgumentNullException(nameof(reader));

            var result = new LightDataTable();

            if (!await reader.ReadAsync())
            {
                try { result.Columns.Add(reader); }
                catch (Exception) { }
                return result;
            }

            result.Columns.Add(reader);
            await result.Rows.AddAsync(reader);

            return result;

        }

        /// <summary>
        /// Gets an XML string containing the table schema and data.
        /// </summary>
        /// <returns></returns>
        public string GetXmlString()
        {

            var result = new LightDataTableProxy
            {
                Columns = Columns.ToProxyList(),
                Rows = Rows.ToProxyList(),
                TableName = _tableName
            };

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

    }
}