using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

using Trivial.Data;
using Trivial.Text;

namespace Trivial.CommandLine;

/// <summary>
/// The command line interface.
/// </summary>
public sealed partial class StyleConsole
{
    /// <summary>
    /// The additional context.
    /// </summary>
    private object context;

    /// <summary>
    /// The command line handler.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private IHandler handlerCache;

    /// <summary>
    /// Adds or removes the handler occurred after flushing.
    /// </summary>
    public event DataEventHandler<IReadOnlyList<ConsoleText>> Flushed;

    /// <summary>
    /// Adds or removes the handler occurred after cursor moving by.
    /// </summary>
    public event EventHandler<Maths.IntPoint2D.DataEventArgs> CursorMovedBy;

    /// <summary>
    /// Adds or removes the handler occurred after cursor moving To.
    /// </summary>
    public event EventHandler<Maths.IntPoint2D.DataEventArgs> CursorMovedTo;

    /// <summary>
    /// Adds or removes the handler occurred after output area clearing.
    /// </summary>
    public event DataEventHandler<RelativeAreas> Cleared;

    /// <summary>
    /// Gets or sets the terminal mode.
    /// </summary>
    public Modes Mode { get; set; }

    /// <summary>
    /// Gets or sets the handler to flush cache.
    /// </summary>
    public IHandler Handler
    {
        get
        {
            return handlerCache;
        }

        set
        {
            lock (locker)
            {
                if (handlerCache == value) return;
                handlerCache = value;
                if (handlerCache == null) return;
                context = handlerCache.CreateContext();
            }
        }
    }

    /// <summary>
    /// Gets the column position of the cursor within the buffer area.
    /// </summary>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
    /// <exception cref="SecurityException">The user does not have permission to perform this action.</exception>
    public int CursorLeft => handlerCache == null ? Console.CursorLeft : handlerCache.CursorLeft;

    /// <summary>
    /// Gets the row position of the cursor within the buffer area.
    /// </summary>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
    /// <exception cref="SecurityException">The user does not have permission to perform this action.</exception>
    public int CursorTop => handlerCache == null ? Console.CursorTop : handlerCache.CursorTop;

    /// <summary>
    /// Gets the height of the buffer area.
    /// </summary>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
    /// <exception cref="SecurityException">The user does not have permission to perform this action.</exception>
    public int BufferWidth => handlerCache == null ? Console.BufferWidth : handlerCache.BufferWidth;

    /// <summary>
    /// Gets the height of the buffer area.
    /// </summary>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="InvalidOperationException">The input stream does not provide such information.</exception>
    /// <exception cref="SecurityException">The user does not have permission to perform this action.</exception>
    public int BufferHeight => handlerCache == null ? Console.BufferHeight : handlerCache.BufferHeight;

    /// <summary>
    /// Enters a backspace to console to remove the last charactor.
    /// </summary>
    /// <param name="count">The count of the charactor to remove from end.</param>
    /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
    public void Backspace(int count = 1, bool doNotRemoveOutput = false)
    {
        if (count < 1)
        {
            if (count == -1 && !doNotRemoveOutput)
                Write(" \b ");
            return;
        }

        count = BackspaceOutputCache(count);
        if (doNotRemoveOutput)
        {
            BackspaceInternal(count, 2);
            return;
        }

        for (var i = 50; i <= count; i += 50)
        {
            BackspaceInternal(50, -1);
        }

        BackspaceInternal(count % 50, -1);
        Flush();
    }

