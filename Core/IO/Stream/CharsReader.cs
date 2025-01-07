using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Reflection;

namespace Trivial.IO;

/// <summary>
/// The characters reader.
/// </summary>
[Guid("B8BB09CE-3ADC-4E56-AF99-D7DF82FAA419")]
public class CharsReader : TextReader
{
    private readonly IEnumerator<char> enumerator;
    private bool hasRead;

    /// <summary>
    /// Initializes a new instance of the CharsReader class.
    /// </summary>
    /// <param name="chars">The characters.</param>
    public CharsReader(IEnumerable<char> chars)
    {
        enumerator = (chars ?? new List<char>()).GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the CharsReader class.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    public CharsReader(Stream stream, Encoding encoding = null)
    {
        enumerator = ReadChars(stream, encoding).GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the CharsReader class.
    /// </summary>
    /// <param name="bytes">The byte collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    public CharsReader(IEnumerable<byte> bytes, Encoding encoding = null)
    {
        enumerator = ReadChars(bytes, encoding).GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the CharsReader class.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    public CharsReader(IEnumerable<Stream> streams, Encoding encoding = null, bool closeStream = false)
    {
        enumerator = ReadChars(streams, encoding, closeStream).GetEnumerator();
    }

    /// <summary>
    /// Initializes a new instance of the CharsReader class.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    public CharsReader(StreamPagingResolver streams, Encoding encoding = null, bool closeStream = false)
    {
        enumerator = ReadChars(streams, encoding, closeStream).GetEnumerator();
    }

    /// <summary>
    /// Returns the next available character but does not consume it.
    /// </summary>
    /// <returns>An integer representing the next character to be read, or -1 if no more characters are available or the stream does not support seeking.</returns>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    public override int Peek()
    {
        if (hasRead) return enumerator.Current;
        if (!enumerator.MoveNext()) return -1;
        hasRead = true;
        return enumerator.Current;
    }

    /// <summary>
    /// Reads the next character from the input string and advances the character position by one character.
    /// </summary>
    /// <returns>The next character from the underlying string, or -1 if no more characters are available.</returns>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    public override int Read()
    {
        if (hasRead)
        {
            hasRead = false;
            return enumerator.Current;
        }

        if (!enumerator.MoveNext()) return -1;
        return enumerator.Current;
    }

    /// <summary>
    /// Reads a block of characters from the input string and advances the character position by count.
    /// </summary>
    /// <param name="buffer">
    /// When this method returns,
    /// contains the specified character array with the values between index and (index + count - 1)
    /// replaced by the characters read from the current source.
    /// </param>
    /// <param name="index">The starting index in the buffer.</param>
    /// <param name="count">The number of characters to read.</param>
    /// <returns>
    /// The total number of characters read into the buffer.
    /// This can be less than the number of characters requested if that many characters are not currently available,
    /// or zero if the end of the underlying string has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">buffer was null.</exception>
    /// <exception cref="ArgumentException">The buffer length minus index was less than count.</exception>
    /// <exception cref="ArgumentOutOfRangeException">index or count was negative.</exception>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    public override int Read(char[] buffer, int index, int count)
    {
        if (buffer == null) throw ObjectConvert.ArgumentNull(nameof(buffer));
        if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "index should not be negative.");
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count should not be negative.");
        if (count == 0) return 0;
        if ((buffer.Length - index) < count) throw new ArgumentException("count plus index should not be greater than buffer length.");

        var max = count + index;
        var i = index;
        if (hasRead)
        {
            hasRead = false;
            i++;
            buffer[i] = enumerator.Current;
        }

        for (; i < max; i++)
        {
            if (!enumerator.MoveNext()) break;
            buffer[i] = enumerator.Current;
        }

        return i - index;
    }

    /// <summary>
    /// Reads a specified maximum number of characters from the current string asynchronously
    /// and writes the data to a buffer, beginning at the specified index.
    /// </summary>
    /// <param name="buffer">
    /// When this method returns, contains the specified character array with the values
    /// between index and (index + count - 1) replaced by the characters read from the current source.
    /// </param>
    /// <param name="index">The position in buffer at which to begin writing.</param>
    /// <param name="count">
    /// The maximum number of characters to read.
    /// If the end of the string is reached before the specified number of characters is written into the buffer,
    /// the method returns.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation.
    /// The value of the TResult parameter contains the total number of bytes read into the buffer.
    /// The result value can be less than the number of bytes requested
    /// if the number of bytes currently available is less than the requested number,
    /// or it can be 0 (zero) if the end of the string has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">buffer was null.</exception>
    /// <exception cref="ArgumentException">The buffer length minus index was less than count.</exception>
    /// <exception cref="ArgumentOutOfRangeException">index or count was negative.</exception>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
    public override Task<int> ReadAsync(char[] buffer, int index, int count)
        => Task.Run(() =>
        {
            return Read(buffer, index, count);
        });

    /// <summary>
    /// Reads a specified maximum number of characters from the current text reader
    /// and writes the data to a buffer, beginning at the specified index.
    /// </summary>
    /// <param name="buffer">
    /// When this method returns, contains the specified character array with the values
    /// between index and (index + count - 1) replaced by the characters read from the current source.
    /// </param>
    /// <param name="index">The position in buffer at which to begin writing.</param>
    /// <param name="count">
    /// The maximum number of characters to read. If the end of the string is reached
    /// before the specified number of characters is written into the buffer, the method returns.
    /// </param>
    /// <returns>
    /// The number of characters that have been read.
    /// The number will be less than or equal to count,
    /// depending on whether all input characters have been read.
    /// </returns>
    public override int ReadBlock(char[] buffer, int index, int count)
        => Read(buffer, index, count);

    /// <summary>
    /// Reads a specified maximum number of characters from the current string asynchronously
    /// and writes the data to a buffer, beginning at the specified index.
    /// </summary>
    /// <param name="buffer">
    /// When this method returns, contains the specified character array with the values
    /// between index and (index + count - 1) replaced by the characters read from the current source.
    /// </param>
    /// <param name="index">The position in buffer at which to begin writing.</param>
    /// <param name="count">
    /// The maximum number of characters to read. If the end of the string is reached
    /// before the specified number of characters is written into the buffer, the method returns.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous read operation.
    /// The value of the TResult parameter contains the total number of bytes read into the buffer.
    /// The result value can be less than the number of bytes requested
    /// if the number of bytes currently available is less than the requested number,
    /// or it can be 0 (zero) if the end of the string has been reached.
    /// </returns>
    /// <exception cref="ArgumentNullException">buffer was null.</exception>
    /// <exception cref="ArgumentException">The buffer length minus index was less than count.</exception>
    /// <exception cref="ArgumentOutOfRangeException">index or count was negative.</exception>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
    public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        => Task.Run(() =>
        {
            return ReadBlock(buffer, index, count);
        });

    /// <summary>
    /// Reads a line of characters from the current string and returns the data as a string.
    /// </summary>
    /// <returns>The next line from the current string, or null if the end of the string is reached.</returns>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    public override string ReadLine()
    {
        var sb = new StringBuilder();
        if (hasRead)
        {
            hasRead = false;
            if (enumerator.Current == '\n' || enumerator.Current == '\r') return string.Empty;
            sb.Append(enumerator.Current);
        }

        while (enumerator.MoveNext())
        {
            var c = enumerator.Current;
            if (c == '\n') return sb.ToString();
            if (c == '\r')
            {
                if (enumerator.MoveNext() && enumerator.Current != '\n') hasRead = true;
                return sb.ToString();
            }

            sb.Append(enumerator.Current);
        }

        return sb.Length > 0 ? sb.ToString() : null;
    }

    /// <summary>
    /// Reads a line of characters asynchronously from the current string and returns the data as a string.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous read operation.
    /// The value of the TResult parameter contains the next line from the reader,
    /// or is null if all the characters have been read.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than System.Int32.MaxValue.</exception>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    public override Task<string> ReadLineAsync()
        => Task.Run(() =>
        {
            return ReadLineAsync();
        });

    /// <summary>
    /// Reads all characters from the current position to the end of the string and returns them as a single string.
    /// </summary>
    /// <returns>The content from the current position to the end of the underlying string.</returns>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    public override string ReadToEnd()
    {
        var sb = new StringBuilder();
        if (hasRead)
        {
            hasRead = false;
            sb.Append(enumerator.Current);
        }

        while (enumerator.MoveNext())
        {
            sb.Append(enumerator.Current);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Reads all characters from the current position to the end of the string asynchronously and returns them as a single string.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous read operation.
    /// The value of the TResult parameter contains a string with the characters from the current position to the end of the string.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line is larger than System.Int32.MaxValue.</exception>
    /// <exception cref="ObjectDisposedException">The current reader is closed.</exception>
    /// <exception cref="InvalidOperationException">The reader is currently in use by a previous read operation.</exception>
    public override Task<string> ReadToEndAsync()
        => Task.Run(() =>
        {
            return ReadLineAsync();
        });

    /// <summary>
    /// Resets the position read.
    /// </summary>
    public void ResetPosition()
    {
        hasRead = false;
        enumerator.Reset();
    }

    /// <summary>
    /// Releases the unmanaged resources used by this instance and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (!disposing) return;
        hasRead = false;
        enumerator.Dispose();
    }

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(TextReader reader, bool removeEmptyLine = false)
        => ReadLines(reader, null, removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="onReadToEnd">The handler occurred on reading to end.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(TextReader reader, Action<bool> onReadToEnd, bool removeEmptyLine = false)
    {
        if (reader == null) yield break;
        var isSucc = false;
        try
        {
            if (removeEmptyLine)
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    if (string.IsNullOrEmpty(line)) continue;
                    yield return line;
                }
            }
            else
            {
                while (true)
                {
                    var line = reader.ReadLine();
                    if (line == null) break;
                    yield return line;
                }
            }

            isSucc = true;
        }
        finally
        {
            onReadToEnd?.Invoke(isSucc);
        }
    }

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream.</returns>
    /// <exception cref="ArgumentNullException">stream was null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(Stream stream, Encoding encoding, bool removeEmptyLine = false)
    {
        if (stream == null) throw ObjectConvert.ArgumentNull(nameof(stream));
        var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        var hasDisposed = false;
        return ReadLines(reader, isSucc =>
        {
            if (hasDisposed) return;
            hasDisposed = true;
            reader.Dispose();
        }, removeEmptyLine);
    }

    /// <summary>
    /// Reads lines from a specific byte collection.
    /// </summary>
    /// <param name="bytes">The byte collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream.</returns>
    /// <exception cref="ArgumentNullException">stream was null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(IEnumerable<byte> bytes, Encoding encoding, bool removeEmptyLine = false)
        => ReadLines(ReadChars(bytes, encoding), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream collection.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Lines from the specific stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(IEnumerable<Stream> streams, Encoding encoding, bool removeEmptyLine = false, bool closeStream = false)
        => ReadLines(ReadChars(streams, encoding, closeStream), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream collection.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Lines from the specific stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(StreamPagingResolver streams, Encoding encoding, bool removeEmptyLine = false, bool closeStream = false)
        => ReadLines(ReadChars(streams, encoding, closeStream), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific charactor collection.
    /// </summary>
    /// <param name="chars">The charactors to read.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific charactor collection.</returns>
    public static IEnumerable<string> ReadLines(IEnumerable<char> chars, bool removeEmptyLine = false)
    {
        if (chars == null) yield break;
        var wasR = false;
        var str = new StringBuilder();
        foreach (var c in chars)
        {
            if (wasR)
            {
                wasR = false;
                if (c == '\n') continue;
            }

            if (c == '\r') wasR = true;
            if (!wasR && c != '\n')
            {
                str.Append(c);
                continue;
            }

            if (removeEmptyLine && str.Length == 0) continue;
            yield return str.ToString();
            str.Clear();
        }

        if (removeEmptyLine && str.Length == 0) yield break;
        yield return str.ToString();
    }

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    public static IEnumerable<string> ReadLines(FileInfo file, Encoding encoding, bool removeEmptyLine = false)
    {
        if (file == null) throw ObjectConvert.ArgumentNull(nameof(file));
        var reader = new StreamReader(file.FullName, encoding);
        var hasDisposed = false;
        return ReadLines(reader, isSucc =>
        {
            if (hasDisposed) return;
            hasDisposed = true;
            reader.Dispose();
        }, removeEmptyLine);
    }

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="file">The file to read.</param>
    /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    public static IEnumerable<string> ReadLines(FileInfo file, bool detectEncodingFromByteOrderMarks, bool removeEmptyLine = false)
    {
        if (file == null) throw ObjectConvert.ArgumentNull(nameof(file));
        var reader = new StreamReader(file.FullName, detectEncodingFromByteOrderMarks);
        var hasDisposed = false;
        return ReadLines(reader, isSucc =>
        {
            if (hasDisposed) return;
            hasDisposed = true;
            reader.Dispose();
        }, removeEmptyLine);
    }

#if !NETFRAMEWORK
    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static IEnumerable<string> ReadLines(System.IO.Compression.ZipArchive zip, string entryName, Encoding encoding, bool removeEmptyLine = false)
    {
        var entry = (zip?.GetEntry(entryName)) ?? throw new FileNotFoundException("file is not found.");
        using var stream = entry.Open();
        return ReadLines(stream, encoding, removeEmptyLine);
    }

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="zip">The zip file.</param>
    /// <param name="entryName">A path, relative to the root of the archive, that identifies the entry to retrieve.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="ArgumentNullException">file was null.</exception>
    /// <exception cref="FileNotFoundException">file was not found.</exception>
    /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
    /// <exception cref="NotSupportedException">Cannot read the file.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The zip archive has been disposed.</exception>
    /// <exception cref="NotSupportedException">The zip archive does not support reading.</exception>
    /// <exception cref="InvalidDataException">The zip archive is corrupt, and the entry cannot be retrieved.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(System.IO.Compression.ZipArchive zip, string entryName, Encoding encoding, bool removeEmptyLine = false)
    {
        var entry = (zip?.GetEntry(entryName)) ?? throw new FileNotFoundException("file is not found.");
        using var stream = entry.Open();
        return ReadLinesAsync(stream, encoding, removeEmptyLine);
    }
#endif

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(TextReader reader, bool removeEmptyLine = false)
        => ReadLinesAsync(reader, null, removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="onReadToEnd">The handler occurred on reading to end.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static async IAsyncEnumerable<string> ReadLinesAsync(TextReader reader, Action<bool> onReadToEnd, bool removeEmptyLine = false)
    {
        if (reader == null) yield break;
        var isSucc = false;
        try
        {
            if (removeEmptyLine)
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;
                    if (string.IsNullOrEmpty(line)) continue;
                    yield return line;
                }
            }
            else
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;
                    yield return line;
                }
            }

            isSucc = true;
        }
        finally
        {
            onReadToEnd?.Invoke(isSucc);
        }
    }

    /// <summary>
    /// Reads lines from a specific stream.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream.</returns>
    /// <exception cref="ArgumentNullException">stream was null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(Stream stream, Encoding encoding, bool removeEmptyLine = false)
    {
        if (stream == null) throw ObjectConvert.ArgumentNull(nameof(stream));
        var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        var hasDisposed = false;
        return ReadLinesAsync(reader, isSucc =>
        {
            if (hasDisposed) return;
            hasDisposed = true;
            reader.Dispose();
        }, removeEmptyLine);
    }

    /// <summary>
    /// Reads lines from a specific byte collection.
    /// </summary>
    /// <param name="bytes">The byte collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream.</returns>
    /// <exception cref="ArgumentNullException">stream was null.</exception>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(IAsyncEnumerable<byte> bytes, Encoding encoding, bool removeEmptyLine = false)
        => ReadLinesAsync(ReadCharsAsync(bytes, encoding), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream collection.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Lines from the specific stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(IEnumerable<Stream> streams, Encoding encoding, bool removeEmptyLine = false, bool closeStream = false)
        => ReadLinesAsync(ReadCharsAsync(streams, encoding, closeStream), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific stream collection.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Lines from the specific stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(IAsyncEnumerable<Stream> streams, Encoding encoding, bool removeEmptyLine = false, bool closeStream = false)
        => ReadLinesAsync(ReadCharsAsync(streams, encoding, closeStream), removeEmptyLine);

    /// <summary>
    /// Reads lines from a specific charactor collection.
    /// </summary>
    /// <param name="chars">The charactors to read.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific charactor collection.</returns>
    public async static IAsyncEnumerable<string> ReadLinesAsync(IAsyncEnumerable<char> chars, bool removeEmptyLine = false)
    {
        if (chars == null) yield break;
        var wasR = false;
        var str = new StringBuilder();
        await foreach (var c in chars)
        {
            if (wasR)
            {
                wasR = false;
                if (c == '\n') continue;
            }

            if (c == '\r') wasR = true;
            if (!wasR && c != '\n')
            {
                str.Append(c);
                continue;
            }

            if (removeEmptyLine && str.Length == 0) continue;
            yield return str.ToString();
            str.Clear();
        }

        if (removeEmptyLine && str.Length == 0) yield break;
        yield return str.ToString();
    }

    /// <summary>
    /// Reads lines from a specific stream collection.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Lines from the specific stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<string> ReadLinesAsync(StreamPagingResolver streams, Encoding encoding, bool removeEmptyLine = false, bool closeStream = false)
        => ReadLinesAsync(ReadCharsAsync(streams, encoding, closeStream), removeEmptyLine);

    /// <summary>
    /// Reads characters from the stream and advances the position within each stream to the end.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <returns>Bytes from the stream.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<char> ReadChars(Stream stream, Encoding encoding = null)
    {
        if (stream == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var buffer = new byte[12];
        while (true)
        {
            var count = stream.Read(buffer, 0, buffer.Length);
            if (count == 0) break;
            var len = decoder.GetCharCount(buffer, 0, count);
            var chars = new char[len];
            decoder.GetChars(buffer, 0, count, chars, 0);
            foreach (var c in chars)
            {
                yield return c;
            }
        }
    }

    /// <summary>
    /// Reads characters from the bytes and advances the position within each stream to the end.
    /// </summary>
    /// <param name="bytes">The byte collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <returns>Bytes from the stream.</returns>
    public static IEnumerable<char> ReadChars(IEnumerable<byte> bytes, Encoding encoding = null)
    {
        if (bytes == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var pos = -1;
        var count = 12;
        var buffer = new byte[count];
        foreach (var b in bytes)
        {
            pos++;
            buffer[pos] = b;
            if (pos < 11) continue;
            var len = decoder.GetCharCount(buffer, 0, count);
            var chars = new char[len];
            decoder.GetChars(buffer, 0, count, chars, 0);
            pos = -1;
            foreach (var c in chars)
            {
                yield return c;
            }
        }

        if (pos > -1)
        {
            count = pos + 1;
            var len = decoder.GetCharCount(buffer, 0, count);
            var chars = new char[len];
            decoder.GetChars(buffer, 0, count, chars, 0);
            foreach (var c in chars)
            {
                yield return c;
            }
        }
    }

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<char> ReadChars(IEnumerable<Stream> streams, Encoding encoding = null, bool closeStream = false)
        => ReadChars(streams, null, encoding, closeStream);

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="onStreamReadToEnd">A callback occurred on read to end per stream.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<char> ReadChars(IEnumerable<Stream> streams, Action<Stream> onStreamReadToEnd, Encoding encoding = null, bool closeStream = false)
    {
        if (streams == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var buffer = new byte[12];
        foreach (var stream in streams)
        {
            if (stream == null) continue;
            try
            {
                while (true)
                {
                    var count = stream.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    var len = decoder.GetCharCount(buffer, 0, count);
                    var chars = new char[len];
                    decoder.GetChars(buffer, 0, count, chars, 0);
                    foreach (var c in chars)
                    {
                        yield return c;
                    }
                }
            }
            finally
            {
                onStreamReadToEnd?.Invoke(stream);
                if (closeStream) stream.Close();
            }
        }
    }

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<char> ReadChars(StreamPagingResolver streams, Encoding encoding = null, bool closeStream = false)
        => ReadChars(StreamCopy.ToStreamCollection(streams), encoding, closeStream);

    /// <summary>
    /// Reads characters from the bytes and advances the position within each stream to the end.
    /// </summary>
    /// <param name="bytes">The byte collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <returns>Bytes from the stream.</returns>
    public static async IAsyncEnumerable<char> ReadCharsAsync(IAsyncEnumerable<byte> bytes, Encoding encoding = null)
    {
        if (bytes == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var pos = -1;
        var count = 12;
        var buffer = new byte[count];
        await foreach (var b in bytes)
        {
            pos++;
            buffer[pos] = b;
            if (pos < 11) continue;
            var len = decoder.GetCharCount(buffer, 0, count);
            var chars = new char[len];
            decoder.GetChars(buffer, 0, count, chars, 0);
            pos = -1;
            foreach (var c in chars)
            {
                yield return c;
            }
        }

        if (pos > -1)
        {
            count = pos + 1;
            var len = decoder.GetCharCount(buffer, 0, count);
            var chars = new char[len];
            decoder.GetChars(buffer, 0, count, chars, 0);
            foreach (var c in chars)
            {
                yield return c;
            }
        }
    }

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<char> ReadCharsAsync(IEnumerable<Stream> streams, Encoding encoding = null, bool closeStream = false)
        => ReadCharsAsync(streams, null, encoding, closeStream);

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<char> ReadCharsAsync(IAsyncEnumerable<Stream> streams, Encoding encoding = null, bool closeStream = false)
        => ReadCharsAsync(streams, null, encoding, closeStream);

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="onStreamReadToEnd">A callback occurred on read to end per stream.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public async static IAsyncEnumerable<char> ReadCharsAsync(IEnumerable<Stream> streams, Action<Stream> onStreamReadToEnd, Encoding encoding = null, bool closeStream = false)
    {
        if (streams == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var buffer = new byte[12];
        foreach (var stream in streams)
        {
            if (stream == null) continue;
            try
            {
                while (true)
                {
                    var count = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    var len = decoder.GetCharCount(buffer, 0, count);
                    var chars = new char[len];
                    decoder.GetChars(buffer, 0, count, chars, 0);
                    foreach (var c in chars)
                    {
                        yield return c;
                    }
                }
            }
            finally
            {
                onStreamReadToEnd?.Invoke(stream);
                if (closeStream) stream.Close();
            }
        }
    }

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="onStreamReadToEnd">A callback occurred on read to end per stream.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public async static IAsyncEnumerable<char> ReadCharsAsync(IAsyncEnumerable<Stream> streams, Action<Stream> onStreamReadToEnd, Encoding encoding = null, bool closeStream = false)
    {
        if (streams == null) yield break;
        var decoder = (encoding ?? Encoding.Default ?? Encoding.UTF8).GetDecoder();
        var buffer = new byte[12];
        await foreach (var stream in streams)
        {
            if (stream == null) continue;
            try
            {
                while (true)
                {
                    var count = stream.Read(buffer, 0, buffer.Length);
                    if (count == 0) break;
                    var len = decoder.GetCharCount(buffer, 0, count);
                    var chars = new char[len];
                    decoder.GetChars(buffer, 0, count, chars, 0);
                    foreach (var c in chars)
                    {
                        yield return c;
                    }
                }
            }
            finally
            {
                onStreamReadToEnd?.Invoke(stream);
                if (closeStream) stream.Close();
            }
        }
    }

    /// <summary>
    /// Reads characters from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="encoding">The encoding to read text.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IAsyncEnumerable<char> ReadCharsAsync(StreamPagingResolver streams, Encoding encoding = null, bool closeStream = false)
        => ReadCharsAsync(StreamCopy.ToStreamCollection(streams), encoding, closeStream);

    /// <summary>
    /// Converts a string to a memory stream.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <param name="encoding">The encoding to write text.</param>
    /// <returns>A memeory stream with the specific string and encoding.</returns>
    public static Stream ToStream(string s, Encoding encoding = null)
    {
        if (s == null) return null;
        var bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
        return new MemoryStream(bytes);
    }
}
