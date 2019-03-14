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
                return ReadLines(reader);
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
                return ReadLines(reader);
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
                return ReadLines(reader);
            }
        }
    }
}
