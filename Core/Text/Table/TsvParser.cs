using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Trivial.Text;

/// <summary>
/// The text parser for tab-separated values file format.
/// </summary>
[Guid("E90FE0E3-799C-4E0C-94DC-8D1925EA7F0E")]
public class TsvParser : BaseLinesStringTableParser
{
    /// <summary>
    /// TSV MIME.
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
    public TsvParser(Stream stream, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(stream, detectEncodingFromByteOrderMarks, encoding ?? Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TsvParser class.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="encoding">The optional character encoding to use.</param>
    public TsvParser(Stream stream, Encoding encoding = null) : base(stream, encoding ?? Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TsvParser class.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
    /// <param name="encoding">The optional character encoding to use.</param>
    public TsvParser(FileInfo file, bool detectEncodingFromByteOrderMarks, Encoding encoding = null) : base(file, detectEncodingFromByteOrderMarks, encoding ?? Encoding.UTF8)
    {
    }

    /// <summary>
    /// Initializes a new instance of the TsvParser class.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="encoding">The optional character encoding to use.</param>
    public TsvParser(FileInfo file, Encoding encoding = null) : base(file, encoding ?? Encoding.UTF8)
    {
    }

    /// <summary>
    /// Parses a line in TSV file.
    /// </summary>
    /// <param name="line">A line in TSV file.</param>
    /// <returns>Values in this line.</returns>
    protected override List<string> ParseLine(string line)
        => CsvParser.ParseLineStatic(line, fieldSeperator);

    /// <summary>
    /// Converts a line information to a string.
    /// </summary>
    /// <param name="line">The fields.</param>
    /// <returns>A line string.</returns>
    protected override string ToLineString(IReadOnlyList<string> line)
        => CsvParser.ToLineString(line, fieldSeperator);

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV text.</param>
    /// <returns>Content of TSV.</returns>
    public static IEnumerable<IReadOnlyList<string>> Parse(IEnumerable<string> tsv)
    {
        foreach (var line in tsv)
        {
            var item = CsvParser.ParseLineStatic(line, fieldSeperator);
            if (item == null) continue;
            yield return item.AsReadOnly();
        }
    }

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV text.</param>
    /// <returns>Content of TSV.</returns>
    public static IEnumerable<IReadOnlyList<string>> Parse(string tsv)
        => Parse(StringExtensions.YieldSplit(tsv, '\r', '\n', StringSplitOptions.RemoveEmptyEntries));

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The stream contains TSV.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The stream was null.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<IReadOnlyList<string>> Parse(Stream tsv, Encoding encoding = null)
    {
        var reader = new StreamReader(tsv, encoding ?? Encoding.UTF8);
        return Parse(reader, true);
    }

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV file.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The file was null.</exception>
    public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo tsv, Encoding encoding = null)
    {
        var reader = new StreamReader(tsv.FullName, encoding ?? Encoding.UTF8);
        return Parse(reader, true);
    }

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV file.</param>
    /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The file was null.</exception>
    public static IEnumerable<IReadOnlyList<string>> Parse(FileInfo tsv, bool detectEncodingFromByteOrderMarks)
    {
        var reader = new StreamReader(tsv.FullName, detectEncodingFromByteOrderMarks);
        return Parse(reader, true);
    }

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV text reader.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The reader was null.</exception>
    /// <exception cref="ObjectDisposedException">The reader has disposed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than max value of integer 32-bit value type.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    public static IEnumerable<IReadOnlyList<string>> Parse(TextReader tsv)
        => Parse(tsv, null);

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV text reader.</param>
    /// <param name="onComplete">The callback occurred on complete.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The reader was null.</exception>
    /// <exception cref="ObjectDisposedException">The reader has disposed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than max value of integer 32-bit value type.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    public static IEnumerable<IReadOnlyList<string>> Parse(TextReader tsv, Action onComplete)
    {
        if (tsv == null) yield break;
        try
        {
            while (true)
            {
                var line = tsv.ReadLine();
                if (line == null) break;
                var item = CsvParser.ParseLineStatic(line, fieldSeperator);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// Parses TSV.
    /// </summary>
    /// <param name="tsv">The TSV text reader.</param>
    /// <param name="disposeOnComplete">true if need dispose on complete; otherwise, false.</param>
    /// <returns>Content of TSV.</returns>
    /// <exception cref="ArgumentNullException">The reader was null.</exception>
    /// <exception cref="ObjectDisposedException">The reader has disposed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than max value of integer 32-bit value type.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    internal static IEnumerable<IReadOnlyList<string>> Parse(TextReader tsv, bool disposeOnComplete)
    {
        if (tsv == null) yield break;
        try
        {
            while (true)
            {
                var line = tsv.ReadLine();
                if (line == null) break;
                var item = CsvParser.ParseLineStatic(line, fieldSeperator);
                if (item == null) continue;
                yield return item.AsReadOnly();
            }
        }
        finally
        {
            if (disposeOnComplete) tsv.Dispose();
        }
    }
}
