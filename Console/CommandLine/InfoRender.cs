using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security;

using Trivial.Text;

namespace Trivial.CommandLine;

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
    /// <param name="captionStyle">The style of header.</param>
    /// <param name="messageStyle">The style of details.</param>
    /// <param name="ex">The exception.</param>
    /// <param name="stackTrace">true if output stack trace; otherwise, false.</param>
    public static void WriteLine(this StyleConsole cli, ConsoleTextStyle captionStyle, ConsoleTextStyle messageStyle, Exception ex, bool stackTrace = false)
    {
        if (ex == null) return;
        cli ??= StyleConsole.Default;
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
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="ex">The error information.</param>
    public static void WriteLine(this StyleConsole cli, Data.ErrorMessageResult ex)
        => WriteLine(cli, new ConsoleTextStyle
        {
            ForegroundConsoleColor = ConsoleColor.Red
        }, null, ex);

    /// <summary>
    /// Writes an exception, followed by the current line terminator, to the standard output stream.
    /// It will flush immediately.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="captionStyle">The style of header.</param>
    /// <param name="messageStyle">The style of details.</param>
    /// <param name="ex">The error information.</param>
    public static void WriteLine(this StyleConsole cli, ConsoleTextStyle captionStyle, ConsoleTextStyle messageStyle, Data.ErrorMessageResult ex)
    {
        if (ex == null) return;
        cli ??= StyleConsole.Default;
        var header = new ConsoleText(Resource.Error, captionStyle);
        if (!string.IsNullOrWhiteSpace(ex.Message)) header.Content.Append(ex.Message);
        var message = new ConsoleText(Environment.NewLine, messageStyle);
        if (!string.IsNullOrWhiteSpace(ex.ErrorCode))
            message.Content.Append($" [ErrCode] {ex.ErrorCode}");
        if (ex.Details != null)
        {
            foreach (var line in ex.Details)
            {
                message.Content.AppendLine();
                message.Content.Append($"- {line}");
            }
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
    /// Adds an empty line.
    /// </summary>
    /// <param name="list">The console text collection.</param>
    public static void AddEmptyLine(this IList<ConsoleText> list)
        => list?.Add(new ConsoleText(Environment.NewLine));

    /// <summary>
    /// Writes a sentense to allow pressing a key to continue.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="style">The style.</param>
    public static void PressAnyKeyToContinue(StyleConsole cli, ConsoleTextStyle style = null)
    {
        cli ??= StyleConsole.Default;
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
    /// Removes rest content value.
    /// </summary>
    /// <param name="s">The console text instance to limit length.</param>
    /// <param name="length">The length at most.</param>
    public static void RemoveRest(this ConsoleText s, int length)
    {
        if (s == null) return;
        var sb = s.Content;
        if (sb.Length <= length / 2) return;
        var count = 0;
        var n = new StringBuilder();
        foreach (var c in sb.ToString())
        {
            var needStop = false;
            switch (c)
            {
                case '\t':
                    count += 4;
                    break;
                case '\r':
                case '\n':
                case '\0':
                    needStop = true;
                    break;
                case '\b':
                    break;
                default:
                    count += GetLetterWidth(c);
                    break;
            }

            if (needStop || count > length) break;
            n.Append(c);
        }

        sb.Clear();
#if NETFRAMEWORK
        sb.Append(n.ToString());
#else
        sb.Append(n);
#endif
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
            return (cli ?? StyleConsole.Default).CursorLeft;
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
        catch (System.Runtime.InteropServices.ExternalException)
        {
        }
        catch (ArgumentException)
        {
        }

        return null;
    }

    private static int GetBufferSafeWidth(StyleConsole cli)
    {
        try
        {
            return (cli ?? StyleConsole.Default).BufferWidth - 1;
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
        catch (System.Runtime.InteropServices.ExternalException)
        {
        }
        catch (ArgumentException)
        {
        }

        return 70;
    }
}
