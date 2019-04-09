using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.IO
{
    /// <summary>
    /// Gets a stream in given page.
    /// </summary>
    /// <param name="pageNo">The page number to turn to.</param>
    /// <returns>The stream in given page; or null, if no stream in the page.</returns>
    public delegate Stream StreamPagingResolver(int pageNo);

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
        /// <exception cref="ArgumentNullException">The argument was null.</exception>
        /// <exception cref="NotSupportedException">The source stream was not readable; or the destination stream was not writable.</exception>
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
        /// <exception cref="ArgumentNullException">The argument was null.</exception>
        /// <exception cref="NotSupportedException">The source stream was not readable; or the destination stream was not writable.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The buffer size was negative.</exception>
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            if (source == null) throw new ArgumentNullException(nameof(source), "source should not be null.");
            if (!source.CanRead) throw new NotSupportedException("The source stream should be readable.");
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite) throw new NotSupportedException("The destination stream should be writable.");
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
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static IEnumerable<MemoryStream> Separate(this Stream source, int size = DefaultBufferSize)
        {
            if (source == null) throw new ArgumentNullException(nameof(source), "source should not be null.");
            if (!source.CanRead) throw new ArgumentException("The source stream should be readable.", nameof(source));
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
        /// Reads lines from a specific stream reader.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream reader.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static IEnumerable<string> ReadLines(this StreamReader reader, bool removeEmptyLine = false)
        {
            if (reader == null) yield break;
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
        /// <returns>Lines from the specific stream.</returns>
        public static IEnumerable<string> ReadLines(Stream stream, Encoding encoding, bool removeEmptyLine = false)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream), "stream should not be null.");
            using (var reader = new StreamReader(stream, encoding))
            {
                return ReadLines(reader, removeEmptyLine);
            }
        }

        /// <summary>
        /// Reads lines from a specific stream collection.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream collection.</returns>
        public static IEnumerable<string> ReadLines(IEnumerable<Stream> streams, Encoding encoding, bool removeEmptyLine = false)
        {
            return ReadLines(ReadChars(streams, encoding), removeEmptyLine);
        }

        /// <summary>
        /// Reads lines from a specific stream collection.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="removeEmptyLine">true if need remove the empty line; otherwise, false.</param>
        /// <returns>Lines from the specific stream collection.</returns>
        public static IEnumerable<string> ReadLines(StreamPagingResolver streams, Encoding encoding, bool removeEmptyLine = false)
        {
            return ReadLines(ReadChars(streams, encoding), removeEmptyLine);
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
        public static IEnumerable<string> ReadLines(FileInfo file, Encoding encoding, bool removeEmptyLine = false)
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "file should not be null.");
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
        /// <exception cref="ArgumentNullException">file was null.</exception>
        /// <exception cref="FileNotFoundException">file was not found.</exception>
        /// <exception cref="DirectoryNotFoundException">The directory of the file was not found.</exception>
        /// <exception cref="NotSupportedException">Cannot read the file.</exception>
        public static IEnumerable<string> ReadLines(FileInfo file, bool detectEncodingFromByteOrderMarks, bool removeEmptyLine = false)
        {
            if (file == null) throw new ArgumentNullException(nameof(file), "file should not be null.");
            using (var reader = new StreamReader(file.FullName, detectEncodingFromByteOrderMarks))
            {
                return ReadLines(reader, removeEmptyLine);
            }
        }

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
        /// Reads bytes from the stream and advances the position within the stream to the end.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <returns>Bytes from the stream.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
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
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static IEnumerable<byte> ReadBytes(IEnumerable<Stream> streams)
        {
            if (streams == null) yield break;
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
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static IEnumerable<byte> ReadBytes(StreamPagingResolver streams)
        {
            return ReadBytes(ToStreamCollection(streams));
        }

        /// <summary>
        /// Reads characters from the stream and advances the position within each stream to the end.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <returns>Bytes from the stream.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static IEnumerable<char> ReadChars(Stream stream, Encoding encoding = null)
        {
            if (stream == null) yield break;
            var decoder = (encoding ?? Encoding.Default).GetDecoder();
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
        /// Reads characters from the streams and advances the position within each stream to the end.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <returns>Bytes from the stream collection.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
        public static IEnumerable<char> ReadChars(IEnumerable<Stream> streams, Encoding encoding = null)
        {
            if (streams == null) yield break;
            var decoder = (encoding ?? Encoding.Default).GetDecoder();
            var buffer = new byte[12];
            foreach (var stream in streams)
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
        }

        /// <summary>
        /// Reads characters from the streams and advances the position within each stream to the end.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <returns>Bytes from the stream collection.</returns>
        /// <exception cref="NotSupportedException">The stream does not support reading.</exception>
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
