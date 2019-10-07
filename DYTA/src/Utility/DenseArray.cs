using System;
using System.Collections.Generic;

namespace DYTA.Utility
{
    public class DenseArray<T> : IEnumerable<T>
    {
        private T[] _dataarray;
        private int[] _columnindexes;

        /// <summary>
        /// Calculates the column indexes
        /// </summary>
        private void CalculateColumnIndexes()
        {
            _columnindexes = new int[Columns];
            for (int i = 0; i < Columns; i++)
            {
                _columnindexes[i] = i * Rows;
            }
        }

        /// <summary>
        /// Creates a new instance of DenseArray
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        public DenseArray(int rows, int columns)
        {
            _dataarray = new T[rows * columns];
            Rows = rows;
            Columns = columns;
            CalculateColumnIndexes();
        }

        /// <summary>
        /// Creates a new instance of DenseArray
        /// </summary>
        /// <param name="source">Source DenseArray</param>
        public DenseArray(DenseArray<T> source)
        {
            _dataarray = new T[source.Rows * source.Columns];
            Rows = source.Rows;
            Columns = source.Columns;
            Array.Copy(source._dataarray, this._dataarray, source._dataarray.LongLength);
            _columnindexes = new int[Columns];
            Array.Copy(source._columnindexes,
                       this._columnindexes,
                       source._columnindexes.LongLength);
        }

        /// <summary>
        /// Creates a new instance of DenseArray
        /// </summary>
        /// <param name="array">source 2d array</param>
        public DenseArray(T[,] array)
        {
            Rows = array.GetLength(0);
            Columns = array.GetLength(1);
            _dataarray = new T[Rows * Columns];
            CalculateColumnIndexes();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    this[i, j] = array[i, j];
                }
            }
        }

        /// <summary>
        /// Gets the number of columns
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// Gets the number of rows
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// Gets or sets an element of the array
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>Value at row and column index</returns>
        public T this[int row, int column]
        {
            get { return _dataarray[_columnindexes[column] + row]; }
            set { _dataarray[_columnindexes[column] + row] = value; }
        }

        /// <summary>
        /// Gets or sets an element of the array
        /// </summary>
        /// <param name="i">Index</param>
        /// <returns>Value at index</returns>
        public T this[int i]
        {
            get { return _dataarray[i]; }
            set { _dataarray[i] = value; }
        }

        /// <summary>
        /// IEnumerable implementation.
        /// </summary>
        /// <returns>internal array enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_dataarray.GetEnumerator();
        }

        /// <summary>
        /// IEnumerable Implementation
        /// </summary>
        /// <returns>internal array enumerator</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dataarray.GetEnumerator();
        }

        /// <summary>
        /// Gets a row as an Array
        /// </summary>
        /// <param name="rowindex">Row index</param>
        /// <returns>Row values in an array</returns>
        public T[] GetRow(int rowindex)
        {
            T[] ret = new T[this.Columns];
            for (int i = 0; i < this.Columns; i++)
            {
                ret[i] = this[rowindex, i];
            }
            return ret;
        }

        /// <summary>
        /// Gets a column as an Array
        /// </summary>
        /// <param name="columnindex">Column index</param>
        /// <returns>Column values in an array</returns>
        public T[] GetColumn(int columnindex)
        {
            T[] ret = new T[this.Rows];
            for (int i = 0; i < this.Rows; i++)
            {
                ret[i] = this[i, columnindex];
            }
            return ret;
        }

        /// <summary>
        /// Sets a row's values from an array
        /// </summary>
        /// <param name="row">the row index</param>
        /// <param name="items">items in an array</param>
        public void SetRow(int row, T[] items)
        {
            if (row < 0 || row > this.Rows)
                throw new ArgumentOutOfRangeException("row");
            if (items == null)
                throw new ArgumentNullException("items");

            int limit = System.Math.Min(items.Length, this.Columns);

            for (int i = 0; i < limit; i++)
            {
                this[row, i] = items[i];
            }
        }

        /// <summary>
        /// Sets a column's values from an array
        /// </summary>
        /// <param name="column">the column index</param>
        /// <param name="items">items in an array</param>
        public void SetColumn(int column, T[] items)
        {
            if (column < 0 || column > this.Columns)
                throw new ArgumentOutOfRangeException("column");
            if (items == null)
                throw new ArgumentNullException("items");

            int limit = System.Math.Min(items.Length, this.Rows);

            for (int i = 0; i < limit; i++)
            {
                this[i, column] = items[i];
            }
        }
    }
}