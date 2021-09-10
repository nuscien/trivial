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
    public sealed partial class ConsoleInterface
    {
        /// <summary>
        /// The terminal mode.
        /// </summary>
        public enum Modes
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
        /// The terminal origin.
        /// </summary>
        public enum Origins
        {
            /// <summary>
            /// Current cursof position.
            /// </summary>
            Current = 0,

            /// <summary>
            /// View port (at the top-left corner).
            /// </summary>
            ViewPort = 1,

            /// <summary>
            /// Buffer (at the top-left corner).
            /// </summary>
            Buffer = 2
        }

        /// <summary>
        /// The areas related current cusor to remove.
        /// </summary>
        public enum RelativeAreas
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
            /// <param name="origin">The relative origin.</param>
            /// <param name="x">The horizontal translation size.</param>
            /// <param name="y">The vertical translation size.</param>
            /// <param name="context">The context.</param>
            void MoveCursor(Origins origin, int x, int y, object context);

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
        /// The event arguments with relative position.
        /// </summary>
        public class RelativePositionEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the RelativePositionEventArgs class.
            /// </summary>
            /// <param name="origin">The origin.</param>
            /// <param name="x">The horizontal distance..</param>
            /// <param name="y">The vertical distance.</param>
            public RelativePositionEventArgs(Origins origin, int x, int y)
            {
                Origin = origin;
                X = x;
                Y = y;
            }

            /// <summary>
            /// Gets the origin.
            /// </summary>
            public Origins Origin { get; }

            /// <summary>
            /// Gets the horizontal distance.
            /// </summary>
            public int X { get; }

            /// <summary>
            /// Gets the vertical distance.
            /// </summary>
            public int Y { get; }
        }

        /// <summary>
        /// Gets the singleton of console text content.
        /// </summary>
        public static ConsoleInterface Default { get; } = new();
    }
}
