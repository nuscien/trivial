using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data
{
    /// <summary>
    /// Lines format string table parser.
    /// </summary>
    public abstract class LinesStringTableParser : IEnumerable<IReadOnlyList<string>>
    {
        private readonly IEnumerable<IReadOnlyList<string>> text;

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        public LinesStringTableParser(StreamReader reader)
        {
            text = Parse(reader);
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(string lines)
        {
            text = Parse(lines);
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(IEnumerable<string> lines)
        {
            text = Parse(lines);
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(IEnumerable<IReadOnlyList<string>> lines)
        {
            text = lines;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IReadOnlyList<string>> GetEnumerator()
        {
            return text.GetEnumerator();
        }

        /// <summary>
        /// Returns the typed collection.
        /// </summary>
        /// <typeparam name="T">The type of each instance in the collection.</typeparam>
        /// <param name="creator">The instance factory.</param>
        /// <param name="propertyNames">The optional property names to map.</param>
        /// <returns>A typed collection based on the data parsed.</returns>
        public IEnumerable<T> ConvertTo<T>(Func<IReadOnlyList<string>, T> creator, IEnumerable<string> propertyNames = null)
        {
            var type = typeof(T);
            var props = propertyNames?.Select(ele =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(ele)) return type.GetProperty(ele);
                }
                catch (AmbiguousMatchException)
                {
                }

                return null;
            })?.ToList();
            if (props != null && props.Count == 0) props = null;
            if (creator == null && props == null) yield break;
            foreach (var item in this)
            {
                var instance = TypeUtility.ConvertTo(item, creator, props);
                if (instance == null) continue;
                yield return instance;
            }
        }

        /// <summary>
        /// Returns the typed collection.
        /// </summary>
        /// <typeparam name="T">The type of each instance in the collection.</typeparam>
        /// <param name="propertyNames">The property names to map.</param>
        /// <returns>A typed collection based on the data parsed.</returns>
        public IEnumerable<T> ConvertTo<T>(IEnumerable<string> propertyNames)
        {
            return ConvertTo<T>(null, propertyNames);
        }

        /// <summary>
        /// Parses a line.
        /// </summary>
        /// <param name="line">A line.</param>
        /// <returns>The list of fields.</returns>
        protected abstract List<string> ParseLine(string line);

        /// <summary>
        /// Parses from lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>Content with table.</returns>
        private IEnumerable<IReadOnlyList<string>> Parse(IEnumerable<string> lines)
        {
            if (lines == null) return new List<IReadOnlyList<string>>();
            return lines.Select(ParseLine);
        }

        /// <summary>
        /// Parses from a string.
        /// </summary>
        /// <param name="text">The lines.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">reader was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(string text)
        {
            return Parse(StringUtility.YieldSplit(text, '\r', '\n', StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Parses from a stream reader.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">reader was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(StreamReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "reader could not be null.");
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                var item = ParseLine(line);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }

        /// <summary>
        /// Parses from a text stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">stream was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(Stream stream, Encoding encoding)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream), "stream could not be null.");
            using (var reader = new StreamReader(stream, encoding))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses from a text stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">stream was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream), "stream could not be null.");
            using (var reader = encoding != null ? new StreamReader(stream, detectEncodingFromByteOrderMarks) : new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses from a text stream.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">file was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(FileInfo file, Encoding encoding)
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "file could not be null.");
            using (var reader = encoding != null ? new StreamReader(file.FullName, encoding) : new StreamReader(file.FullName))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses from a text stream.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">file was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null)
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "file could not be null.");
            using (var reader = encoding != null ? new StreamReader(file.FullName, detectEncodingFromByteOrderMarks) : new StreamReader(file.FullName, encoding, detectEncodingFromByteOrderMarks))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return text.GetEnumerator();
        }
    }

    /// <summary>
    /// String table data reader.
    /// </summary>
    public class StringTableDataReader : BaseListDataReader<IReadOnlyList<string>>
    {
        private readonly string defaultColumnTypeName;
        private readonly Func<DbColumnInfo, DbColumnInfo> columnResolver;
        private IReadOnlyList<DbColumnInfo> columnsInfo;
        private bool isInit = false;

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, bool hasHeader = false) : this(lines, columnNames, null, hasHeader)
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
        public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> columnNames, string columnTypeName, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
        {
            defaultColumnTypeName = columnTypeName;
            if (columnsInfo == null) return;
            columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, columnTypeName)).ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnInfoResolver">The column info resolver from the first line.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, Func<DbColumnInfo, DbColumnInfo> columnInfoResolver, IEnumerable<string> columnNames = null, string columnTypeName = null) : this(lines, columnNames, columnTypeName, true)
        {
            columnResolver = columnInfoResolver;
            defaultColumnTypeName = columnTypeName;
            if (columnsInfo == null) return;
            columnsInfo = columnNames.Select(ele => new DbColumnInfo(ele, typeof(string))).ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columns">The columns information</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<DbColumnInfo> columns, bool hasHeader = false) : base(lines, hasHeader ? 1 : 0)
        {
            if (columnsInfo == null) return;
            columnsInfo = columns.ToList().AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ReadOnlyStringTableDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnInfoResolver">The column info resolver from the first line.</param>
        /// <param name="columns">The columns information</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        public StringTableDataReader(IEnumerable<IReadOnlyList<string>> lines, Func<DbColumnInfo, DbColumnInfo> columnInfoResolver, IEnumerable<DbColumnInfo> columns = null, string columnTypeName = null) : this(lines, columns, true)
        {
            columnResolver = columnInfoResolver;
            defaultColumnTypeName = columnTypeName;
            if (columnsInfo == null) return;
            columnsInfo = columns.ToList().AsReadOnly();
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
        public override object this[int ordinal] => CurrentRecord[ordinal];

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
        /// Returns the typed collection.
        /// </summary>
        /// <typeparam name="T">The type of each instance in the collection.</typeparam>
        /// <param name="creator">The instance factory.</param>
        /// <param name="propertyNames">The property names to map.</param>
        /// <returns>A typed collection based on the data parsed.</returns>
        public T ConvertTo<T>(Func<IReadOnlyList<string>, T> creator, IEnumerable<string> propertyNames)
        {
            return TypeUtility.ConvertTo<T>(CurrentRecord, creator, propertyNames);
        }

        /// <summary>
        /// Returns the typed collection.
        /// </summary>
        /// <typeparam name="T">The type of each instance in the collection.</typeparam>
        /// <param name="propertyNames">The property names to map.</param>
        /// <returns>A typed collection based on the data parsed.</returns>
        public T ConvertTo<T>(IEnumerable<string> propertyNames)
        {
            return ConvertTo<T>(null, propertyNames);
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
                if (columnResolver == null) continue;
                var col = columnResolver(columns[i]);
                if (col != null) columns[i] = col;
            }

            columnsInfo = columns.AsReadOnly();
        }
    }
}
