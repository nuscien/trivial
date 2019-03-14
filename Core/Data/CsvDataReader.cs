using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Trivial.Text;
using Trivial.IO;

namespace Trivial.Data
{
    /// <summary>
    /// CSV data reader.
    /// </summary>
    public class CsvDataReader : ReadOnlyStringTableDataReader
    {
        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public CsvDataReader(StreamReader reader, bool hasHeader = false) : base(Parse(reader), null, null, hasHeader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="columns">The columns information</param>
        public CsvDataReader(StreamReader reader, IReadOnlyList<DbColumnInfo> columns) : base(Parse(reader), columns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        public CsvDataReader(StreamReader reader, IReadOnlyList<string> columnNames, string columnTypeName = null) : base(Parse(reader), columnNames, columnTypeName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public CsvDataReader(IEnumerable<string> lines, bool hasHeader = false) : base(Parse(lines), null, null, hasHeader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columns">The columns information</param>
        public CsvDataReader(IEnumerable<string> lines, IReadOnlyList<DbColumnInfo> columns) : base(Parse(lines), columns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="lines">Lines to read.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        public CsvDataReader(IEnumerable<string> lines, IReadOnlyList<string> columnNames, string columnTypeName = null) : base(Parse(lines), columnNames, columnTypeName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="text">A CSV format string.</param>
        /// <param name="hasHeader">true if the first is the table header, and the column names will be replaced by the header; otherwise, false.</param>
        public CsvDataReader(string text, bool hasHeader = false) : base(Parse(text), null, null, hasHeader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="text">A CSV format string.</param>
        /// <param name="columns">The columns information</param>
        public CsvDataReader(string text, IReadOnlyList<DbColumnInfo> columns) : base(Parse(text), columns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvDataReader class.
        /// </summary>
        /// <param name="text">A CSV format string.</param>
        /// <param name="columnNames">The names of column.</param>
        /// <param name="columnTypeName">The type name of each column.</param>
        public CsvDataReader(string text, IReadOnlyList<string> columnNames, string columnTypeName = null) : base(Parse(text), columnNames, columnTypeName)
        {
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV text.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(IEnumerable<string> csv)
        {
            foreach (var line in csv)
            {
                var item = ParseLine(line);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV text.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(string csv)
        {
            return Parse(StringUtility.YieldSplit(csv, '\r', '\n', StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The stream contains CSV.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(Stream csv, Encoding encoding)
        {
            using (var reader = new StreamReader(csv, encoding))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo csv, Encoding encoding)
        {
            using (var reader = new StreamReader(csv.FullName, encoding))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV file.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo csv, bool detectEncodingFromByteOrderMarks)
        {
            using (var reader = new StreamReader(csv.FullName, detectEncodingFromByteOrderMarks))
            {
                return Parse(reader);
            }
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV stream reader.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(StreamReader csv)
        {
            while (true)
            {
                var line = csv.ReadLine();
                if (line == null) break;
                var item = ParseLine(line);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        private static List<string> ParseLine(string line)
        {
            if (string.IsNullOrEmpty(line)) return null;
            var arr = line.Split(',');
            if (line.IndexOf("\"") < 0) return arr.ToList();
            var list = new List<string>();
            var inScope = false;
            foreach (var item in arr)
            {
                if (!inScope)
                {
                    if (item.Length > 0 && item[0] == '"')
                    {
                        list.Add(item.Substring(1));
                        inScope = true;
                    }
                    else
                    {
                        list.Add(item);
                    }

                    continue;
                }

                if (item.Length > 0 && item[item.Length - 1] == '"' && (item.Length == 1 || item[item.Length - 2] != '\\'))
                {
                    list[list.Count - 1] += "," + item.Substring(0, item.Length - 1);
                    inScope = false;
                }
                else
                {
                    list[list.Count - 1] += "," + item;
                }
            }

            return list;
        }
    }
}
