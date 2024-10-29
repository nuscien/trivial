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

namespace Trivial.CommandLine;

/// <summary>
/// The command line interface.
/// </summary>
public sealed partial class StyleConsole
{
    /// <summary>
    /// The lock.
    /// </summary>
    private readonly object locker = new();

    /// <summary>
    /// The cache.
    /// </summary>
    private readonly List<ConsoleText> col = new();

    /// <summary>
    /// Gets the collection of output cache.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<ConsoleText> GetOutputCache()
    {
        IReadOnlyList<ConsoleText> list;
        lock (locker)
        {
            list = col.ToList().AsReadOnly();
        }

        return list;
    }

    /// <summary>
    /// Clears output cache.
    /// </summary>
    public void ClearOutputCache()
    {
        lock (locker)
        {
            col.Clear();
        }
    }

    /// <summary>
    /// Gets the last console text instance in output cache.
    /// </summary>
    /// <returns>The console text found; or null, if empty.</returns>
    public ConsoleText LastOfOutputCache()
        => col.LastOrDefault();

    /// <summary>
    /// Gets the last console text instance in output cache if has, or create one.
    /// </summary>
    /// <returns>The console text.</returns>
    public ConsoleText LastOrCreateOutputCache()
    {
        var item = col.LastOrDefault();
        if (item != null) return item;
        item = new ConsoleText();
        col.Add(item);
        return item;
    }

    /// <summary>
    /// Tests if the given console text is in the output cache..
    /// </summary>
    /// <param name="item">The item to test.</param>
    /// <returns>true if contains; otherwise, false.</returns>
    public bool OutputCacheContains(ConsoleText item)
    {
        var has = false;
        lock (locker)
        {
            has = col.Contains(item);
        }

        return has;
    }

    /// <summary>
    /// Flushes all data.
    /// </summary>
    public void Flush()
    {
        TestMode();
        if (col.Count == 0) return;
        IReadOnlyList<ConsoleText> list;
        lock (locker)
        {
            list = col.ToList().AsReadOnly();
            col.Clear();
        }

        var h = Handler;
        if (h != null)
        {
            h.Write(list, context);
            return;
        }

        if (Mode == Modes.Cmd)
        {
            foreach (var item in list)
            {
                var s = item?.Content?.ToString();
                if (string.IsNullOrEmpty(s)) continue;
                var hasSetColor = false;
                try
                {
                    if (item.Style.ForegroundConsoleColor.HasValue)
                    {
                        Console.ForegroundColor = item.Style.ForegroundConsoleColor.Value;
                        hasSetColor = true;
                    }
                    else if (item.Style.ForegroundRgbColor.HasValue)
                    {
                    }

                    if (item.Style.BackgroundConsoleColor.HasValue)
                    {
                        Console.BackgroundColor = item.Style.BackgroundConsoleColor.Value;
                        hasSetColor = true;
                    }
                    else if (item.Style.BackgroundRgbColor.HasValue)
                    {
                    }
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
                catch (SecurityException)
                {
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                }

                Console.Write(s);
                if (hasSetColor) Console.ResetColor();
            }
        }
        else if (Mode == Modes.Text)
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                if (item?.Content != null)
                    StringExtensions.Append(sb, item?.Content);
            }

            Console.Write(sb.ToString());
        }
        else
        {
            var sb = new StringBuilder();
            foreach (var item in list)
            {
                if (item?.Content == null) continue;
                item.AppendTo(sb);
            }

            Console.Write(sb.ToString());
        }

        Flushed?.Invoke(this, new DataEventArgs<IReadOnlyList<ConsoleText>>(list));
    }

    private void OnAppend()
    {
    }

    private int BackspaceOutputCache(int count)
    {
        lock (locker)
        {
            var item = col.LastOrDefault();
            while (item != null && count > 0)
            {
                var len = item.Length;
                if (len < 3 && item.Content.ToString() == Environment.NewLine)
                    return 0;
                if (len > count)
                {
                    try
                    {
                        item.Content.Remove(item.Content.Length - count, count);
                        return 0;
                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                try
                {
                    col.RemoveAt(col.Count - 1);
                    count -= len;
                }
                catch (NullReferenceException)
                {
                }
                catch (ArgumentException)
                {
                }

                item = col.LastOrDefault();
            }
        }

        return count;
    }
}
