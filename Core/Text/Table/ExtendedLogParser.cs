using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trivial.Text
{
    /// <summary>
    /// The text parser for log file (W3C WD-logfile-960323) format.
    /// </summary>
    public class ExtendedLogParser : BaseLinesStringTableParser
    {
        /// <summary>
        /// The seperator per feild.
        /// </summary>
        private const char fieldSeperator = ' ';

        /// <summary>
        /// THe cache of directive.
        /// </summary>
        private readonly Dictionary<string, string> directives = new();

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="reader">The text reader.</param>
        public ExtendedLogParser(TextReader reader) : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public ExtendedLogParser(string lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public ExtendedLogParser(IEnumerable<string> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        public ExtendedLogParser(IEnumerable<IReadOnlyList<string>> lines) : base(lines)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <param name="headers">The headers.</param>
        public ExtendedLogParser(IEnumerable<IReadOnlyList<string>> lines, IEnumerable<string> headers) : base(lines)
        {
            Headers = headers?.ToList()?.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public ExtendedLogParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public ExtendedLogParser(Stream stream, Encoding encoding = null) : base(stream, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public ExtendedLogParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExtendedLogParser class.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The optional character encoding to use.</param>
        public ExtendedLogParser(FileInfo file, Encoding encoding = null) : base(file, encoding)
        {
        }

        /// <summary>
        /// Gets the fields names.
        /// </summary>
        public IReadOnlyList<string> Headers { get; private set; }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <returns>The value of directive; or null, if not found.</returns>
        public string GetDirectiveValue(string key)
        {
            if (directives.TryGetValue(key, out var value)) return value;
            return null;
        }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains the directive; otherise, false.</returns>
        public bool TryGetDirectiveValue(string key, out string result)
        {
            return directives.TryGetValue(key, out result);
        }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains the directive with a date time value; otherise, false.</returns>
        public bool TryGetDirectiveValue(string key, out DateTime result)
        {
            if (directives.TryGetValue(key, out var value))
            {
                var dt = Web.WebFormat.ParseDate(value);
                if (dt.HasValue)
                {
                    result = dt.Value;
                    return true;
                }
            }

            result = default;
            return false;
        }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains the directive with an integer value; otherise, false.</returns>
        public bool TryGetDirectiveValue(string key, out int result)
        {
            if (directives.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value) && int.TryParse(value, out result))
                return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains the directive with an integer value; otherise, false.</returns>
        public bool TryGetDirectiveValue(string key, out long result)
        {
            if (directives.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value) && long.TryParse(value, out result))
                return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Gets the value of a specific directive.
        /// </summary>
        /// <param name="key">The directive key.</param>
        /// <param name="result">The result output.</param>
        /// <returns>true if contains the directive with an integer value; otherise, false.</returns>
        public bool TryGetDirectiveValue(string key, out double result)
        {
            if (directives.TryGetValue(key, out var value) && !string.IsNullOrEmpty(value) && double.TryParse(value, out result))
                return true;
            result = default;
            return false;
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        protected override List<string> ParseLine(string line)
        {
           return ParseLineStatic(line, fieldSeperator, HandleDirective);
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
        private static string ToLineString(IReadOnlyList<string> line, char seperator)
        {
            if (line == null || line.Count == 0) return null;
            var str = new StringBuilder();
            foreach (var field in line)
            {
                if (field.IndexOfAny(new[] { seperator, '\"' }) >= 0)
                {
                    str.Append('\"');
                    str.Append(field.Replace("\"", "\"\""));
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
        /// <param name="directiveHandler">Additional handler for directive.</param>
        /// <returns>Values in this line.</returns>
        private static List<string> ParseLineStatic(string line, char seperator, Action<string, string> directiveHandler = null)
        {
            if (string.IsNullOrEmpty(line)) return null;
            if (line.StartsWith("#"))
            {
                var pos = line.IndexOf(':');
                if (directiveHandler == null || pos < 1 || line.Length < 5 || line[1] == ' ' || line[1] == '#') return null;
                var dir = line.SubRangeString(1, pos).Trim();
                if (dir.Contains(" ")) return null;
                directiveHandler(dir, line.Substring(pos + 1).Trim());
                return null;
            }

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
                        if (StringExtensions.IsLast(item, '"', '"', true))
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

                if (StringExtensions.IsLast(item, '"', '"', false))
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

        private void HandleDirective(string key, string value)
        {
            switch (key)
            {
                case "Fields":
                    if (Headers == null)
                    {
                        var arr = value.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        if (arr.Length > 0) Headers = arr.ToList().AsReadOnly();
                    }

                    break;
                default:
                    directives[key] = value;
                    break;
            }
        }
    }
}
