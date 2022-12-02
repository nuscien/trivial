using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.IO;

/// <summary>
/// Gets a stream in given page.
/// </summary>
/// <param name="pageNo">The page number to turn to.</param>
/// <returns>The stream in given page; or null, if no stream in the page.</returns>
public delegate Stream StreamPagingResolver(int pageNo);

/// <summary>
/// Stream copy.
/// </summary>
public static class StreamCopy
{
    /// <summary>
    /// The default buffer size.
    /// </summary>
    public const int DefaultBufferSize = 81920; // 80K

    /// <summary>
    /// Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size.
    /// </summary>
    /// <param name="source">The source stream to copy.</param>
    /// <param name="destination">The destination stream to write.</param>
    /// <param name="progress">The progress to report. The value is the length of the stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument was null.</exception>
    /// <exception cref="NotSupportedException">The source stream was not readable; or the destination stream was not writable.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        => CopyToAsync(source, destination, DefaultBufferSize, progress, cancellationToken);

    /// <summary>
    /// Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size.
    /// </summary>
    /// <param name="source">The source stream to copy.</param>
    /// <param name="destination">The destination stream to write.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
    /// <param name="progress">The progress to report. The value is the length of the stream.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument was null.</exception>
    /// <exception cref="NotSupportedException">The source stream was not readable; or the destination stream was not writable.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The buffer size was negative.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
    {
        if (source == null) throw new ArgumentNullException(nameof(source), "source should not be null.");
        if (!source.CanRead) throw new NotSupportedException("The source stream should be readable.");
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        if (!destination.CanWrite) throw new NotSupportedException("The destination stream should be writable.");
        if (bufferSize < 0) throw new ArgumentOutOfRangeException(nameof(bufferSize), "The buffer size should not be negative");

        var buffer = new byte[bufferSize];
        long totalBytesRead = 0;
        int bytesRead;
#if NETFRAMEWORK
        while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
        {
            await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            progress?.Report(totalBytesRead);
        }
#else
        while ((bytesRead = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false)) != 0)
        {
            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
            totalBytesRead += bytesRead;
            progress?.Report(totalBytesRead);
        }
#endif
    }

    /// <summary>
    /// Separates a stream to several parts.
    /// </summary>
    /// <param name="source">The source stream.</param>
    /// <param name="size">The size for each part.</param>
    /// <returns>All parts of the stream separated.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<MemoryStream> Separate(this Stream source, int size = DefaultBufferSize)
    {
        if (source == null) yield break;
        if (!source.CanRead) throw new NotSupportedException("The source stream should be readable.");
        if (size < 1) throw new ArgumentOutOfRangeException(nameof(size), "size should not be less than 1.");
        var destination = new MemoryStream();
        var buffer = new byte[size];
        long totalBytesRead = 0;
        int bytesRead;
        while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
        {
            destination.Write(buffer, 0, bytesRead);
            totalBytesRead += bytesRead;
            if (totalBytesRead < size) continue;
            destination.Position = 0;
            yield return destination;
            destination = new MemoryStream();
            totalBytesRead = 0;
        }

        if (destination.Length < 1) yield break;
        destination.Position = 0;
        yield return destination;
    }

    /// <summary>
    /// Reads bytes from the stream and advances the position within the stream to the end.
    /// </summary>
    /// <param name="stream">The stream to read.</param>
    /// <returns>Bytes from the stream.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<byte> ReadBytes(Stream stream)
    {
        if (stream == null) yield break;
        while (true)
        {
            var b = stream.ReadByte();
            if (b < 0) break;
            yield return (byte)b;
        }
    }

    /// <summary>
    /// Reads bytes from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<byte> ReadBytes(IEnumerable<Stream> streams, bool closeStream = false)
        => ReadBytes(streams, null, closeStream);

    /// <summary>
    /// Reads bytes from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="onStreamReadToEnd">A callback occurred on read to end per stream.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<byte> ReadBytes(IEnumerable<Stream> streams, Action<Stream> onStreamReadToEnd, bool closeStream = false)
    {
        if (streams == null) yield break;
        foreach (var stream in streams)
        {
            if (stream == null) continue;
            try
            {
                while (true)
                {
                    var b = stream.ReadByte();
                    if (b < 0) break;
                    yield return (byte)b;
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
    /// Reads bytes from the streams and advances the position within each stream to the end.
    /// </summary>
    /// <param name="streams">The stream collection to read.</param>
    /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
    /// <returns>Bytes from the stream collection.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<byte> ReadBytes(StreamPagingResolver streams, bool closeStream = false)
        => ReadBytes(ToStreamCollection(streams), closeStream);

    /// <summary>
    /// Reads lines from a specific stream reader.
    /// </summary>
    /// <param name="reader">The stream reader.</param>
    /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
    /// <returns>Lines from the specific stream reader.</returns>
    /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream has disposed.</exception>
    public static IEnumerable<string> ReadLines(this TextReader reader, bool removeEmptyLine = false)
        => CharsReader.ReadLines(reader, removeEmptyLine);

    /// <summary>
    /// Converts a stream paging resolver to a stream collection.
    /// </summary>
    /// <param name="streams">A stream paging resolver.</param>
    /// <returns>A stream collection.</returns>
    public static IEnumerable<Stream> ToStreamCollection(StreamPagingResolver streams)
    {
        if (streams == null) yield break;
        for (var i = 0; ; i++)
        {
            Stream stream;
            try
            {
                stream = streams(i);
                if (stream == null) break;
            }
            catch (ArgumentException)
            {
                break;
            }
            
            yield return stream;
        }
    }

    internal static bool TrySeek(this Stream stream, SeekOrigin origin)
    {
        try
        {
            if (stream == null) return false;
            switch (origin)
            {
                case SeekOrigin.Current:
                    return true;
                case SeekOrigin.Begin:
                    if (stream.Position == 0) return true;
                    if (!stream.CanSeek) return false;
                    stream.Position = 0;
                    break;
                case SeekOrigin.End:
                    var end = stream.Length - 1;
                    if (stream.Position >= end) return true;
                    if (!stream.CanSeek) return false;
                    stream.Position = end;
                    break;
                default:
                    return false;
            }

            return true;
        }
        catch (IOException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
        catch (InvalidOperationException)
        {
        }

        return false;
    }
}