    /// <summary>
    /// Enters backspaces to console to remove the charactors to the beginning of the line.
    /// </summary>
    public void BackspaceToBeginning()
    {
        if (Mode == Modes.Text && Handler == null)
        {
            col.Add(new ConsoleText("\b \b"));
            return;
        }

        var curLeft = 300;
        try
        {
            if (CursorLeft >= 0) curLeft = CursorLeft;
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        catch (NotSupportedException)
        {
        }

        Flush();
        if (Mode == Modes.Ansi || Handler != null)
        {
            Clear(RelativeAreas.ToBeginningOfLine);
            Backspace(curLeft, true);
        }
        else
        {
            Backspace(curLeft);
        }
    }

    /// <summary>
    /// Reads the next line of characters from the standard input stream.
    /// </summary>
    /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The number of characters in the next line of characters is greater than max value of 32-bit integer.</exception>
    public string ReadLine()
    {
        Flush();
        var h = Handler;
        return h == null ? Console.ReadLine() : h.ReadLine(context);
    }

    /// <summary>
    /// Obtains the next character or function key pressed by the user. The pressed key is optionally displayed in the console window.
    /// </summary>
    /// <param name="intercept">Determines whether to display the pressed key in the console window. true to not display the pressed key; otherwise, false.</param>
    /// <returns>The next line of characters from the input stream, or null if no more lines are available.</returns>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="InvalidOperationException">The input stream is redirected from the one other than the console.</exception>
    public ConsoleKeyInfo ReadKey(bool intercept = false)
    {
        var h = Handler;
        return h == null ? Console.ReadKey(intercept) : h.ReadKey(intercept, context);
    }

    /// <summary>
    /// Obtains the password pressed by the user.
    /// </summary>
    /// <returns>
    /// The password.
    /// </returns>
    public SecureString ReadPassword()
        => ReadPassword(null, null);

    /// <summary>
    /// Obtains the password pressed by the user.
    /// </summary>
    /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
    /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
    /// <returns>
    /// The password.
    /// </returns>
    public SecureString ReadPassword(char replaceChar, bool inline = false)
        => ReadPassword(null, replaceChar, inline);

    /// <summary>
    /// Obtains the password pressed by the user.
    /// </summary>
    /// <param name="foreground">The replace charactor color.</param>
    /// <param name="replaceChar">The optional charactor to output to replace the original one, such as *.</param>
    /// <param name="inline">true if do not follow the line terminator after typing the password; otherwise, false.</param>
    /// <returns>
    /// The password.
    /// </returns>
    public SecureString ReadPassword(ConsoleColor? foreground, char? replaceChar, bool inline = false)
    {
        Flush();
        var str = new SecureString();
        var normalMode = Mode != Modes.Text;
        while (true)
        {
            ConsoleKeyInfo key;
            try
            {
                key = ReadKey(true);
            }
            catch (InvalidOperationException)
            {
                ReadLine(foreground, str, replaceChar, inline);
                return str;
            }
            catch (IOException)
            {
                ReadLine(foreground, str, replaceChar, inline);
                return str;
            }
            catch (NotSupportedException)
            {
                ReadLine(foreground, str, replaceChar, inline);
                return str;
            }

            var len = str.Length;
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    if (replaceChar.HasValue)
                    {
                        if (!normalMode)
                            col.Add(new ConsoleText(replaceChar.Value, 6, foreground));
                        else if (len < 6)
                            col.Add(new ConsoleText(replaceChar.Value, 6 - len, foreground));
                        else if (len > 6)
                            Backspace(len - 6);
                    }

                    if (!inline) col.Add(new ConsoleText(Environment.NewLine));
                    Flush();
                    return str;
                case ConsoleKey.Escape:
                    str.Dispose();
                    return null;
                case ConsoleKey.Backspace:
                    if (key.Modifiers == ConsoleModifiers.Shift || key.Modifiers == ConsoleModifiers.Control)
                    {
                        str.Clear();
                        if (replaceChar.HasValue && normalMode) Backspace(len);
                        break;
                    }

                    if (str.Length == 0) break;
                    str.RemoveAt(str.Length - 1);
                    if (normalMode)
                    {
                        Append(' ');
                        Backspace(replaceChar.HasValue ? 2 : 1);
                    }

                    break;
                case ConsoleKey.Delete:
                case ConsoleKey.Clear:
                case ConsoleKey.F5:
                    str.Clear();
                    if (replaceChar.HasValue) Backspace(len);
                    break;
                default:
                    var hasKey = key.KeyChar != '\0';
                    if (hasKey) str.AppendChar(key.KeyChar);
                    if (hasKey && replaceChar.HasValue && normalMode)
                    {
                        if (foreground.HasValue)
                            Write(foreground.Value, replaceChar.Value);
                        else
                            Write(replaceChar.Value);
                    }

                    break;
            }
        }
    }

