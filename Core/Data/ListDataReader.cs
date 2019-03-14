using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

using Trivial.Reflection;

namespace Trivial.Data
{
    /// <summary>
    /// The database column string information.
    /// </summary>
    public class DbColumnInfo
    {
        /// <summary>
        /// Initializes a new instance of the DbColumnInfo class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="typeName">The type name of the column.</param>
        public DbColumnInfo(string name, string typeName)
        {
            Name = name;
            TypeName = typeName;
        }

        /// <summary>
        /// Initializes a new instance of the DbColumnInfo class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="type">The type of the column.</param>
        public DbColumnInfo(string name, Type type) : this(name, type.Name)
        {
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type name of the column.
        /// </summary>
        public string TypeName { get; }
    }

    /// <summary>
    /// Reads a forward-only stream of rows from a collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    public abstract class BaseListDataReader<T> : DbDataReader
    {
        private IEnumerator<T> enumerator;

        /// <summary>
        /// Initializes a new instance of the BaseListDataReader class.
        /// </summary>
        /// <param name="collection">A list to load.</param>
        /// <param name="skip">The count of record to skip.</param>
        public BaseListDataReader(IEnumerable<T> collection, int skip = 0)
        {
            Skip = skip;
            if (collection == null)
            {
                IsEnd = true;
                return;
            }

            enumerator = collection.GetEnumerator();
        }

        /// <summary>
        /// The record count to skip.
        /// </summary>
        public int Skip { get; }

        /// <summary>
        /// Gets a value indicating whether it is end to read.
        /// </summary>
        public bool IsEnd { get; private set; }

        /// <summary>
        /// Gets a value indicating whether begin to read and the position is not reset.
        /// </summary>
        public bool HasBegunToRead { get; private set; }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[string name]
        {
            get
            {
                var i = GetOrdinal(name);
                if (i < 0) throw new IndexOutOfRangeException("name is not in the columns.");
                return this[i];
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
        public override int FieldCount => ColumnsInfoWithoutNull.Count;

        /// <summary>
        /// Gets a value that indicates whether this data reader contains one or more rows.
        /// </summary>
        public override bool HasRows => enumerator != null;

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        public override bool IsClosed => IsEnd;

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the statement.
        /// </summary>
        public override int RecordsAffected => -1;

        /// <summary>
        /// Gets the information of columns.
        /// </summary>
        public abstract IReadOnlyList<DbColumnInfo> ColumnsInfo { get; }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override bool GetBoolean(int ordinal)
        {
            return GetFieldValue<bool>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a byte.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override byte GetByte(int ordinal)
        {
            return GetFieldValue<byte>(ordinal);
        }

        /// <summary>
        /// Gets the current record instance.
        /// </summary>
        /// <returns>The current record instance.</returns>
        protected T CurrentRecord => enumerator.Current;

        /// <summary>
        /// Gets the information of columns.
        /// </summary>
        private IList<DbColumnInfo> ColumnsInfoWithoutNull => (ColumnsInfo ?? new List<DbColumnInfo>()).Where(ele => ele != null).ToList();

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
            var fieldType = GetFieldType(ordinal);
            if (fieldType == typeof(Stream) || fieldType.IsSubclassOf(typeof(Stream)))
            {
                var stream = GetFieldValue<Stream>(ordinal);
                if (dataOffset >= stream.Length) return 0;
                stream.Position = dataOffset;
                var len = Math.Min(length, stream.Length - dataOffset);
                stream.Write(buffer, bufferOffset, (int)len);
                return len;
            }

            var str = GetString(ordinal);
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(str);
                    writer.Flush();
                }

                if (dataOffset >= stream.Length) return 0;
                stream.Position = dataOffset;
                var len = Math.Min(length, stream.Length - dataOffset);
                stream.Write(buffer, bufferOffset, (int)len);
                return len;
            }
        }

        /// <summary>
        /// Gets the value of the specified column as a charactor.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override char GetChar(int ordinal)
        {
            return GetFieldValue<char>(ordinal);
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
            var str = GetString(ordinal);
            var len = Math.Min(length, str.Length - dataOffset);
            str.CopyTo((int)dataOffset, buffer, bufferOffset, (int)len);
            return len;
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
            return GetFieldValue<DateTime>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a Decimal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override decimal GetDecimal(int ordinal)
        {
            return GetFieldValue<decimal>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a Double.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override double GetDouble(int ordinal)
        {
            return GetFieldValue<double>(ordinal);
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
            return TypeUtility.ConvertTo<TValue>(this[ordinal]);
        }

        /// <summary>
        /// Gets the value of the specified column as a Float.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override float GetFloat(int ordinal)
        {
            return GetFieldValue<float>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a Guid.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override Guid GetGuid(int ordinal)
        {
            return GetFieldValue<Guid>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 16-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override short GetInt16(int ordinal)
        {
            return GetFieldValue<short>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 32-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override int GetInt32(int ordinal)
        {
            return GetFieldValue<int>(ordinal);
        }

        /// <summary>
        /// Gets the value of the specified column as a 64-bit signed integer.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override long GetInt64(int ordinal)
        {
            return GetFieldValue<long>(ordinal);
        }

        /// <summary>
        /// Gets the name of the column, given the zero-based column ordinal.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The name of the specified column.</returns>
        public override string GetName(int ordinal) => ColumnsInfoWithoutNull[ordinal]?.Name;

        /// <summary>
        /// Gets the column ordinal given the name of the column.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <returns>The zero-based column ordinal.</returns>
        public override int GetOrdinal(string name)
        {
            var index = -1;
            if (string.IsNullOrWhiteSpace(name)) return index;
            foreach (var item in ColumnsInfoWithoutNull)
            {
                index++;
                if (item.Name == name) return index;
            }

            return index;
        }

        /// <summary>
        /// Returns a System.Data.DataTable that describes the column metadata of the System.Data.Common.DbDataReader.
        /// </summary>
        /// <returns>A System.Data.DataTable that describes the column metadata.</returns>
        public override DataTable GetSchemaTable()
        {
            var table = new DataTable();
            foreach (var info in ColumnsInfoWithoutNull)
            {
                var row = table.NewRow();
                row["ColumnName"] = info.Name;
                row["DataTypeName"] = info.TypeName;
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
            return GetFieldValue<string>(ordinal);
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current row.
        /// </summary>
        /// <param name="values">An array of System.Object into which to copy the attribute columns.</param>
        /// <returns>The number of instances of System.Object in the array.</returns>
        public override int GetValues(object[] values)
        {
            var current = enumerator.Current;
            if (current == null) return 0;
            var len = Math.Min(ColumnsInfoWithoutNull.Count, values.Length);
            for (var i = 0; i < len; i++)
            {
                values[i] = this[i];
            }

            return len;
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

        /// <summary>
        /// Advances the reader to the next result when reading the results of a batch of statements.
        /// </summary>
        /// <returns>true if there are more result sets; otherwise false.</returns>
        public override bool NextResult()
        {
            return false;
        }

        /// <summary>
        /// Gets ready to read.
        /// </summary>
        public void ReadyToRead()
        {
            if (HasBegunToRead || IsEnd) return;
            HasBegunToRead = true;
            if (Skip < 1) return;
            var skip = 0;
            while (skip < Skip)
            {
                if (!enumerator.MoveNext())
                {
                    IsEnd = true;
                    return;
                }

                OnSkipRecord(enumerator.Current);
                skip++;
            }

            return;
        }

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns>true if there are more rows; otherwise false.</returns>
        public override bool Read()
        {
            if (enumerator == null) return false;
            ReadyToRead();
            if (IsEnd) return false;
            var result = enumerator.MoveNext();
            IsEnd = !result;
            OnReadRecord(enumerator.Current);
            return result;
        }

        /// <summary>
        /// Resets the position read.
        /// </summary>
        public void ResetPosition()
        {
            if (enumerator == null) return;
            enumerator.Reset();
            IsEnd = false;
            HasBegunToRead = true;
        }

        /// <summary>
        /// Processes on skipping record.
        /// </summary>
        /// <param name="record">The record to skip.</param>
        protected virtual void OnSkipRecord(T record)
        {
        }

        /// <summary>
        /// Processes on reading record.
        /// </summary>
        /// <param name="record">The record to read.</param>
        protected virtual void OnReadRecord(T record)
        {
        }
    }

    /// <summary>
    /// Reads a forward-only stream of rows from a collection.
    /// </summary>
    /// <typeparam name="T">The type of list item.</typeparam>
    public class ListDataReader<T> : BaseListDataReader<T>
    {
        private readonly ColumnMapping columnMapping;

        /// <summary>
        /// Initializes a new instance of the ListDataReader class.
        /// </summary>
        /// <param name="collection">A list to load.</param>
        /// <param name="mapping">The column mapping with properties.</param>
        public ListDataReader(IEnumerable<T> collection, ColumnMapping mapping) : base(collection)
        {
            columnMapping = mapping ?? new ColumnMapping();
        }

        /// <summary>
        /// Gets the information of columns.
        /// </summary>
        public override IReadOnlyList<DbColumnInfo> ColumnsInfo
        {
            get
            {
                var list = new List<DbColumnInfo>();
                foreach (var info in columnMapping)
                {
                    var prop = typeof(T).GetProperty(info.PropertyName);
                    if (prop == null) continue;
                    list.Add(new DbColumnInfo(info.ColumnName, prop.GetType().Name));
                }

                return list.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[int ordinal]
        {
            get
            {
                if (ordinal >= columnMapping.Count) throw new IndexOutOfRangeException("ordinal is out of range.");
                var info = columnMapping[ordinal];
                var current = CurrentRecord;
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
                var info = columnMapping.FirstOrDefault((item) => item.ColumnName == name);
                if (info == null) throw new IndexOutOfRangeException("ordinal is out of range.");
                var current = CurrentRecord;
                return current.GetType().GetProperty(info.PropertyName).GetValue(current);
            }
        }
    }

    /// <summary>
    /// String list data reader.
    /// </summary>
    public class ReadOnlyStringTableDataReader : BaseListDataReader<IReadOnlyList<string>>
    {
        private readonly string defaultColumnTypeName;
        private IReadOnlyList<DbColumnInfo> columnsInfo;
        private bool isInit = false;

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public ReadOnlyStringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, bool hasHeader = false) : base(lines, 1)
        {
            if (columnsInfo == null) return;
            columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, typeof(string))).ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public ReadOnlyStringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, string columnTypeName, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
        {
            defaultColumnTypeName = columnTypeName;
            if (columnsInfo == null) return;
            columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, columnTypeName)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columns">The columns information</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public ReadOnlyStringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IReadOnlyList<DbColumnInfo> columns, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
        {
            columnsInfo = columns;
        }

        /// <summary>
        /// Gets the current record instance.
        /// </summary>
        /// <returns>The current record instance.</returns>
        public new IReadOnlyList<string> CurrentRecord => base.CurrentRecord;

        /// <summary>
        /// Gets the information of columns.
        /// </summary>
        public override IReadOnlyList<DbColumnInfo> ColumnsInfo => columnsInfo;

        /// <summary>
        /// Gets the value of the specified column as an instance of System.Object.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override object this[int ordinal]
        {
            get
            {
                return CurrentRecord[ordinal];
            }
        }

        /// <summary>
        /// Gets the value of the specified column as a string.
        /// </summary>
        /// <param name="ordinal">The zero-based column ordinal.</param>
        /// <returns>The value of the specified column.</returns>
        public override string GetString(int ordinal)
        {
            return CurrentRecord[ordinal];
        }

        /// <summary>
        /// Processes on reading record.
        /// </summary>
        /// <param name="record">The record to read.</param>
        protected override void OnReadRecord(IReadOnlyList<string> record)
        {
            base.OnReadRecord(record);
            if (isInit) return;
            isInit = true;
            if (Skip < 1) return;
            var columns = ColumnsInfo != null ? new List<DbColumnInfo>() : new List<DbColumnInfo>(ColumnsInfo);
            for (var i = 0; i < record.Count; i++)
            {
                if (i <= columns.Count) columns.Add(new DbColumnInfo(record[i], defaultColumnTypeName ?? typeof(string).Name));
                else if (columns[i] == null) columns[i] = new DbColumnInfo(record[i], defaultColumnTypeName ?? typeof(string).Name);
                else columns[i] = new DbColumnInfo(record[i], columns[i].TypeName ?? defaultColumnTypeName ?? typeof(string).Name);
            }

            columnsInfo = columns.AsReadOnly();
        }
    }
}
