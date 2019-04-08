using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.IO
{
    /// <summary>
    /// Stream extension.
    /// </summary>
    public static class StreamUtility
    {
        /// <summary>
        /// Gets a stream in given page.
        /// </summary>
        /// <param name="pageNo">The page number to turn to.</param>
        /// <returns>The stream in given page; or null, if no stream in the page.</returns>
        public delegate Stream StreamPagingResolver(int pageNo);

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
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            return CopyToAsync(source, destination, DefaultBufferSize, progress, cancellationToken);
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to another stream, using a specified buffer size.
        /// </summary>
        /// <param name="source">The source stream to copy.</param>
        /// <param name="destination">The destination stream to write.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
        /// <param name="progress">The progress to report. The value is the length of the stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        /// <exception cref="ArgumentException">The source stream should be readable; or the destination stream should be writable.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The buffer size is negative.</exception>
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (!source.CanRead) throw new ArgumentException("The source stream should be readable.", nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite) throw new ArgumentException("The destination stream should be writable.", nameof(destination));
            if (bufferSize < 0) throw new ArgumentOutOfRangeException("The buffer size should not be negative", nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        /// <summary>
        /// Separates a stream to several parts.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <param name="size">The size for each part.</param>
        /// <returns>All parts of the stream separated.</returns>
        public static IEnumerable<MemoryStream> Separate(this Stream source, int size)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (!source.CanRead) throw new ArgumentException("The source stream should be readable.", nameof(source));
            var destination = new MemoryStream();
            var bufferSize = DefaultBufferSize;
            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0)
            {
                destination.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                if (totalBytesRead < size) continue;
                destination = new MemoryStream();
                totalBytesRead = 0;
                yield return destination;
            }

            if (destination.Length > 0) yield return destination;
        }

        /// <summary>
        /// Reads lines from a specific stream reader.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream reader.</returns>
        public static IEnumerable<string> ReadLines(this StreamReader reader, bool removeEmptyLine = false)
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
        }

        /// <summary>
        /// Reads lines from a specific stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream reader.</returns>
        public static IEnumerable<string> ReadLines(Stream stream, Encoding encoding, bool removeEmptyLine = false)
        {
            using (var reader = new StreamReader(stream, encoding))
            {
                return ReadLines(reader, removeEmptyLine);
            }
        }

        /// <summary>
        /// Reads lines from a specific stream.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream reader.</returns>
        public static IEnumerable<string> ReadLines(FileInfo file, Encoding encoding, bool removeEmptyLine = false)
        {
            using (var reader = new StreamReader(file.FullName, encoding))
            {
                return ReadLines(reader, removeEmptyLine);
            }
        }

        /// <summary>
        /// Reads lines from a specific stream.
        /// </summary>
        /// <param name="file">The file to read.</param>
        /// <param name="detectEncodingFromByteOrderMarks">true if look for byte order marks at the beginning of the file; otherwise, false.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream reader.</returns>
        public static IEnumerable<string> ReadLines(FileInfo file, bool detectEncodingFromByteOrderMarks, bool removeEmptyLine = false)
        {
            using (var reader = new StreamReader(file.FullName, detectEncodingFromByteOrderMarks))
            {
                return ReadLines(reader, removeEmptyLine);
            }
        }

        /// <summary>
        /// Reads bytes from the stream and advances the position within the stream to the end.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>Bytes from the stream.</returns>
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
        /// <returns>Bytes from the stream collection.</returns>
        public static IEnumerable<byte> ReadBytes(IEnumerable<Stream> streams)
        {
            foreach (var stream in streams)
            {
                while (true)
                {
                    var b = stream.ReadByte();
                    if (b < 0) break;
                    yield return (byte)b;
                }
            }
        }

        /// <summary>
        /// Reads bytes from the streams and advances the position within each stream to the end.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <returns>Bytes from the stream collection.</returns>
        public static IEnumerable<byte> ReadBytes(StreamPagingResolver streams)
        {
            return ReadBytes(ToStreamCollection(streams));
        }

        /// <summary>
        /// Reads characters from the stream and advances the position within each stream to the end.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <returns>Bytes from the stream collection.</returns>
        public static IEnumerable<char> ReadChars(Stream stream, Encoding encoding = null)
        {
            var decoder = (encoding ?? Encoding.Default).GetDecoder();
            while (true)
            {
                var buffer = new byte[12];
                if (stream.Read(buffer, 0, buffer.Length) < buffer.Length) break;
                var len = decoder.GetCharCount(buffer, 0, buffer.Length);
                var chars = new char[len];
                decoder.GetChars(buffer, 0, buffer.Length, chars, 0);
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
        /// <returns>Bytes from the stream collection.</returns>
        public static IEnumerable<char> ReadChars(IEnumerable<Stream> streams, Encoding encoding = null)
        {
            var decoder = (encoding ?? Encoding.Default).GetDecoder();
            foreach (var stream in streams)
            {
                while (true)
                {
                    var buffer = new byte[12];
                    if (stream.Read(buffer, 0, buffer.Length) < buffer.Length) break;
                    var len = decoder.GetCharCount(buffer, 0, buffer.Length);
                    var chars = new char[len];
                    decoder.GetChars(buffer, 0, buffer.Length, chars, 0);
                    foreach (var c in chars)
                    {
                        yield return c;
                    }
                }
            }
        }

        /// <summary>
        /// Reads characters from the streams and advances the position within each stream to the end.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <returns>Bytes from the stream collection.</returns>
        public static IEnumerable<char> ReadChars(StreamPagingResolver streams, Encoding encoding = null)
        {
            return ReadChars(ToStreamCollection(streams), encoding);
        }

        private static IEnumerable<Stream> ToStreamCollection(StreamPagingResolver streams)
        {
            if (streams == null) yield break;
            for (var i = 0; ; i++)
            {
                var stream = streams(i);
                if (stream == null) break;
                yield return stream;
            }
        }
    }
}
