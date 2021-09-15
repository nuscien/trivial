using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security;

using Trivial.Text;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The extensions for console renderer.
    /// </summary>
    public static partial class ConsoleRenderExtensions
    {
        /// <summary>
        /// Writes an exception, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="stackTrace">true if output stack trace; otherwise, false.</param>
        public static void WriteLine(this StyleConsole cli, Exception ex, bool stackTrace = false)
            => WriteLine(cli, new ConsoleTextStyle
            {
                ForegroundConsoleColor = ConsoleColor.Red
            }, null, ex, stackTrace);

        /// <summary>
        /// Writes an exception, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="ex">The exception.</param>
        /// <param name="captionStyle">The style of header.</param>
        /// <param name="messageStyle">The style of details.</param>
        /// <param name="stackTrace">true if output stack trace; otherwise, false.</param>
        public static void WriteLine(this StyleConsole cli, ConsoleTextStyle captionStyle, ConsoleTextStyle messageStyle, Exception ex, bool stackTrace = false)
        {
            if (ex == null) return;
            if (cli == null) cli = StyleConsole.Default;
            var header = new ConsoleText(Resource.Error, captionStyle);
            if (!string.IsNullOrWhiteSpace(ex.Message)) header.Content.Append(ex.Message);
            var message = new ConsoleText(Environment.NewLine, messageStyle);
            if (!string.IsNullOrWhiteSpace(ex.HelpLink))
                message.Content.AppendLine(ex.HelpLink);
            message.Content.Append(ex.GetType().FullName);
            if (ex.InnerException != null)
            {
                message.Content.Append($" > {ex.InnerException.GetType().FullName}");
                if (ex.InnerException is AggregateException aggEx && aggEx.InnerExceptions != null)
                {
                    foreach (var iEx in aggEx.InnerExceptions)
                    {
                        message.Content.AppendLine();
                        message.Content.Append($"- {iEx.GetType().FullName}\t{iEx.Message}");
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(ex.InnerException.Message))
                    {
                        message.Content.AppendLine();
                        message.Content.Append(ex.InnerException.Message);
                    }
                }
            }

            if (stackTrace && !string.IsNullOrWhiteSpace(ex.StackTrace))
            {
                message.Content.AppendLine();
                message.Content.AppendLine("Stack trace");
                message.Content.Append(ex.StackTrace);
            }

            cli.WriteLine(header, message);
        }

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, IJsonDataNode json)
            => (cli ?? StyleConsole.Default).WriteLine(new JsonConsoleStyle().CreateTextCollection(json, 0));

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="style">The style.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, JsonConsoleStyle style, IJsonDataNode json)
            => (cli ?? StyleConsole.Default).WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json, 0));

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, System.Text.Json.Nodes.JsonObject json)
            => (cli ?? StyleConsole.Default).WriteLine(new JsonConsoleStyle().CreateTextCollection(json == null ? null : (JsonObjectNode)json, 0));

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="style">The style.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, JsonConsoleStyle style, System.Text.Json.Nodes.JsonObject json)
            => (cli ?? StyleConsole.Default).WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json == null ? null : (JsonObjectNode)json, 0));

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, System.Text.Json.Nodes.JsonArray json)
            => (cli ?? StyleConsole.Default).WriteLine(new JsonConsoleStyle().CreateTextCollection(json == null ? null : (JsonArrayNode)json, 0));

        /// <summary>
        /// Writes a JSON object, followed by the current line terminator, to the standard output stream.
        /// It will flush immediately.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="style">The style.</param>
        /// <param name="json">The JSON instance.</param>
        public static void WriteLine(this StyleConsole cli, JsonConsoleStyle style, System.Text.Json.Nodes.JsonArray json)
            => (cli ?? StyleConsole.Default).WriteLine((style ?? new JsonConsoleStyle()).CreateTextCollection(json == null ? null : (JsonArrayNode)json, 0));

        /// <summary>
        /// Writes a sentense to allow pressing a key to continue.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="style">The style.</param>
        public static void PressAnyKeyToContinue(StyleConsole cli, ConsoleTextStyle style = null)
        {
            if (cli == null) cli = StyleConsole.Default;
            cli.WriteLine(style, Resource.PressAnyKeyToCont);
            try
            {
                cli.ReadKey(true);
            }
            catch (InvalidOperationException)
            {
            }
            catch (IOException)
            {
            }
            catch (NotSupportedException)
            {
            }

            try
            {
                cli.ReadLine();
            }
            catch (InvalidOperationException)
            {
            }
            catch (IOException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (ArgumentException)
            {
            }

            return;
        }

        /// <summary>
        /// Tries to gets the row position of the cursor within the buffer area.
        /// </summary>
        /// <param name="cli">The command line interface.</param>
        /// <returns>The row position of the cursor within the buffer area; or null if failed.</returns>
        public static int? TryGetCursorTop(this StyleConsole cli)
        {
            try
            {
                return cli.CursorTop;
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (SecurityException)
            {
            }

            return null;
        }

        /// <summary>
        /// Tries to gets the column position of the cursor within the buffer area; or null if failed.
        /// </summary>
        /// <param name="cli">The command line interface.</param>
        /// <returns>The column position of the cursor within the buffer area; or null if failed.</returns>
        public static int? TryGetCursorLeft(this StyleConsole cli)
        {
            try
            {
                return cli.CursorLeft;
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (SecurityException)
            {
            }

            return null;
        }

        private static int GetBufferWidth()
        {
            try
            {
                return Console.BufferWidth - 1;
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (SecurityException)
            {
            }

            return 70;
        }
    }
}
