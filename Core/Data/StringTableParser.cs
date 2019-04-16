using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Text;

namespace Trivial.Data
{
    /// <summary>
    /// String table parser.
    /// </summary>
    public abstract class StringTableParser : IEnumerable<IReadOnlyList<string>>
    {
        private IEnumerable<IReadOnlyList<string>> text;

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public StringTableParser(string lines)
        {
            text = Parse(lines);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public StringTableParser(IEnumerable<string> lines)
        {
            text = Parse(lines);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public StringTableParser(IEnumerable<IReadOnlyList<string>> lines)
        {
            text = lines;
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public StringTableParser(TextReader reader)
        {
            if (reader == null) return;
            text = Parse(reader);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public StringTableParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null)
        {
            if (stream == null) return;
            text = Parse(stream, detectEncodingFromByteOrderMarks, encoding);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public StringTableParser(Stream stream, Encoding encoding = null)
        {
            if (stream == null) return;
            text = Parse(stream, encoding);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public StringTableParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null)
        {
            if (file == null) return;
            text = Parse(file, encoding);
        }

        /// <summary>
        /// Initializes a new instance of the StringTableParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public StringTableParser(FileInfo file, Encoding encoding = null)
        {
            if (file == null) return;
            text = Parse(file, encoding);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IReadOnlyList<string>> GetEnumerator()
        {
            if (text == null) text = new List<IReadOnlyList<string>>();
            return text.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <param name="creator">The instance factory.</param>
        /// <param name="propertyNames">The optional property names to map.</param>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator<T>(Func<IReadOnlyList<string>, T> creator, IEnumerable<string> propertyNames = null)
        {
            return ConvertTo(creator, propertyNames).GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <param name="propertyNames">The optional property names to map.</param>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator<T>(IEnumerable<string> propertyNames)
        {
            return ConvertTo<T>(propertyNames).GetEnumerator();
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
        /// Reads lines from a given text.
        /// </summary>
        /// <param name="text">The text includes multiple lines.</param>
        /// <returns>Lines.</returns>
        protected virtual IEnumerable<string> ReadLines(string text)
        {
            return StringExtensions.YieldSplit(text, '\r', '\n', StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Reads lines from a given text reader.
        /// </summary>
        /// <param name="reader">A text reader.</param>
        /// <returns>Lines.</returns>
        protected virtual IEnumerable<string> ReadLines(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "reader could not be null.");
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null) break;
                yield return line;
            }
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
            return Parse(ReadLines(text));
        }

        /// <summary>
        /// Parses from a text reader.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">reader was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(TextReader reader)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader), "reader could not be null.");
            return Parse(ReadLines(reader));
        }

        /// <summary>
        /// Parses from a text stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content with table.</returns>
        /// <exception cref="ArgumentNullException">stream was null.</exception>
        private IEnumerable<IReadOnlyList<string>> Parse(Stream stream, Encoding encoding = null)
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
            if (text == null) text = new List<IReadOnlyList<string>>();
            return text.GetEnumerator();
        }
    }

    /// <summary>
    /// Lines format string table parser.
    /// </summary>
    public abstract class LinesStringTableParser : StringTableParser
    {
        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(string lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(IEnumerable<string> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public LinesStringTableParser(IEnumerable<IReadOnlyList<string>> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public LinesStringTableParser(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public LinesStringTableParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public LinesStringTableParser(Stream stream, Encoding encoding = null) : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public LinesStringTableParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the LinesStringTableParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public LinesStringTableParser(FileInfo file, Encoding encoding = null) : base(file, encoding)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether need enable ToString() method to return the raw string in the source format of the parser.
        /// </summary>
        public bool IsSourceToStringEnabled { get; }

        /// <summary>
        /// Converts lines string and each line is in the source format of this parser.
        /// </summary>
        /// <returns>All lines.</returns>
        public IEnumerable<string> ToLinesString()
        {
            return this.Select(ToLineString).Where(ele => ele != null);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// The raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            if (!IsSourceToStringEnabled) return base.ToString();
            return ToRawString();
        }

        /// <summary>
        /// Returns the raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToRawString(string seperator = null)
        {
            return string.Join(seperator ?? Environment.NewLine, ToLinesString());
        }

        /// <summary>
        /// Returns the raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public string ToRawString(char seperator)
        {
            return ToRawString(seperator.ToString());
        }

        /// <summary>
        /// Returns the raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public Task<string> ToRawStringAsync(string seperator, CancellationToken cancellationToken = default)
        {
            if (seperator == null) seperator = Environment.NewLine;
            return Task.Run(() =>
            {
                var str = new StringBuilder();
                foreach (var item in ToLinesString())
                {
                    str.Append(item);
                    str.Append(seperator);
                    cancellationToken.ThrowIfCancellationRequested();
                }

                return str.ToString();
            });
        }

        /// <summary>
        /// Returns the raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public Task<string> ToRawStringAsync(char seperator, CancellationToken cancellationToken = default)
        {
            return ToRawStringAsync(seperator.ToString(), cancellationToken);
        }

        /// <summary>
        /// Returns the raw string in the source format of the parser if set IsSourceToStringEnabled property as True.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public Task<string> ToRawStringAsync(CancellationToken cancellationToken = default)
        {
            return ToRawStringAsync(null, cancellationToken);
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <returns>A line string.</returns>
        protected abstract string ToLineString(IReadOnlyList<string> line);
    }

    /// <summary>
    /// Fixed string table parser.
    /// </summary>
    public class FixedStringTableParser : LinesStringTableParser
    {
        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="lines">The lines.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, string lines) : base(lines)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="lines">The lines.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, IEnumerable<string> lines) : base(lines)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="lines">The lines.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, IEnumerable<IReadOnlyList<string>> lines) : base(lines)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="reader">The text reader.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, TextReader reader) : base(reader)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, Stream stream, Encoding encoding = null) : base(stream, encoding)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the FixedStringTableParser class.
        /// </summary>
        /// <param name="fieldLength">The collection of each field length.</param>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public FixedStringTableParser(IEnumerable<int> fieldLength, FileInfo file, Encoding encoding = null) : base(file, encoding)
        {
            FieldLength = fieldLength?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Gets the the information for each field length.
        /// </summary>
        public IReadOnlyList<int> FieldLength { get; }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        protected override List<string> ParseLine(string line)
        {
            if (FieldLength == null) return new List<string>{ line };
            var i = 0;
            var list = new List<string>();
            foreach (var len in FieldLength)
            {
                if (i + len >= line.Length)
                {
                    list.Add(line.Substring(i));
                    break;
                }

                list.Add(line.Substring(i, len));
                i += len;
            }

            while (list.Count < FieldLength.Count) list.Add(null);
            return list;
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <returns>A line string.</returns>
        protected override string ToLineString(IReadOnlyList<string> line)
        {
            if (line == null || line.Count == 0) return null;
            if (FieldLength == null) return line[0];
            var str = new StringBuilder();
            for (var i = 0; i < Math.Min(line.Count, FieldLength.Count); i++)
            {
                var field = line[i];
                var len = FieldLength[i];
                str.Append(FixField(line[i], i, len));
            }

            return str.ToString();
        }

        /// <summary>
        /// Fixes the field.
        /// </summary>
        /// <param name="field">The original field.</param>
        /// <param name="index">The field index.</param>
        /// <param name="length">The length of field.</param>
        /// <returns>The fixed field.</returns>
        protected virtual string FixField(string field, int index, int length)
        {
            if (field == null) field = string.Empty;
            if (field.Length > length) return field.Substring(0, length);
            return field.PadLeft(length - field.Length);
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
            if (columnNames == null) return;
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
            if (columnNames == null) return;
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
            if (columnNames == null) return;
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
            if (columns == null) return;
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
            if (columns == null) return;
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
