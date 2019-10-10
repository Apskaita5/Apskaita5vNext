using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a collection of LightDataColumn objects for a LightDataTable.
    /// </summary>
    public sealed class LightDataColumnCollection : ICollection<LightDataColumn>, IEnumerable<LightDataColumn>
    {

        private const string DefaultColumnName = "Column({0})";

        private readonly List<LightDataColumn> _list = new List<LightDataColumn>();
        private readonly LightDataTable _dataTable = null;


        /// <summary>
        /// Gets the total number of elements in a collection.
        /// </summary>
        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection is read-only. (implements ICollection)
        /// </summary>
        /// <returns>Returns false if the parent table contains any row, otherwise returns true.</returns>
        /// <remarks>Implements ICollection.</remarks>
        public bool IsReadOnly
        {
            get
            {
                return _dataTable.Rows.Count > 0;
            }
        }

        /// <summary>
        /// Gets the LightDataColumn from the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the column to return.</param>
        /// <returns>The DataColumn at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">The index value is greater than the number of items in the collection.</exception>
        public LightDataColumn this[int index]
        {
            get
            {
                if (_list.Count < 1 || index < 0 || (index + 1) > _list.Count)
                    throw new IndexOutOfRangeException(String.Format(Properties.Resources.IndexValueOutOfRange,
                        index.ToString(CultureInfo.InvariantCulture)));
                return _list[index];
            }
        }

        /// <summary>
        /// Gets the LightDataColumn from the collection with the specified name.
        /// </summary>
        /// <param name="name">The ColumnName of the column to return.</param>
        /// <returns>The DataColumn in the collection with the specified ColumnName; 
        /// otherwise a null value if the DataColumn does not exist.
        /// Item is not case-sensitive when it searches for column names.</returns>
        /// <exception cref="ArgumentNullException">A parameter name is null.</exception>
        public LightDataColumn this[string name]
        {
            get
            {
                int colIndex = this.IndexOf(name);
                if (colIndex < 0) return null;
                return _list[colIndex];
            }
        }


        /// <summary>
        /// Initializes a new instance of LightDataColumnCollection.
        /// </summary>
        /// <param name="dataTable">A LightDataTable that the collection belongs to.</param>
        internal LightDataColumnCollection(LightDataTable dataTable)
        {
            _dataTable = dataTable;
            _list = new List<LightDataColumn>();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the List.
        /// </summary>
        public IEnumerator<LightDataColumn> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the List.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Copies the entire collection into an existing array, starting at a specified index within the array.
        /// </summary>
        /// <param name="array">An array of LightDataColumn objects to copy the collection into.</param>
        /// <param name="index">The index to start from.</param>
        public void CopyTo(Array array, int index)
        {

            if (_list.Count < 1) return;

            Int32 currentIndex = index;

            foreach (var column in _list)
            {
                array.SetValue(column, currentIndex);
                currentIndex += 1;
            }

        }

        /// <summary>
        /// Copies the entire collection into an existing array, starting at a specified index within the array.
        /// </summary>
        /// <param name="array">An array of LightDataColumn objects to copy the collection into.</param>
        /// <param name="arrayIndex">The index to start from.</param>
        public void CopyTo(LightDataColumn[] array, int arrayIndex)
        {

            if (_list.Count < 1) return;

            Int32 currentIndex = arrayIndex;

            foreach (var column in _list)
            {
                array.SetValue(column, currentIndex);
                currentIndex += 1;
            }

        }

        /// <summary>
        /// Adds the specified LightDataColumn object to the LightDataColumnCollection.
        /// </summary>
        /// <param name="column">The LightDataColumn to add.</param>
        /// <exception cref="InvalidOperationException">Columns cannot be added to a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        /// <exception cref="ArgumentNullException">The column parameter is null.</exception>
        /// <exception cref="ArgumentException">The column already belongs to this collection, 
        /// or to another collection.</exception>
        /// <exception cref="ArgumentException">The collection already has a column with the specified name. 
        /// (The comparison is not case-sensitive.)</exception>
        public void Add(LightDataColumn column)
        {
            if (this.IsReadOnly)
                throw new InvalidOperationException(Properties.Resources.LightDataColumnCollection_CannotAddColumnToReadOnlyCollection);
            if (column.IsNull()) throw new ArgumentNullException(nameof(column));
            if (!column.Table.IsNull())
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnAlreadyAdded);
            if (this.Contains(column.ColumnName))
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnNameAlreadyExists);

            column.Table = _dataTable;

            _list.Add(column);

        }

        /// <summary>
        /// Creates and adds a LightDataColumn object that has the specified name, caption and type to the DataColumnCollection.
        /// </summary>
        /// <param name="columnName">The ColumnName to use when you create the column.</param>
        /// <param name="caption">The Caption to use when you create the column.</param>
        /// <param name="dataType">The DataType to use when you create the column.</param>
        /// <returns>The newly created LightDataColumn.</returns>
        /// <exception cref="ArgumentNullException">The columnName parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The dataType parameter is null.</exception>
        /// <exception cref="ArgumentException">The collection already has a column with the specified name. 
        /// (The comparison is not case-sensitive.)</exception>
        public LightDataColumn Add(string columnName, string caption, Type dataType)
        {

            if (columnName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(columnName));
            if (this.Contains(columnName))
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnNameAlreadyExists);

            var result = new LightDataColumn(columnName, dataType) { Caption = caption };

            this.Add(result);

            return result;

        }

        /// <summary>
        /// Creates and adds a LightDataColumn object that has the specified name and type to the DataColumnCollection.
        /// </summary>
        /// <param name="columnName">The ColumnName to use when you create the column.</param>
        /// <param name="dataType">The DataType to use when you create the column.</param>
        /// <returns>The newly created LightDataColumn.</returns>
        /// <exception cref="ArgumentNullException">The columnName parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The dataType parameter is null.</exception>
        /// <exception cref="ArgumentException">The collection already has a column with the specified name. 
        /// (The comparison is not case-sensitive.)</exception>
        public LightDataColumn Add(string columnName, Type dataType)
        {

            if (columnName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(columnName));
            if (this.Contains(columnName))
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnNameAlreadyExists);

            var result = new LightDataColumn(columnName, dataType);

            this.Add(result);

            return result;

        }

        /// <summary>
        /// Creates and adds a LightDataColumn object that has the specified name to the DataColumnCollection.
        /// </summary>
        /// <param name="columnName">The ColumnName to use when you create the column.</param>
        /// <returns>The newly created LightDataColumn.</returns>
        /// <exception cref="ArgumentNullException">The columnName parameter is null.</exception>
        /// <exception cref="ArgumentException">The collection already has a column with the specified name. 
        /// (The comparison is not case-sensitive.)</exception>
        public LightDataColumn Add(string columnName)
        {

            if (columnName.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(columnName));
            if (this.Contains(columnName))
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnNameAlreadyExists);

            var result = new LightDataColumn(columnName);

            this.Add(result);

            return result;

        }

        /// <summary>
        /// Creates and adds a LightDataColumn object that has the specified type and a default name to the DataColumnCollection.
        /// </summary>
        /// <param name="dataType">The DataType to use when you create the column.</param>
        /// <returns>The newly created LightDataColumn.</returns>
        ///  <exception cref="ArgumentNullException">The dataType parameter is null.</exception> 
        public LightDataColumn Add(Type dataType)
        {     
            var result = new LightDataColumn(GetDefaultColumnName(), dataType)
            { Table = _dataTable };
            this.Add(result);      
            return result;         
        }

        /// <summary>
        /// Creates and adds a LightDataColumn object that has a default name to the LightDataColumnCollection.
        /// </summary>
        /// <returns>The newly created LightDataColumn.</returns>
        public LightDataColumn Add()
        {
            var result = new LightDataColumn(GetDefaultColumnName()) { Table = _dataTable };
            this.Add(result);                                                               
            return result;
        }

        /// <summary>
        ///  Adds the elements of the specified LightDataColumn array to the end of the collection.
        /// </summary>
        /// <param name="columns">The array of DataColumn objects to add to the collection.</param>
        /// <exception cref="InvalidOperationException">Columns cannot be added to a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        /// <exception cref="ArgumentNullException">The columns parameter is null.</exception>
        /// <exception cref="ArgumentException">A column in the array already belongs to this collection, 
        /// or to another collection.</exception>
        /// <exception cref="ArgumentException">The collection already has a column with the specified name. 
        /// (The comparison is not case-sensitive.)</exception>
        public void AddRange(LightDataColumn[] columns)
        {

            if (this.IsReadOnly)
                throw new InvalidOperationException(Properties.Resources.LightDataColumnCollection_CannotAddColumnToReadOnlyCollection);
            if (null == columns)
                throw new ArgumentNullException(nameof(columns));

            // because all or no columns should be added
            foreach (var column in columns)
            {
                if (column.Table != null)
                    throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnAlreadyAdded);
                if (this.Contains(column.ColumnName))
                    throw new ArgumentException(Properties.Resources.LightDataColumnCollection_ColumnNameAlreadyExists);
            }

            foreach (var column in columns)
            {
                column.Table = _dataTable;
                _list.Add(column);
            }

        }

        /// <summary>
        /// Creates and adds LightDataColumn objects using the data reader specified.
        /// </summary>
        /// <param name="reader">a data reader to use</param>
        /// <remarks>should invoke reader.Read() once before passing the reader to this method</remarks>
        internal void Add(IDataReader reader)
        {
            if (reader.IsNull()) throw new ArgumentNullException(nameof(reader));

            if (reader.FieldCount < 1) return;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var column = new LightDataColumn(reader.GetName(i), reader.GetFieldType(i))
                { Table = _dataTable, NativeDataType = reader.GetDataTypeName(i) };
                _list.Add(column);
            }

        }

        /// <summary>
        /// Clears the collection of any columns.
        /// </summary>
        ///  <exception cref="InvalidOperationException">Columns cannot be removed from a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        public void Clear()
        {  
            if (this.IsReadOnly)
                throw new InvalidOperationException(Properties.Resources.LightDataColumnCollection_CannotRemoveColumnFromReadOnlyCollection);
            _list.Clear();
        }

        /// <summary>
        /// Checks whether the collection contains the specified column.
        /// </summary>
        /// <param name="column">The column to look for.</param>
        /// <exception cref="ArgumentNullException">A parameter column is null.</exception>
        public bool Contains(LightDataColumn column)
        {
            if (column.IsNull()) throw new ArgumentNullException(nameof(column)); 
            return _list.Any(col => Object.ReferenceEquals(column, col));
        }

        /// <summary>
        /// Checks whether the collection contains a column with the specified name.
        /// </summary>
        /// <param name="name">A ColumnName of the column to look for.</param>
        /// <exception cref="ArgumentNullException">A parameter name is null.</exception>
        public bool Contains(string name)
        {
            if (name.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(name));
            return _list.Any(col => col.ColumnName.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Removes the specified LightDataColumn object from the collection.
        /// </summary>
        /// <param name="column">The LightDataColumn to remove.</param>
        /// <returns>true if the specified LightDataColumn object was removed from the collection,
        /// false otherwise (i.e. the specified LightDataColumn object not found in the collection)</returns>
        /// <exception cref="InvalidOperationException">Columns cannot be removed from a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        /// <exception cref="ArgumentNullException">A parameter column is not specified.</exception>
        public bool Remove(LightDataColumn column)
        {

            if (column.IsNull()) throw new ArgumentNullException(nameof(column));
            if (this.IsReadOnly)
                throw new InvalidOperationException(Properties.Resources.LightDataColumnCollection_CannotRemoveColumnFromReadOnlyCollection);

            return _list.Remove(column);

        }

        /// <summary>
        /// Removes the LightDataColumn object that has the specified name from the collection.
        /// </summary>
        /// <param name="name">The name of the column to remove.</param>
        /// <returns>true if the specified LightDataColumn object was removed from the collection,
        /// false otherwise (i.e. the specified LightDataColumn object not found in the collection)</returns>
        /// <exception cref="InvalidOperationException">Columns cannot be removed from a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        /// <exception cref="ArgumentNullException">A parameter name is not specified.</exception>
        /// <exception cref="ArgumentException">The collection does not have a column with the specified name.</exception>
        public bool Remove(string name)
        {   
            var index = IndexOf(name);
            if (index < 0) throw new ArgumentException(Properties.Resources.LightDataColumnCollection_NoColumnByName);
            return this.Remove(_list[index]);
        }

        /// <summary>
        /// Removes the column at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index of the column to remove.</param>
        /// <returns>true if the specified LightDataColumn object was removed from the collection,
        /// false otherwise (i.e. the specified LightDataColumn object not found in the collection)</returns>
        /// <exception cref="InvalidOperationException">Columns cannot be removed from a readonly collection. 
        /// (The collection is readonly when any row is added to the parent table.)</exception>
        /// <exception cref="ArgumentNullException">A parameter index is not specified (should be zero or more).</exception>
        /// <exception cref="ArgumentException">The collection does not have a column at the specified index.</exception>
        public void RemoveAt(int index)
        {

            if (index < 0)
                throw new ArgumentNullException(nameof(index));

            if ((index + 1) > _list.Count)
                throw new ArgumentException(Properties.Resources.LightDataColumnCollection_NoColumnByIndex);

            this.Remove(_list[index]);

        }

        /// <summary>
        /// Gets the index of the column specified.
        /// </summary>
        /// <param name="column">The column to return the index for.</param>
        /// <returns>The index of the column specified if it is found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException">A parameter column is not specified.</exception>
        public int IndexOf(LightDataColumn column)
        {
            if (column.IsNull()) throw new ArgumentNullException(nameof(column)); 
            return _list.IndexOf(column);     
        }

        /// <summary>
        /// Gets the index of the column with the specific name (the name is not case sensitive).
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The zero-based index of the column with the specified name, 
        /// or -1 if the column does not exist in the collection.</returns>
        /// <exception cref="ArgumentNullException">A parameter name is not specified.</exception>
        public int IndexOf(string name)
        {
            if (name.IsNullOrWhiteSpace())
                throw new ArgumentNullException(nameof(name));

            foreach (var column in _list)
            {
                if (column.ColumnName.Trim().Equals(name.Trim(), StringComparison.OrdinalIgnoreCase))
                    return _list.IndexOf(column);
            }

            return -1;

        }



        private string GetDefaultColumnName()
        {
            int curIndex = 0;
            while (true)
            {
                string curName = string.Format(DefaultColumnName, curIndex.ToString(CultureInfo.InvariantCulture));
                if (!this.Contains(curName)) return curName;
                curIndex += 1;
            }
        }

        internal List<LightDataColumnProxy> ToProxyList()
        {
            var result = new List<LightDataColumnProxy>();
            foreach (var column in _list)
            {
                result.Add(column.GetLightDataColumnProxy());
            }
            return result;
        }

    }
}