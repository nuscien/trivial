using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trivial.Text
{
    /// <summary>
    /// The text parser for comma-separated values (RFC-4180) file format.
    /// </summary>
    public class CsvParser : BaseLinesStringTableParser
    {
        /// <summary>
        /// CSV MIME.
        /// </summary>
        public const string MIME = "text/csv";  // RFC 7111.

        /// <summary>
        /// The seperator per feild.
        /// </summary>
        private const char fieldSeperator = ',';

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public CsvParser(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public CsvParser(string lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public CsvParser(IEnumerable<string> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public CsvParser(IEnumerable<IReadOnlyList<string>> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public CsvParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public CsvParser(Stream stream, Encoding encoding = null) : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public CsvParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CsvParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public CsvParser(FileInfo file, Encoding encoding = null) : base(file, encoding)
        {
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        protected override List<string> ParseLine(string line)
        {
            return ParseLineStatic(line, fieldSeperator);
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <returns>A line string.</returns>
        protected override string ToLineString(IReadOnlyList<string> line)
        {
            return ToLineString(line, fieldSeperator);
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
                var item = ParseLineStatic(line, fieldSeperator);
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
            return Parse(StringExtensions.YieldSplit(csv, '\r', '\n', StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The stream contains CSV.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(Stream csv, Encoding encoding = null)
        {
            using var reader = new StreamReader(csv, encoding ?? Encoding.UTF8);
            return Parse(reader);
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV file.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo csv, Encoding encoding = null)
        {
            using var reader = new StreamReader(csv.FullName, encoding ?? Encoding.UTF8);
            return Parse(reader);
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV file.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo csv, bool detectEncodingFromByteOrderMarks)
        {
            using var reader = new StreamReader(csv.FullName, detectEncodingFromByteOrderMarks);
            return Parse(reader);
        }

        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV text reader.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(TextReader csv)
        {
            while (true)
            {
                var line = csv.ReadLine();
                if (line == null) break;
                var item = ParseLineStatic(line, fieldSeperator);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <param name="seperator">The field seperator.</param>
        /// <returns>A line string.</returns>
        internal static string ToLineString(IReadOnlyList<string> line, char seperator)
        {
            if (line == null || line.Count == 0) return null;
            var str = new StringBuilder();
            foreach (var field in line)
            {
                if (field.IndexOfAny(new[] { seperator, '\"' }) >= 0)
                {
                    str.Append('\"');
                    str.Append(field.Replace("\"", "\\\""));
                    str.Append('\"');
                }
                else
                {
                    str.Append(field);
                }

                str.Append(fieldSeperator);
            }

            if (str.Length == 0) return null;
            str.Remove(str.Length - 1, 1);
            return str.ToString();
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <param name="seperator">The field seperator.</param>
        /// <returns>Values in this line.</returns>
        internal static List<string> ParseLineStatic(string line, char seperator)
        {
            if (string.IsNullOrEmpty(line)) return null;
            var arr = line.Split(seperator);
            if (line.IndexOf("\"") < 0) return arr.ToList();
            var list = new List<string>();
            var inScope = false;
            foreach (var item in arr)
            {
                if (!inScope)
                {
                    if (item.Length > 0 && item[0] == '"')
                    {
                        if (item.Length > 1 && item[item.Length - 1] == '"' && item[item.Length - 2] != '\\')
                        {
                            list.Add(StringExtensions.ReplaceBackSlash(item.SubRangeString(1, 1, true)));
                        }
                        else
                        {
                            list.Add(StringExtensions.ReplaceBackSlash(item.Substring(1)));
                            inScope = true;
                        }
                    }
                    else
                    {
                        list.Add(StringExtensions.ReplaceBackSlash(item));
                    }

                    continue;
                }

                if (item.Length > 0 && item[item.Length - 1] == '"' && (item.Length == 1 || item[item.Length - 2] != '\\'))
                {
                    list[list.Count - 1] += seperator + StringExtensions.ReplaceBackSlash(item.SubRangeString(0, 1, true));
                    inScope = false;
                }
                else
                {
                    list[list.Count - 1] += seperator + StringExtensions.ReplaceBackSlash(item);
                }
            }

            return list;
        }
    }
}
