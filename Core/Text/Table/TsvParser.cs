using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trivial.Text
{
    /// <summary>
    /// The text parser for tab-separated values file format.
    /// </summary>
    public class TsvParser : BaseLinesStringTableParser
    {
        /// <summary>
        /// CSV MIME.
        /// </summary>
        public const string MIME = "text/tsv";

        /// <summary>
        /// The seperator per feild.
        /// </summary>
        private const char fieldSeperator = '\t';

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public TsvParser(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public TsvParser(string lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public TsvParser(IEnumerable<string> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public TsvParser(IEnumerable<IReadOnlyList<string>> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public TsvParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public TsvParser(Stream stream, Encoding encoding = null) : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public TsvParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the TsvParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public TsvParser(FileInfo file, Encoding encoding = null) : base(file, encoding)
        {
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        protected override List<string> ParseLine(string line)
        {
            return CsvParser.ParseLineStatic(line, fieldSeperator);
        }

        /// <summary>
        /// Converts a line information to a string.
        /// </summary>
        /// <param name="line">The fields.</param>
        /// <returns>A line string.</returns>
        protected override string ToLineString(IReadOnlyList<string> line)
        {
            return CsvParser.ToLineString(line, fieldSeperator);
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
                var item = CsvParser.ParseLineStatic(line, fieldSeperator);
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
                var item = CsvParser.ParseLineStatic(line, fieldSeperator);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }
    }
}
