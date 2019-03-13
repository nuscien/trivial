using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Data
{
    // <summary>
    /// Reads a forward-only stream of rows from a collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    public class ListDataReader<T> : DbDataReader
    {
        private bool isClosed = false;
        private IEnumerator<T> enumerator;

        /// <summary>
        /// Initializes a new instance of the ListDataReader class.
        /// </summary>
        /// <param name="collection">A list to load.</param>
        public ListDataReader(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                isClosed = true;
                return;
            }

            enumerator = collection.GetEnumerator();
        }

        /// <summary>
        /// Gets the column mapping.
        /// </summary>
        public ColumnMapping Mapping { get; } = new ColumnMapping();

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[int ordinal]
        {
            get
            {
                if (ordinal >= Mapping.Count) throw new IndexOutOfRangeException("ordinal is out of range.");
                var info = Mapping[ordinal];
                var current = enumerator.Current;
                return current.GetType().GetProperty(info.PropertyName).GetValue(current);
            }
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[string name]
        {
            get
            {
                var info = Mapping.FirstOrDefault((item) => item.ColumnName == name);
                if (info == null) throw new IndexOutOfRangeException("ordinal is out of range.");
                var current = enumerator.Current;
                return current.GetType().GetProperty(info.PropertyName).GetValue(current);
            }
        }


        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        public override int Depth
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public override int FieldCount
        {
            get
            {
                return Mapping.Count;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this data reader contains one or more rows.
        /// </summary>
        public override bool HasRows
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed
        {
            get
            {
                return isClosed;
            }
        }

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the statement.
        /// </summary>
        public override int RecordsAffected
        {
            get
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal)
        {
            return (bool)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal)
        {
            return (byte)this[ordinal];
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of bytes read.</returns>
        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the specified column as a charactor.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal)
        {
            return (char)this[ordinal];
        }

        /// <summary>
        /// Reads a stream of characters from the specified column, starting at location indicated by dataOffset, into the buffer, starting at the location indicated by bufferOffset.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <param name="dataOffset">The index within the row from which to begin the read operation.</param>
        /// <param name="buffer">The buffer into which to copy the data.</param>
        /// <param name="bufferOffset">The index with the buffer to which the data will be copied.</param>
        /// <param name="length">The maximum number of characters to read.</param>
        /// <returns>The actual number of characters read.</returns>
        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets name of the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>A string representing the name of the data type.</returns>
        public override string GetDataTypeName(int ordinal)
        {
            return GetFieldType(ordinal).Name;
        }

        /// <summary>
        /// Gets the value of the specified column as a DateTime.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override DateTime GetDateTime(int ordinal)
        {
            return (DateTime)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a Decimal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal)
        {
            return (decimal)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a Double.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal)
        {
            return (double)this[ordinal];
        }

        /// <summary>
        /// Returns an System.Collections.IEnumerator that can be used to iterate through the rows in the data reader.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator that can be used to iterate through the rows in the data reader.</returns>
        public override IEnumerator GetEnumerator()
        {
            return enumerator;
        }

        /// <summary>
        /// Gets the data type of the specified column.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The data type of the specified column.</returns>
        public override Type GetFieldType(int ordinal)
        {
            return this[ordinal].GetType();
        }

        /// <summary>
        /// Synchronously gets the value of the specified column as a type.
        /// </summary>
        /// <typeparam name="TValue">Synchronously gets the value of the specified column as a type.</typeparam>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override TValue GetFieldValue<TValue>(int ordinal)
        {
            return (TValue)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a Float.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal)
        {
            return (float)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal)
        {
            var result = this[ordinal];
            var str = result as string;
            if (str != null) return Guid.Parse(str);
            return (Guid)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal)
        {
            return (short)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal)
        {
            return (int)this[ordinal];
        }

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal)
        {
            return (long)this[ordinal];
        }

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal)
        {
            if (ordinal >= Mapping.Count) throw new IndexOutOfRangeException("ordinal is out of range.");
            var info = Mapping[ordinal];
            return info.ColumnName;
        }

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
        {
            var index = 0;
            foreach (var item in Mapping)
            {
                if (item.ColumnName == name) break;
                index++;
            }

            return index < Mapping.Count ? index : -1;
        }

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the System.Data.Common.DbDataReader.
        /// </summary>
        /// <returns>A System.Data.DataTable that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            var table = new DataTable();
            foreach (var info in Mapping)
            {
                var prop = typeof(T).GetProperty(info.PropertyName);
                if (prop == null) continue;
                var row = table.NewRow();
                row["ColumnName"] = info.ColumnName;
                row["DataTypeName"] = prop.GetType().Name;
            }

            return table;
        }

        /// <summary>
        /// Gets the value of the specified column as a string.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal)
        {
            var result = this[ordinal];
            return result?.ToString();
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of System.Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of System.Object in the array.</returns>
        public override int GetValues(object[] values)
        {
            var index = 0;
            var current = enumerator.Current;
            if (current == null) return 0;
            foreach (var info in Mapping)
            {
                if (values.Length <= index) break;
                var prop = current.GetType().GetProperty(info.PropertyName);
                if (prop != null) values[index] = prop.GetValue(current);
                index++;
            }

            return index;
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object GetValue(int ordinal)
        {
            return this[ordinal];
        }

        /// <summary>
        /// Gets a value that indicates whether the column contains nonexistent or missing values.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>true if the specified column is equivalent to System.DBNull; otherwise false.</returns>
        public override bool IsDBNull(int ordinal)
        {
            return this[ordinal] == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            var result = enumerator.MoveNext();
            isClosed = !result;
            return result;
        }

        /// <summary>
        /// Resets the position read.
        /// </summary>
        public void ResetPosition()
        {
            enumerator.Reset();
            isClosed = false;
        }
    }
}
