using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.IO
{
    /// <summary>
    /// The characters reader.
    /// </summary>
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
            enumerator = StreamUtility.ReadChars(stream, encoding).GetEnumerator();
        }

        /// <summary>
        /// Initializes a new instance of the CharsReader class.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
        public CharsReader(IEnumerable<Stream> streams, Encoding encoding = null, bool closeStream = false)
        {
            enumerator = StreamUtility.ReadChars(streams, encoding, closeStream).GetEnumerator();
        }

        /// <summary>
        /// Initializes a new instance of the CharsReader class.
        /// </summary>
        /// <param name="streams">The stream collection to read.</param>
        /// <param name="encoding">The encoding to read text.</param>
        /// <param name="closeStream">true if need close stream automatically after read; otherwise, false.</param>
        public CharsReader(StreamPagingResolver streams, Encoding encoding = null, bool closeStream = false)
        {
            enumerator = StreamUtility.ReadChars(streams, encoding, closeStream).GetEnumerator();
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
            if (buffer == null) throw new ArgumentNullException(nameof(buffer), "buffer should not be null");
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
        {
            return Task.Run(() =>
            {
                return Read(buffer, index, count);
            });
        }

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
        {
            return Read(buffer, index, count);
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
        {
            return Task.Run(() =>
            {
                return ReadBlock(buffer, index, count);
            });
        }

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
        {
            return Task.Run(() =>
            {
                return ReadLineAsync();
            });
        }

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
        {
            return Task.Run(() =>
            {
                return ReadLineAsync();
            });
        }

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
    }
}
