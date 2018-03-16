using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Apskaita5.DAL.Common
{
    /// <summary>
    /// Represents a collection of LightDataRow objects for a LightDataTable.
    /// </summary>
    [Serializable]
    public sealed class LightDataRowCollection : ICollection<LightDataRow>, IEnumerable<LightDataRow>
    {

        private readonly List<LightDataRow> _list = new List<LightDataRow>();
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
        /// <returns>Returns false.</returns>
        /// <remarks>Implements ICollection.</remarks>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the LightDataRow from the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the row to return.</param>
        /// <returns>The LightDataRow at the specified index.</returns>
        /// <exception cref="IndexOutOfRangeException">The index value is greater than the number of items in the collection.</exception>
        public LightDataRow this[int index]
        {
            get
            {
                if (_list.Count < 1 || index < 0 || (index + 1) > _list.Count)
                    throw new IndexOutOfRangeException(String.Format(Properties.Resources.LightDataRowCollection_IndexOutOfRowRange,
                        index.ToString(CultureInfo.InvariantCulture)));
                return _list[index];
            }
        }


        /// <summary>
        /// Initializes a new instance of LightDataRowCollection.
        /// </summary>
        /// <param name="dataTable">A LightDataTable that the collection belongs to.</param>
        internal LightDataRowCollection(LightDataTable dataTable)
        {
            _dataTable = dataTable;
            _list = new List<LightDataRow>();
        }


        /// <summary>
        /// Returns an enumerator that iterates through the List.
        /// </summary>
        public IEnumerator<LightDataRow> GetEnumerator()
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
        /// Adds the specified LightDataRow object to the LightDataRowCollection.
        /// </summary>
        /// <param name="row">The LightDataRow to add.</param>
        /// <exception cref="ArgumentNullException">The row parameter is null.</exception>
        /// <exception cref="ArgumentException">The row was created for different table.</exception>
        public void Add(LightDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            if (!Object.ReferenceEquals(_dataTable, row.Table))
                throw new ArgumentException(Properties.Resources.LightDataRowCollection_RowBelongsToOtherTable);

            _list.Add(row);

        }

        /// <summary>
        /// Creates and adds a new LightDataRow object that is filled with the values in the specified array.
        /// </summary>
        /// <param name="values">values to set</param>
        /// <exception cref="ArgumentNullException">Parameter values is not specified.</exception>
        /// <exception cref="InvalidOperationException">Parent table has no columns.</exception>
        /// <exception cref="ArgumentException">Value array field count does not match table column count.</exception>
        public LightDataRow Add(Object[] values)
        {

            var result = new LightDataRow(_dataTable, values);

            this.Add(result);

            return result;

        }

        /// <summary>
        /// Creates and adds an empty LightDataRow object to the LightDataRowCollection.
        /// </summary>
        /// <returns>The newly created LightDataRow.</returns>
        public LightDataRow Add()
        {

            var result = new LightDataRow(_dataTable);

            this.Add(result);

            return result;

        }

        /// <summary>
        /// Adds new rows using the data reader.
        /// </summary>
        /// <param name="reader">data reader to use</param>
        /// <remarks>should invoke reader.Read() once before passing the reader to this method</remarks>
        internal void Add(IDataReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            do
            {
                _list.Add(new LightDataRow(_dataTable, reader));
            } while (reader.Read());
        }

        /// <summary>
        /// Clears the collection of any rows.
        /// </summary>
        public void Clear()
        {
            _list.Clear();
        }

        /// <summary>
        /// Checks whether the collection contains the specified row.
        /// </summary>
        /// <param name="row">The row to look for.</param>
        /// <exception cref="ArgumentNullException">A parameter row is null.</exception>
        public bool Contains(LightDataRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));
            return _list.Any(r => Object.ReferenceEquals(row, r));
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="array">An array of LightDataRow objects to copy the collection into.</param>
        /// <param name="arrayIndex">The index to start from.</param>
        public void CopyTo(LightDataRow[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified LightDataRow object from the collection.
        /// </summary>
        /// <param name="row">The LightDataRow to remove.</param>
        /// <returns>true if the specified LightDataRow object was removed from the collection,
        /// false otherwise (i.e. the specified LightDataRow object not found in the collection)</returns>
        /// <exception cref="ArgumentNullException">A parameter row is not specified.</exception>
        public bool Remove(LightDataRow row)
        {

            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return _list.Remove(row);

        }

        /// <summary>
        /// Removes the row at the specified index from the collection.
        /// </summary>
        /// <param name="index">The index of the row to remove.</param>
        /// <returns>true if the specified LightDataRow object was removed from the collection,
        /// false otherwise (i.e. the specified LightDataRow object not found in the collection)</returns>
        /// <exception cref="ArgumentNullException">A parameter index is not specified (should be zero or more).</exception>
        /// <exception cref="ArgumentException">The collection does not have a row at the specified index.</exception>
        public void RemoveAt(int index)
        {

            if (index < 0)
                throw new ArgumentNullException(nameof(index));

            if ((index + 1) > _list.Count)
                throw new ArgumentException(Properties.Resources.LightDataRowCollection_NoRowAtIndex);

            this.Remove(_list[index]);

        }

    }
}