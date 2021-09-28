using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Trivial.Data;
using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The command line interface.
    /// </summary>
    public sealed partial class StyleConsole
    {
        /// <summary>
        /// The terminal mode.
        /// </summary>
        public enum Modes : byte
        {
            /// <summary>
            /// ANSI escape sequences.
            /// </summary>
            Ansi = 1,

            /// <summary>
            /// Command prompt.
            /// </summary>
            Cmd = 2,

            /// <summary>
            /// Plain text.
            /// </summary>
            Text = 3
        }

        /// <summary>
        /// The areas related current cusor to remove.
        /// </summary>
        public enum RelativeAreas : byte
        {
            /// <summary>
            /// None.
            /// </summary>
            None = 0,

            /// <summary>
            /// Current line.
            /// </summary>
            Line = 1,

            /// <summary>
            /// To beginning of the current line.
            /// </summary>
            ToBeginningOfLine = 2,

            /// <summary>
            /// To end of the current line.
            /// </summary>
            ToEndOfLine = 3,

            /// <summary>
            /// To entire view port.
            /// </summary>
            EntireScreen = 4,

            /// <summary>
            /// To beginning of the view port.
            /// </summary>
            ToBeginningOfScreen = 5,

            /// <summary>
            /// To end of the view port.
            /// </summary>
            ToEndOfScreen = 6,

            /// <summary>
            /// To entire buffer.
            /// </summary>
            EntireBuffer = 7
        }

        /// <summary>
        /// The command line implementation.
        /// </summary>
        public interface IHandler
        {
            /// <summary>
            /// Gets the column position of the cursor within the buffer area.
            /// </summary>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
            int CursorLeft { get; }

            /// <summary>
            /// Gets the row position of the cursor within the buffer area.
            /// </summary>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
            int CursorTop { get; }

            /// <summary>
            /// Gets the height of the buffer area.
            /// </summary>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
            int BufferWidth { get; }

            /// <summary>
            /// Gets the height of the buffer area.
            /// </summary>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
            int BufferHeight { get; }

            /// <summary>
            /// Creates a context.
            /// </summary>
            /// <returns>A context.</returns>
            object CreateContext();

            /// <summary>
            /// Writes information to the standard output stream.
            /// </summary>
            /// <param name="data">The data to output.</param>
            /// <param name="context">The context.</param>
            void Write(IReadOnlyList<ConsoleText> data, object context);

            /// <summary>
            /// Moves cursor by a specific relative position.
            /// </summary>
            /// <param name="x">The horizontal translation size.</param>
            /// <param name="y">The vertical translation size.</param>
            /// <param name="context">The context.</param>
            void MoveCursorBy(int x, int y, object context);

            /// <summary>
            /// Moves cursor to a specific position in buffer.
            /// </summary>
            /// <param name="x">The zero-based X value to move.</param>
            /// <param name="y">The zero-based Y value to move; or -1 if do not move when origin is not related by current cursor.</param>
            /// <param name="context">The context.</param>
            void MoveCursorTo(int x, int y, object context);

            /// <summary>
            /// Removes the specific area.
            /// </summary>
            /// <param name="area">The area to remove.</param>
            /// <param name="context">The context.</param>
            void Remove(RelativeAreas area, object context);

            /// <summary>
            /// Reads the next line of characters from the standard input stream.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
            /// <exception cref="IOException">An I/O error occurred.</exception>
            /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
            /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line of characters is greater than max value of 32-bit integer.</exception>
            string ReadLine(object context);

            /// <summary>
            /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
            /// </summary>
            /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
            /// <param name="context">The context.</param>
            /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
            /// <exception cref="InvalidOperationException">The input stream is redirected from the one other than the console.</exception>
            ConsoleKeyInfo ReadKey(bool intercept, object context);
        }

        /// <summary>
        /// Gets the singleton of console text content.
        /// </summary>
        public static StyleConsole Default { get; } = new();
    }
}
