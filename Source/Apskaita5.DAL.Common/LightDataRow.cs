using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Apskaita5.DAL.Common
{
    [Serializable]
    public sealed class LightDataRow : IDataRecord
    {

        private readonly Object[] _data = null;
        private readonly LightDataTable _table = null;


        /// <summary>
        /// Gets the LightDataTable for which this row has a schema. (deattached rows are not allowed)
        /// </summary>
        public LightDataTable Table
        {
            get { return _table; }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get
            {
                if (_data == null) return 0;
                return _data.Length;
            }
        }

        /// <summary>
        /// Gets or sets the data stored in the specified LightDataColumn.
        /// </summary>
        /// <param name="column">A LightDataColumn that contains the data.</param>
        /// <exception cref="ArgumentNullException">The column is null.</exception>
        /// <exception cref="ArgumentException">The column does not belong to this table.</exception>
        /// <exception cref="InvalidCastException">The item data type is invalid. Must have Clone method.</exception>
        /// <exception cref="ArgumentException">The data types of the value and the column do not match.</exception>
        /// <exception cref="InvalidOperationException">An edit tried to change the value of a read-only column.</exception>
        /// <remarks>Always gets (sets) a copy/clone of the value, not a reference. 
        /// Otherwise an indirect reference would be created to the table 
        /// which is unacceptable for a data transfer object.</remarks>
        public object this[LightDataColumn column]
        {
            get
            {
                if (column == null) throw new ArgumentNullException(nameof(column));
                if (!Object.ReferenceEquals(_table, column.Table))
                    throw new ArgumentException(Properties.Resources.LightDataRow_ColumnDoesNotBelongToTable);

                return CloneValue(_data[column.Ordinal]);

            }
            set
            {
                if (column == null) throw new ArgumentNullException(nameof(column));
                if (!Object.ReferenceEquals(_table, column.Table))
                    throw new ArgumentException(Properties.Resources.LightDataRow_ColumnDoesNotBelongToTable);
                if (column.ReadOnly)
                    throw new InvalidOperationException(Properties.Resources.LightDataRow_CannotEditReadOnlyColumn);

                if (value == null)
                {
                    _data[column.Ordinal] = null;
                    return;
                }
                if (column.DataType.IsAssignableFrom(value.GetType()))
                {
                    _data[column.Ordinal] = CloneValue(value);
                }

                throw new ArgumentException(Properties.Resources.LightDataRow_DataTypeMismatch);

            }
        }

        /// <summary>
        /// Gets or sets the data stored in the column specified by index.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The columnIndex argument is out of range.</exception>
        /// <exception cref="InvalidCastException">The item data type is invalid. Must have Clone method.</exception>
        /// <exception cref="ArgumentException">The data types of the value and the column do not match.</exception>
        /// <exception cref="InvalidOperationException">An edit tried to change the value of a read-only column.</exception>
        /// <remarks>Always gets (sets) a copy/clone of the value, not a reference. 
        /// Otherwise an indirect reference would be created to the table 
        /// which is unacceptable for a data transfer object.</remarks>
        public object this[int columnIndex]
        {
            get
            {
                CheckIndex(columnIndex);
                return CloneValue(_data[columnIndex]);
            }
            set
            {
                CheckIndex(columnIndex);
                if (_table.Columns[columnIndex].ReadOnly)
                    throw new InvalidOperationException(Properties.Resources.LightDataRow_CannotEditReadOnlyColumn);

                if (value == null)
                {
                    _data[columnIndex] = null;
                    return;
                }
                if (_table.Columns[columnIndex].DataType.IsAssignableFrom(value.GetType()))
                {
                    _data[columnIndex] = CloneValue(value);
                }

                throw new ArgumentException(Properties.Resources.LightDataRow_DataTypeMismatch);
            }
        }

        /// <summary>
        /// Gets or sets the data stored in the column specified by name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <exception cref="ArgumentException">The column specified by columnName cannot be found.</exception>
        /// <exception cref="InvalidCastException">The item data type is invalid. Must have Clone method.</exception>
        /// <exception cref="ArgumentException">The data types of the value and the column do not match.</exception>
        /// <exception cref="InvalidOperationException">An edit tried to change the value of a read-only column.</exception>
        /// <remarks>Always gets (sets) a copy/clone of the value, not a reference. 
        /// Otherwise an indirect reference would be created to the table 
        /// which is unacceptable for a data transfer object.</remarks>
        public object this[string columnName]
        {
            get
            {
                var column = _table.Columns[columnName];
                if (column == null)
                    throw new ArgumentException(Properties.Resources.LightDataRow_ColumnNotFoundByName);
                return this[column];
            }
            set
            {
                var column = _table.Columns[columnName];
                if (column == null)
                    throw new ArgumentException(Properties.Resources.LightDataRow_ColumnNotFoundByName);
                this[column] = value;
            }
        }


        /// <summary>
        /// Creates new empty data row for a table.
        /// </summary>
        /// <param name="table">table to create a new row for</param>
        /// <remarks>For internal use only.</remarks>
        /// <exception cref="ArgumentNullException">Parameter table is not specified.</exception>
        /// <exception cref="InvalidOperationException">Parent table has no columns.</exception>
        internal LightDataRow(LightDataTable table)
        {
            if (table==null) 
                throw new ArgumentNullException(nameof(table));
            if (table.Columns.Count < 1)
                throw new InvalidOperationException(Properties.Resources.LightDataRow_TableHasNoColumns);

            _table = table;

            _data = new Object[table.Columns.Count - 1];

        }

        /// <summary>
        /// Creates new data row for a table and fills it with values using data reader.
        /// </summary>
        /// <param name="table">table to create a new row for</param>
        /// <param name="reader">data reader to use</param>
        /// <remarks>For internal use only.</remarks>
        /// <exception cref="ArgumentNullException">Parameter table is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter reader is not specified.</exception>
        /// <exception cref="InvalidOperationException">Parent table has no columns.</exception>
        /// <exception cref="ArgumentException">Reader field count does not match table column count.</exception>
        internal LightDataRow(LightDataTable table, IDataReader reader)
        {

            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (table.Columns.Count < 1)
                throw new InvalidOperationException(Properties.Resources.LightDataRow_TableHasNoColumns);
            if (table.Columns.Count != reader.FieldCount) 
                throw new ArgumentException(Properties.Resources.LightDataRow_ReaderFieldCountInvalid);

            _table = table;

            _data = new object[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                _data[i] = reader.GetValue(i);
            }

        }

        /// <summary>
        /// Creates new data row for a table and fills it with values in the specified array.
        /// </summary>
        /// <param name="table">table to create a new row for</param>
        /// <param name="values">values to set</param>
        /// <exception cref="ArgumentNullException">Parameter table is not specified.</exception>
        /// <exception cref="ArgumentNullException">Parameter values is not specified.</exception>
        /// <exception cref="InvalidOperationException">Parent table has no columns.</exception>
        /// <exception cref="ArgumentException">Value array field count does not match table column count.</exception>
        internal LightDataRow(LightDataTable table, Object[] values)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (values == null || values.Length < 1)
                throw new ArgumentNullException(nameof(values));
            if (table.Columns.Count < 1)
                throw new InvalidOperationException(Properties.Resources.LightDataRow_TableHasNoColumns);
            if (table.Columns.Count != values.Length)
                throw new ArgumentException(Properties.Resources.LightDataRow_ArrayLengthInvalid);

            _table = table;

            _data = new object[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                _data[i] = values[i];
            }

        }

        internal LightDataRow(LightDataTable table, LightDataRowProxy proxy)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));
            if (proxy == null || proxy.Values == null || proxy.Values.Length < 1)
                throw new ArgumentNullException(nameof(proxy));

            _data = new object[proxy.Values.Length];

            for (int i = 0; i < proxy.Values.Length; i++)
            {
                _data[i] = proxy.Values[i];
            }

        }


        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        public string GetName(int i)
        {
            CheckIndex(i);
            return _table.Columns[i].ColumnName;
        }

        /// <summary>
        /// Gets the data type information for the specified field. See <see cref="LightDataColumn.NativeDataType"/>.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        public string GetDataTypeName(int i)
        {
            CheckIndex(i);
            return _table.Columns[i].NativeDataType;
        }

        /// <summary>
        /// Gets the Type information corresponding to the type of Object that would be returned from GetValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        public Type GetFieldType(int i)
        {
            CheckIndex(i);
            if (_data[i] != null) return _data[i].GetType();
            return _table.Columns[i].DataType;
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        public object GetValue(int i)
        {
            return this[i];
        }

        /// <summary>
        /// Not implemented method of IDataRecord.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        public int GetOrdinal(string name)
        {
            return _table.Columns.IndexOf(name);
        }

        /// <summary>
        /// Not implemented method of IDataRecord.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets a value that indicates whether the specified column contains a null or a DbNull value.
        /// </summary>
        /// <param name="column">A LightDataColumn.</param>
        /// <exception cref="ArgumentNullException">The column is null.</exception>
        /// <exception cref="ArgumentException">The column does not belong to this table.</exception>
        public bool IsNull(LightDataColumn column)
        {
            if (column == null) throw new ArgumentNullException(nameof(column));
            if (!Object.ReferenceEquals(_table, column.Table))
                throw new ArgumentException(Properties.Resources.LightDataRow_ColumnDoesNotBelongToTable);

            return (_data[column.Ordinal] == null || _data[column.Ordinal] == DBNull.Value);
        }

        /// <summary>
        /// Gets a value that indicates whether the column at the specified index contains a null or a DbNull value.
        /// </summary>
        /// <param name="columnIndex">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The columnIndex argument is out of range.</exception>
        public bool IsNull(int columnIndex)
        {
            CheckIndex(columnIndex);
            return (_data[columnIndex] == null || _data[columnIndex] == DBNull.Value);
        }

        /// <summary>
        /// Gets a value that indicates whether the named column contains a null or a DbNull value.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <exception cref="ArgumentException">The column specified by columnName cannot be found.</exception>
        public bool IsNull(string columnName)
        {
            var column = _table.Columns[columnName];
            if (column == null)
                throw new ArgumentException(Properties.Resources.LightDataRow_ColumnNotFoundByName);
            return (_data[column.Ordinal] == null || _data[column.Ordinal] == DBNull.Value);
        }

        /// <summary>
        /// Gets a value that indicates whether the specified column contains a DbNull value.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <exception cref="IndexOutOfRangeException">The index argument is out of range.</exception>
        public bool IsDBNull(int i)
        {
            CheckIndex(i);
            return (_data[i] != null && _data[i] == DBNull.Value);
        }

#region Type Converters

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to boolean.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Does "!= 0" conversion if the underlying value type is numeric.
        /// Tries to parse value as "true" or "false" if the underlying value type is string.
        /// Tries to parse Int64 value using InvariantCulture and to do "!= 0" conversion 
        /// if the underlying value type is string and the value is neither "true" nor "false".
        /// Tries to convert to string using UTF8 encoding and parse the string 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public bool GetBoolean(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is bool)
                return (bool)_data[i];

            if (raw is sbyte)
                return (sbyte)raw != 0;
            if (raw is byte)
                return (byte)raw != 0;
            if (raw is short)
                return (short)raw != 0;
            if (raw is ushort)
                return (ushort)raw != 0;
            if (raw is int)
                return (int)raw != 0;
            if (raw is uint)
                return (uint)raw != 0;
            if (raw is long)
                return (long)raw != 0;
            if (raw is ulong)
                return (ulong)raw != 0;

            if (raw is string)
            {
                if (((string)raw).Trim().ToUpperInvariant() == "TRUE") 
                    return true;
                if (((string)raw).Trim().ToUpperInvariant() == "FALSE")
                    return false;

                if (long.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return (intValue != 0);
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (stringValue.Trim().ToUpperInvariant() == "TRUE")
                        return true;
                    if (stringValue.Trim().ToUpperInvariant() == "FALSE")
                        return false;

                    if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                        return (intValue != 0);
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Does checked conversions if the underlying value type is sbyte. 
        /// (narrowing conversion from int types does not make sense for  byte)
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public byte GetByte(int i)
        {
            var raw = GetValueForCast(i, true);

            if (raw is byte)
                return (byte)raw;

            if (raw is sbyte)
                return checked((byte)(sbyte)raw);

            if (raw is string)
            {
                if (byte.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out byte byteValue))
                    return byteValue;
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (byte.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out byte byteValue))
                        return byteValue;
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to sbyte.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Only returns a sbyte value if the underlying value type is sbyte.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public sbyte GetSByte(int i)
        {
            var raw = GetValueForCast(i, true);
            
            if (raw is sbyte)
                return (sbyte)raw;

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Not implemented method of IDataRecord.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldOffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to char.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Returns the first char (after Trim) if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and to return the first char (after Trim)
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public char GetChar(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is char)
                return (char)raw;

            if (raw is string)
            {
                var chars = ((string)raw).Trim().ToCharArray();
                if (chars.Length > 0) return chars[0];
                throw new InvalidCastException(Properties.Resources.LightDataRow_CannotCastTypeOnNull);
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    var chars = stringValue.Trim().ToCharArray();
                    if (chars.Length > 0) return chars[0];
                    throw new InvalidCastException(Properties.Resources.LightDataRow_CannotCastTypeOnNull);
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);
        }

        /// <summary>
        /// Not implemented method of IDataRecord.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fieldoffset"></param>
        /// <param name="buffer"></param>
        /// <param name="bufferoffset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Guid.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Returns Guid.Empty if the underlying value is null or empty string or empty byte array.
        /// Tries to parse value if the underlying value type is string or 16 byte array.
        /// Tries to convert to string using UTF8 encoding and parse the string 
        /// if the underlying value type is byte array and it's size is not 16.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public Guid GetGuid(int i)
        {

            var raw = GetValueForCast(i, false);

            if (raw == null) return Guid.Empty;

            if (raw is Guid)
                return (Guid)raw;

            if (raw is string)
            {
                if (((string)raw).IsNullOrWhiteSpace())
                    return Guid.Empty;
                return new Guid(raw as string);
            }

            if (raw is byte[])
            {
                if (((byte[])raw).Length < 1) 
                    return Guid.Empty;
                if (((byte[])raw).Length == 16) 
                    return new Guid((byte[])raw);

                if (TryReadString((byte[])raw, out string stringValue))
                    return new Guid(stringValue);

            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int16.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public short GetInt16(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is short)
                return (short)raw;

            if (raw is sbyte)
                return (sbyte)raw;
            if (raw is byte)
                return (byte)raw;
            if (raw is ushort)
                return checked((short)(ushort)raw);
            if (raw is int)
                return checked((short)(int)raw);
            if (raw is uint)
                return checked((short)(uint)raw);
            if (raw is long)
                return checked((short)(long)raw);
            if (raw is ulong)
                return checked((short)(ulong)raw);
            if (raw is decimal)
                return (short)(decimal)raw;

            if (raw is string)
            {
                if (long.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return checked((short)intValue);
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                        return checked((short)intValue);
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int32.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public int GetInt32(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is int)
                return (int)raw;

            if (raw is sbyte)
                return (sbyte)raw;
            if (raw is byte)
                return (byte)raw;
            if (raw is short)
                return (short)raw;
            if (raw is ushort)
                return (ushort)raw;
            if (raw is uint)
                return checked((int)(uint)raw);
            if (raw is long)
                return checked((int)(long)raw);
            if (raw is ulong)
                return checked((int)(ulong)raw);
            if (raw is decimal)
                return (int)(decimal)raw;

            if (raw is string)
            {
                if (long.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return checked((int)intValue);
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                        return checked((int)intValue);
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int16.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Does checked conversions if the underlying value type is numeric or decimal.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public long GetInt64(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is long)
                return (long)raw;

            if (raw is sbyte)
                return (sbyte)raw;
            if (raw is byte)
                return (byte)raw;
            if (raw is short)
                return (short)raw;
            if (raw is ushort)
                return (ushort)raw;
            if (raw is int)
                return (int)raw;
            if (raw is uint)
                return (uint)raw;
            if (raw is ulong)
                return checked((long)(ulong)raw);
            if (raw is decimal)
                return (long)(decimal)raw;

            if (raw is string)
            {
                if (long.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                    return intValue;
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (long.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out long intValue))
                        return intValue;
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to float.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public float GetFloat(int i)
        {
            var raw = GetValueForCast(i, true);

            if (raw is float)
                return (float) raw;

            if (raw is string)
            {
                if (float.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                    return floatValue; ;
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                        return floatValue;
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to double.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Casts to double if the underlying value type is float.
        /// Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public double GetDouble(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is double)
                return (double)raw;
            if (raw is float)
                return (float)raw;

            if (raw is string)
            {
                if (double.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                    return doubleValue; ;
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                        return doubleValue;
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to string.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Returns null if the underlying value is null.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.
        /// Tries to cast to string in other cases.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        public string GetString(int i)
        {

            var raw = GetValueForCast(i, false);

            if (raw == null)
                return null;

            if (raw is string)
                return (string) raw;

            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                    return stringValue;
            }

            return (string)raw;

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to decimal.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Tries to parse value using InvariantCulture if the underlying value type is string.
        /// Tries to convert to string using UTF8 encoding and parse value using InvariantCulture 
        /// if the underlying value type is byte array.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        public decimal GetDecimal(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is decimal)
                return (decimal)raw;
            
            if (raw is string)
            {
                if (decimal.TryParse((string)raw, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                    return decimalValue; ;
            }
            if (raw is byte[])
            {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                        return decimalValue;
                }
            }

            throw new FormatException(Properties.Resources.LightDataRow_ColumnValueFormatInvalid);

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTime.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Tries to parse the value using either formats specified by 
        /// <see cref="LightDataTable.DateTimeFormats">parent table</see>
        /// or <see cref="LightDataTable.GetDefaultDateTimeFormats">default formats</see>
        /// if the underlying value type is string. 
        /// Tries to convert to string using UTF8 encoding and parse the string 
        /// if the underlying value type is byte array.
        /// Tries to cast to DateTime in other cases.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        public DateTime GetDateTime(int i)
        {

            var raw = GetValueForCast(i, true);

            if (raw is DateTime)
                return (DateTime)raw;

            if (raw is string)
                {
                if (TryParseDateTime((string)raw, out DateTime dateTimeValue))
                    return dateTimeValue;
            }
            if (raw is byte[])
                {
                if (TryReadString((byte[])raw, out string stringValue))
                {
                    if (TryParseDateTime(stringValue, out DateTime dateTimeValue))
                        return dateTimeValue;
                }
            }

            return (DateTime)raw;

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTimeOffset.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Uses <see cref="GetDateTime">GetDateTime</see> method to get a DateTime
        /// value and initializes a DateTimeOffset value with it.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        public DateTimeOffset GetDateTimeOffset(int i)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(GetDateTime(i), DateTimeKind.Utc));
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte array.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Returns null if the underlying value is null.</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">The column value is not a byte array.</exception>
        public byte[] GetByteArray(int i)
        {
            var raw = GetValueForCast(i, false);

            if (raw == null) return null;

            if (raw is byte[]) return (byte[])CloneValue(raw);

            throw new InvalidCastException(Properties.Resources.LightDataRow_ColumnValueIsNotByteArray);
        }

        /// <summary>
        /// Gets an enum value by converting int value (if it is int value) to the appropriate enum value.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="i">The zero-based index of the column.</param>
        /// <remarks>Should only be used when the enumeration values has int values assigned (not flags).</remarks>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="InvalidCastException">Failed to cast type on the null column value.</exception>
        /// <exception cref="FormatException">The column value is not in an appropriate format.</exception>
        /// <exception cref="ArgumentException">Type T is not an enumeration.</exception>
        /// <exception cref="InvalidCastException">Enum value for int code is not defined.</exception>
        public T GetEnum<T>(int i)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format(Properties.Resources.LightDataRow_ColumnValueIsNotEnumeration, typeof(T).FullName));

            var intValue = GetInt32(i);

            if (!Enum.IsDefined(typeof(T),intValue))
                throw new InvalidCastException(string.Format(Properties.Resources.LightDataRow_EnumValueInvalid, 
                    typeof(T).Name, intValue.ToString()));

            return (T)(object)intValue;
        }


        /// <summary>
        /// Gets the data stored in the column specified by index and converted to boolean.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetBoolean">GetBoolean</see> method.</remarks>
        public bool TryGetBoolean(int i, out bool result)
        {
            CheckIndex(i);
            try
            {
                result = GetBoolean(i);
                return true;
            }
            catch (Exception)
            {
                result = default(bool);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetByte">GetByte</see> method.</remarks>
        public bool TryGetByte(int i, out byte result)
        {
            CheckIndex(i);
            try
            {
                result = GetByte(i);
                return true;
            }
            catch (Exception)
            {
                result = default(byte);
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to sbyte.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetSByte">GetSByte</see> method.</remarks>
        public bool TryGetSByte(int i, out sbyte result)
        {
            CheckIndex(i);
            try
            {
                result = GetSByte(i);
                return true;
            }
            catch (Exception)
            {
                result = default(sbyte);
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to char.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetChar">GetChar</see> method.</remarks>
        public bool TryGetChar(int i, out char result)
        {
            CheckIndex(i);
            try
            {
                result = GetChar(i);
                return true;
            }
            catch (Exception)
            {
                result = default(char);
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Guid.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or Guid.Empty 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetGuid">GetGuid</see> method.</remarks>
        public bool TryGetGuid(int i, out Guid result)
        {
            CheckIndex(i);
            try
            {
                result = GetGuid(i);
                return true;
            }
            catch (Exception)
            {
                result = Guid.Empty;
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int16.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetInt16">GetInt16</see> method.</remarks>
        public bool TryGetInt16(int i, out short result)
        {
            CheckIndex(i);
            try
            {
                result = GetInt16(i);
                return true;
            }
            catch (Exception)
            {
                result = default(Int16);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int32.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetInt32">GetInt32</see> method.</remarks>
        public bool TryGetInt32(int i, out int result)
        {
            CheckIndex(i);
            try
            {
                result = GetInt32(i);
                return true;
            }
            catch (Exception)
            {
                result = default(int);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int64.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetInt64">GetInt64</see> method.</remarks>
        public bool TryGetInt64(int i, out Int64 result)
        {
            CheckIndex(i);
            try
            {
                result = GetInt64(i);
                return true;
            }
            catch (Exception)
            {
                result = default(Int64);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to float.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetFloat">GetFloat</see> method.</remarks>
        public bool TryGetFloat(int i, out float result)
        {
            CheckIndex(i);
            try
            {
                result = GetFloat(i);
                return true;
            }
            catch (Exception)
            {
                result = default(float);
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to double.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetDouble">GetDouble</see> method.</remarks>
        public bool TryGetDouble(int i, out double result)
        {
            CheckIndex(i);
            try
            {
                result = GetDouble(i);
                return true;
            }
            catch (Exception)
            {
                result = default(double);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to string.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetString">GetString</see> method.</remarks>
        public bool TryGetString(int i, out string result)
        {
            CheckIndex(i);
            try
            {
                result = GetString(i);
                return true;
            }
            catch (Exception)
            {
                result = default(string);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to decimal.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetDecimal">GetDecimal</see> method.</remarks>
        public bool TryGetDecimal(int i, out decimal result)
        {
            CheckIndex(i);
            try
            {
                result = GetDecimal(i);
                return true;
            }
            catch (Exception)
            {
                result = default(decimal);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTime.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetDateTime">GetDateTime</see> method.</remarks>
        public bool TryGetDateTime(int i, out DateTime result)
        {
            CheckIndex(i);
            try
            {
                result = GetDateTime(i);
                return true;
            }
            catch (Exception)
            {
                result = default(DateTime);
                return false;
            }

        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTimeOffset.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetDateTimeOffset">GetDateTimeOffset</see> method.</remarks>
        public bool TryGetDateTimeOffset(int i, out DateTimeOffset result)
        {
            CheckIndex(i);
            try
            {
                result = GetDateTimeOffset(i);
                return true;
            }
            catch (Exception)
            {
                result = new DateTimeOffset(default(DateTime));
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte array.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or null 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <remarks>Uses <see cref="GetByteArray">GetByteArray</see> method.</remarks>
        public bool TryGetByteArray(int i, out byte[] result)
        {
            CheckIndex(i);
            try
            {
                result = GetByteArray(i);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to an enum value.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="result">When this method returns, contains the converted value equivalent 
        /// of the value contained in the column, if the conversion succeeded, or default value 
        /// if the conversion failed.</param>
        /// <exception cref="IndexOutOfRangeException">The column index is out of range.</exception>
        /// <exception cref="ArgumentException">Type T is not an enumeration.</exception>
        /// <remarks>Uses <see cref="GetByteArray">GetByteArray</see> method.</remarks>
        public bool TryGetEnum<T>(int i, out T result)
        {
            CheckIndex(i);

            if (!typeof(T).IsEnum)
                throw new ArgumentException(string.Format(Properties.Resources.LightDataRow_ColumnValueIsNotEnumeration, typeof(T).FullName));

            try
            {
                result = GetEnum<T>(i);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }


        /// <summary>
        /// Gets the data stored in the column specified by index and converted to boolean.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetBoolean">TryGetBoolean</see> method.</remarks>
        public bool GetBooleanOrDefault(int i, bool defaultValue)
        {
            if (TryGetBoolean(i, out bool value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetByte">TryGetByte</see> method.</remarks>
        public byte GetByteOrDefault(int i, byte defaultValue)
        {
            if (TryGetByte(i, out byte value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to sbyte.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetSByte">TryGetSByte</see> method.</remarks>
        public sbyte GetSByteOrDefault(int i, sbyte defaultValue)
        {
            if (TryGetSByte(i, out sbyte value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to char.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetChar">TryGetChar</see> method.</remarks>
        public char GetCharOrDefault(int i, char defaultValue)
        {
            if (TryGetChar(i, out char value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Guid.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetGuid">TryGetGuid</see> method.</remarks>
        public Guid GetGuidOrDefault(int i, Guid defaultValue)
        {
            if (TryGetGuid(i, out Guid value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int16.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetInt16">TryGetInt16</see> method.</remarks>
        public short GetInt16OrDefault(int i, short defaultValue)
        {
            if (TryGetInt16(i, out short value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int32.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetInt32">TryGetInt32</see> method.</remarks>
        public int GetInt32OrDefault(int i, int defaultValue)
        {
            if (TryGetInt32(i, out int value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to Int64.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetInt64">TryGetInt64</see> method.</remarks>
        public Int64 GetInt64OrDefault(int i, Int64 defaultValue)
        {
            if (TryGetInt64(i, out long value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to float.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetFloat">TryGetFloat</see> method.</remarks>
        public float GetFloatOrDefault(int i, float defaultValue)
        {
            if (TryGetFloat(i, out float value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to double.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetDouble">TryGetDouble</see> method.</remarks>
        public double GetDoubleOrDefault(int i, double defaultValue)
        {
            if (TryGetDouble(i, out double value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to string.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetString">TryGetString</see> method.</remarks>
        public string GetStringOrDefault(int i, string defaultValue)
        {
            if (TryGetString(i, out string value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to decimal.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetDecimal">TryGetDecimal</see> method.</remarks>
        public decimal GetDecimalOrDefault(int i, decimal defaultValue)
        {
            if (TryGetDecimal(i, out decimal value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTime.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetDateTime">TryGetDateTime</see> method.</remarks>
        public DateTime GetDateTimeOrDefault(int i, DateTime defaultValue)
        {
            if (TryGetDateTime(i, out DateTime value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to DateTimeOffset.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetDateTimeOffset">TryGetDateTimeOffset</see> method.</remarks>
        public DateTimeOffset GetDateTimeOffsetOrDefault(int i, DateTimeOffset defaultValue)
        {
            if (TryGetDateTimeOffset(i, out DateTimeOffset value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to byte array.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetByteArray">TryGetByteArray</see> method.</remarks>
        public byte[] GetByteArrayOrDefault(int i, byte[] defaultValue)
        {
            if (TryGetByteArray(i, out byte[] value)) return value;
            return defaultValue;
        }

        /// <summary>
        /// Gets the data stored in the column specified by index and converted to an enum value.
        /// If the conversion fails, returns defaultValue.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration.</typeparam>
        /// <param name="i">The zero-based index of the column.</param>
        /// <param name="defaultValue">The value to return if the conversion fails.</param>
        /// <remarks>Uses <see cref="TryGetEnum{T}">">TryGetEnum</see> method.</remarks>
        public T GetEnumOrDefault<T>(int i, T defaultValue)
        {
            if (TryGetEnum<T>(i, out T value)) return value;
            return defaultValue;
        }


        private object GetValueForCast(int i, bool throwOnNull)
        {
            CheckIndex(i);
            if (throwOnNull && _data[i] == null)
                throw new InvalidCastException(Properties.Resources.LightDataRow_CannotCastTypeOnNull);
            return _data[i];
        }

        private bool TryReadString(byte[] source, out string result)
        {
            if (source == null || source.Length < 1)
            {
                result = string.Empty;
                return false;
            }
            try
            {
                result = Encoding.UTF8.GetString(source);
                return true;
            }
            catch (Exception)
            {
                result = string.Empty;
                return false;
            }
        }

        private bool TryParseDateTime(string source, out DateTime result)
        {
            if (_table.DateTimeFormats == null || _table.DateTimeFormats.Count <1)
            {
                return TryParseDateTime(source, LightDataTable.GetDefaultDateTimeFormats(), out result);
            }
            return TryParseDateTime(source, _table.DateTimeFormats.ToArray(), out result);
        }

        private bool TryParseDateTime(string source, string[] formats, out DateTime result)
        {

            if (formats == null || formats.Length < 1)
            {
                result = DateTime.MinValue;
                return false;
            }

            foreach (var format in formats)
            {
                try
                {
                    result = DateTime.ParseExact(source, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
                    return true;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception) { }
                // ReSharper restore EmptyGeneralCatchClause
            }

            result = DateTime.MinValue;
            return false;

        }

#endregion

        private void CheckIndex(int i)
        {
            if (i < 0 || (i + 1) > _data.Length)
                throw new IndexOutOfRangeException(string.Format(Properties.Resources.IndexValueOutOfRange, i.ToString()));
        }

        private static Object CloneValue(Object value)
        {

            if (value == null || value == DBNull.Value)
                return null;

            if (value.GetType().IsValueType)
                return value;

            if (value.GetType() == typeof(string))
                return new string(((string)value).ToCharArray());

            if (value.GetType() == typeof(Byte[]))
                return ((Byte[])value).Clone();

            try
            {
                return value.GetType().InvokeMember("Clone", BindingFlags.Instance
                    | BindingFlags.Public, null, value, null);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(string.Format(Properties.Resources.LightDataRow_ReferenceTypeWithoutCloneMethod,
                    value.GetType().FullName), ex);
            }

        }

        internal LightDataRowProxy GetLightDataRowProxy()
        {

            var values = new Object[_data.Length];
            for (int i=0; i < _data.Length; i++)
            {
                values[i] = _data[i];
            }

            var result = new LightDataRowProxy {Values = values};

            return result;

        }

    }
}
