using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Trivial.Text;

namespace Trivial.Data
{
    /// <summary>
    /// The text parser for comma-separated values (RFC-4180) file format.
    /// </summary>
    public class CsvParser : LinesStringTableParser
    {
        /// <summary>
        /// CSV MIME.
        /// </summary>
        public const string MIME = "text/csv";  // RFC 7111.

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
            return ParseLineStatic(line);
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <returns>A line string.</returns>
        protected override string ToLineString(IReadOnlyList<string> line)
        {
            if (line == null || line.Count == 0) return null;
            var str = new StringBuilder();
            foreach (var field in line)
            {
                if (field.IndexOfAny(new[] { ',', '\"' }) >= 0)
                {
                    str.Append('\"');
                    str.Append(field.Replace("\"", "\\\""));
                    str.Append('\"');
                }
                else
                {
                    str.Append(field);
                }

                str.Append(',');
            }

            if (str.Length == 0) return null;
            str.Remove(str.Length - 1, 1);
            return str.ToString();
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
                var item = ParseLineStatic(line);
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
            return Parse(StringExtension.YieldSplit(csv, '\r', '\n', StringSplitOptions.RemoveEmptyEntries));
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
        /// <param name="csv">The CSV text reader.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<IReadOnlyList<string>> Parse(TextReader csv)
        {
            while (true)
            {
                var line = csv.ReadLine();
                if (line == null) break;
                var item = ParseLineStatic(line);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        private static List<string> ParseLineStatic(string line)
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
                        if (item.Length > 1 && item[item.Length - 1] == '"' && item[item.Length - 2] != '\\')
                        {
                            list.Add(StringExtension.ReplaceBackSlash(item.Substring(1, item.Length - 2)));
                        }
                        else
                        {
                            list.Add(StringExtension.ReplaceBackSlash(item.Substring(1)));
                            inScope = true;
                        }
                    }
                    else
                    {
                        list.Add(StringExtension.ReplaceBackSlash(item));
                    }

                    continue;
                }

                if (item.Length > 0 && item[item.Length - 1] == '"' && (item.Length == 1 || item[item.Length - 2] != '\\'))
                {
                    list[list.Count - 1] += "," + StringExtension.ReplaceBackSlash(item.Substring(0, item.Length - 1));
                    inScope = false;
                }
                else
                {
                    list[list.Count - 1] += "," + StringExtension.ReplaceBackSlash(item);
                }
            }

            return list;
        }
    }
}