    /// <summary>
    /// Moves cursor by a specific relative position.
    /// </summary>
    /// <param name="x">The horizontal translation size.</param>
    /// <param name="y">The vertical translation size.</param>
    public void MoveCursorBy(int x, int y = 0)
    {
        var h = Handler;
        if (h != null)
        {
            h.MoveCursorBy(x, y, context);
            CursorMovedBy?.Invoke(this, new Maths.IntPoint2D.DataEventArgs(x, y));
            return;
        }

        TestMode();
        switch (Mode)
        {
            case Modes.Cmd:
                if (y != 0)
                {
                    try
                    {
                        Console.CursorTop += y;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (y > 0)
                        {
                            Console.CursorTop = Math.Min(CursorTop + y, BufferHeight - 1);
                            Write(Environment.NewLine);
                        }
                        else
                        {
                            Console.CursorTop = Math.Max(CursorTop + y, 0);
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                if (x != 0)
                {
                    try
                    {
                        Console.CursorLeft += x;
                    }
                    catch (InvalidOperationException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        if (BufferWidth < 8) throw;
                        Console.CursorLeft = x > 0
                            ? Math.Min(CursorLeft + x, BufferWidth - 1)
                            : Math.Max(CursorLeft + x, 0);
                    }
                    catch (ArgumentException)
                    {
                    }
                    catch (NotSupportedException)
                    {
                    }
                }

                break;
            case Modes.Text:
                if (y > 0 && col.LastOrDefault()?.Content?.ToString() != Environment.NewLine)
                    col.Add(new ConsoleText(Environment.NewLine));
                break;
            default:
                Write(AnsiCodeGenerator.MoveCursorBy(x, y));
                break;
        }

        CursorMovedBy?.Invoke(this, new Maths.IntPoint2D.DataEventArgs(x, y));
    }

    /// <summary>
    /// Moves cursor at a specific position in buffer.
    /// </summary>
    /// <param name="x">Column (zero-based), the left from the edge of buffer.</param>
    /// <param name="y">Row (zero-based) the top from the edge of buffer; or -1 if do not move.</param>
    public void MoveCursorTo(int x, int y)
    {
        var h = Handler;
        if (h != null)
        {
            h.MoveCursorTo(x, y, context);
            CursorMovedTo?.Invoke(this, new Maths.IntPoint2D.DataEventArgs(x, y));
            return;
        }

        TestMode();
        switch (Mode)
        {
            case Modes.Cmd:
                try
                {
                    if (y >= 0) Console.CursorTop = y;
                }
                catch (InvalidOperationException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.CursorTop = Math.Min(y, BufferHeight - 1);
                    Write(Environment.NewLine);
                }
                catch (ArgumentException)
                {
                }
                catch (NotSupportedException)
                {
                }

                try
                {
                    if (x < 0)
                    {
                        x = BufferWidth + x;
                        if (x < 0) x = 0;
                    }

                    Console.CursorLeft = x;
                }
                catch (InvalidOperationException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (BufferWidth < 8) throw;
                    Console.CursorLeft = Math.Min(x, BufferWidth - 1);
                }
                catch (ArgumentException)
                {
                }
                catch (NotSupportedException)
                {
                }

                break;
            case Modes.Text:
                break;
            default:
                Write(AnsiCodeGenerator.MoveCursorTo(x, y));
                break;
        }

        CursorMovedTo?.Invoke(this, new Maths.IntPoint2D.DataEventArgs(x, y));
    }

    /// <summary>
    /// Removes the specific area.
    /// </summary>
    /// <param name="area">The area to remove.</param>
    public void Clear(RelativeAreas area)
    {
        Flush();
        var h = Handler;
        if (h != null)
        {
            h.Remove(area, context);
            Cleared?.Invoke(this, new DataEventArgs<RelativeAreas>(area));
            return;
        }

        switch (Mode)
        {
            case Modes.Cmd:
                try
                {
                    ClearInCmd(area);
                    break;
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

                Write(AnsiCodeGenerator.Clear(area));
                break;
            case Modes.Text:
                switch (area)
                {
                    case RelativeAreas.ToEndOfScreen:
                    case RelativeAreas.ToEndOfLine:
                        col.Add(new ConsoleText(" \b"));
                        break;
                    case RelativeAreas.None:
                        break;
                    default:
                        Append("\b \b");
                        break;
                }

                break;
            default:
                Write(AnsiCodeGenerator.Clear(area));
                break;
        }

        Cleared?.Invoke(this, new DataEventArgs<RelativeAreas>(area));
    }

    /// <summary>
    /// Tests if the output of the instance always contain a line terminator.
    /// </summary>
    /// <param name="instance">The instance of the console handler wrapper.</param>
    /// <returns>true if it will always include a line terminator on output; otherwise, false.</returns>
    public static bool IsBlockOutputOnly(IConsoleTextCreator instance)
        => instance?.ContainsTerminator ?? false;

    private void ReadLine(ConsoleColor? foreground, SecureString str, char? replaceChar, bool inline = false)
    {
        var s = ReadLine();
        foreach (var c in s)
        {
            str.AppendChar(c);
        }

        MoveCursorBy(0, -1);
        Clear(RelativeAreas.Line);
        if (inline && replaceChar.HasValue)
            col.Add(new ConsoleText(replaceChar.Value, 6, foreground));
    }

    /// <summary>
    /// Enters a backspace to console to remove the last charactor.
    /// </summary>
    /// <param name="count">The count of the charactor to remove from end.</param>
    /// <param name="keepLevel">0 to remove; 1 to remove but keep cursor position; 2 to only move cursor.</param>
    private void BackspaceInternal(int count = 1, int keepLevel = 0)
    {
        if (count < 1) return;
        if (Mode == Modes.Text && Handler == null)
        {
            col.Add(new ConsoleText(" \b "));
            return;
        }

        var str = new StringBuilder();
        str.Append('\b', count);
        switch (keepLevel)
        {
            case -1:
            case 0:
                str.Append(' ', count);
                str.Append('\b', count);
                break;
            case 1:
                str.Append(' ', count);
                break;
        }

        col.Add(new ConsoleText(str));
        if (keepLevel >= 0) Flush();
    }

    private void ClearInCmd(RelativeAreas area)
    {
        switch (area)
        {
            case RelativeAreas.Line:
                try
                {
                    var l = CursorLeft;
                    BackspaceInternal(l, 1);
                    if (BufferWidth < 8) break;
                    Console.CursorLeft = BufferWidth - 1;
                    BackspaceInternal(BufferWidth, 1);
                    Console.SetCursorPosition(l, CursorTop - 1);
                }
                catch (SecurityException)
                {
                    BackspaceInternal(2, 1);
                    throw;
                }

                break;
            case RelativeAreas.ToBeginningOfLine:
                try
                {
                    BackspaceInternal(CursorLeft, 1);
                }
                catch (SecurityException)
                {
                    BackspaceInternal(2, 1);
                    throw;
                }

                break;
            case RelativeAreas.ToEndOfLine:
                {
                    var w = BufferWidth - CursorLeft - 1;
                    Console.CursorLeft = BufferWidth - 1;
                    BackspaceInternal(w);
                    break;
                }
            case RelativeAreas.EntireScreen:
                try
                {
                    var l = CursorLeft;
                    var t = CursorTop;
                    Console.SetCursorPosition(BufferWidth - 1, Math.Max(0, CursorTop - 30));
                    for (; CursorTop <= t;)
                    {
                        Console.CursorLeft = BufferWidth - 1;
                        BackspaceInternal(BufferWidth, 1);
                    }

                    var i = 0;
                    for (; i < 12; i++)
                    {
                        Console.CursorLeft = BufferWidth - 1;
                        BackspaceInternal(BufferWidth, 1);
                    }

                    Console.SetCursorPosition(l, Math.Max(0, CursorTop - i - 1));
                }
                catch (ArgumentException)
                {
                }
                catch (SecurityException)
                {
                    BackspaceInternal(2, 1);
                    throw;
                }

                break;
            case RelativeAreas.ToBeginningOfScreen:
                try
                {
                    var l = CursorLeft;
                    var t = CursorTop;
                    Console.SetCursorPosition(BufferWidth - 1, Math.Max(0, CursorTop - 30));
                    for (; CursorTop < t;)
                    {
                        Console.CursorLeft = BufferWidth - 1;
                        BackspaceInternal(BufferWidth, 1);
                    }

                    Console.CursorLeft = l;
                    BackspaceInternal(CursorLeft, 1);
                }
                catch (ArgumentException)
                {
                }
                catch (SecurityException)
                {
                    BackspaceInternal(2, 1);
                    throw;
                }

                break;
            case RelativeAreas.ToEndOfScreen:
                {
                    var l = CursorLeft;
                    var w = BufferWidth - CursorLeft - 1;
                    Console.CursorLeft = BufferWidth - 1;
                    BackspaceInternal(w, 1);
                    Console.CursorTop++;
                    var i = 0;
                    for (; i < 12; i++)
                    {
                        Console.CursorLeft = BufferWidth - 1;
                        BackspaceInternal(BufferWidth, 1);
                    }

                    Console.SetCursorPosition(l, Math.Max(0, CursorTop - i - 1));
                    break;
                }
            case RelativeAreas.EntireBuffer:
                Console.Clear();
                break;
        }
    }

    private void TestMode()
    {
        if (Mode != 0 || Handler is not null)
            return;
#if NETFRAMEWORK
        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Unix:
            case PlatformID.MacOSX:
                Mode = Modes.Ansi;
                return;
        }
#else
        if (!OperatingSystem.IsWindows())
        {
            Mode = OperatingSystem.IsBrowser() ? Modes.Text : Modes.Ansi;
            return;
        }
#endif
        var left = -1;
        try
        {
            left = CursorLeft;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (IOException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
        }

        if (left < 0)
        {
            Mode = Modes.Text;
            return;
        }

        var s = AnsiCodeGenerator.Background(true);
        Console.Write(s);
        if (left == CursorLeft)
        {
            Mode = Modes.Ansi;
            return;
        }

        try
        {
            Console.CursorLeft = left;
            Mode = Modes.Cmd;
            var sb = new StringBuilder();
            sb.Append('\b', s.Length);
            sb.Append(' ', s.Length);
            sb.Append('\b', s.Length);
            Console.Write(sb.ToString());
            return;
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (IOException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (System.Runtime.InteropServices.ExternalException)
        {
        }

        Console.WriteLine();
        Mode = Modes.Text;
    }
}
