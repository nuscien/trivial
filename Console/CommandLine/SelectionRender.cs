﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security;

using Trivial.Collection;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The extensions for console renderer.
    /// </summary>
    public static partial class ConsoleRenderExtensions
    {
        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<object> Select(this StyleConsole cli, SelectionData collection, SelectionConsoleOptions options = null)
            => Select<object>(cli, collection, options);

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="convert">The converter.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(this StyleConsole cli, IEnumerable<T> collection, Func<T, SelectionItem<T>> convert, SelectionConsoleOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection.Select(convert));
            return Select(cli, c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(this StyleConsole cli, IEnumerable<SelectionItem<T>> collection, SelectionConsoleOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection);
            return Select(cli, c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="path">The parent foler path.</param>
        /// <param name="options">The selection display options.</param>
        /// <param name="searchPattern">The search string to match against the names of directories and files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="ArgumentException">searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars method.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(this StyleConsole cli, DirectoryInfo path, SelectionConsoleOptions options = null, string searchPattern = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            var col = string.IsNullOrEmpty(searchPattern) ? path.GetFileSystemInfos() : path.GetFileSystemInfos(searchPattern);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(cli, c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="path">The parent foler path.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(this StyleConsole cli, DirectoryInfo path, Func<FileSystemInfo, bool> predicate, SelectionConsoleOptions options = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            IEnumerable<FileSystemInfo> col = path.GetFileSystemInfos();
            if (predicate != null) col = col.Where(predicate);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(cli, c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(this StyleConsole cli, SelectionData<T> collection, SelectionConsoleOptions options = null)
        {
            if (collection == null) return null;
            if (cli == null) cli = StyleConsole.Default;
            if (options == null) options = new();
            else options = options.Clone();
            cli.Flush();
            if (cli.Handler == null && cli.Mode == StyleConsole.Modes.Text)
                return SelectForText(cli, collection, options);
            return Select(cli, collection, options, 0);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <param name="select">The index of item selected.</param>
        /// <returns>The result of selection.</returns>
        private static SelectionResult<T> Select<T>(StyleConsole cli, SelectionData<T> collection, SelectionConsoleOptions options, int select)
        {
            var temp = (0, 0, 0, 0, false, false, 0, 0);
            var oldSelect = select;
            while (true)
            {
                var list = collection.ToList();
                void resetSelect()
                {
                    if (oldSelect < 0 || oldSelect >= list.Count || oldSelect == select) return;
                    var h = temp.Item3 + (temp.Item5 ? 2 : 1) + (temp.Item6 ? 1 : 0);
                    var oldItem = list[oldSelect];
                    var y2 = Math.DivRem(oldSelect % temp.Item7, temp.Item4, out var x2) - h;
                    x2 *= temp.Item8;
                    cli.MoveCursorBy(x2, y2);
                    RenderData(cli, oldItem, options, false, temp.Item8);
                    cli.MoveCursorBy(-x2 - temp.Item8, -y2);
                };

                if (temp.Item3 > 0 && select >= temp.Item1 && select < (temp.Item1 + temp.Item2))
                {
                    cli.BackspaceToBeginning();
                    var h = temp.Item3 + (temp.Item5 ? 2 : 1) + (temp.Item6 ? 1 : 0);
                    resetSelect();
                    if (select < 0 || select >= list.Count) select = 0;
                    var item = list[select];
                    var y = Math.DivRem(select % temp.Item7, temp.Item4, out var x) - h;
                    x *= temp.Item8;
                    cli.MoveCursorBy(x, y);
                    RenderData(cli, item, options, true, temp.Item8);
                    cli.MoveCursorBy(-x - temp.Item8, -y);
                    RenderSelectResult(cli, item?.Title, options);
                }
                else
                {
                    if (temp.Item3 > 0)
                    {
                        cli.BackspaceToBeginning();
                        var h = temp.Item3 + (temp.Item5 ? 2 : 1) + (temp.Item6 ? 1 : 0);
                        for (var i = 0; i < h; i++)
                        {
                            cli.MoveCursorBy(0, -1);
                            cli.Clear(StyleConsole.RelativeAreas.Line);
                        }
                    }

                    temp = RenderData(cli, list, options, select);
                }

                oldSelect = select;
                ConsoleKeyInfo key;
                try
                {
                    key = cli.ReadKey();
                }
                catch (InvalidOperationException)
                {
                    return SelectByManualTyping(cli, collection, options);
                }
                catch (IOException)
                {
                    return SelectByManualTyping(cli, collection, options);
                }
                catch (SecurityException)
                {
                    return SelectByManualTyping(cli, collection, options);
                }
                catch (NotSupportedException)
                {
                    return SelectByManualTyping(cli, collection, options);
                }

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                    case ConsoleKey.Select:
                    case ConsoleKey.Spacebar:
                        if (select < 0 || select >= list.Count)
                        {
                            select = temp.Item1;
                            if (select < 0 || select >= list.Count)
                                select = 0;
                            break;
                        }

                        var sel = list[select];
                        RenderSelectResult(cli, sel?.Title, options);
                        cli.WriteLine();
                        return new SelectionResult<T>(sel.Title, select, sel.Data, sel.Title);
                    case ConsoleKey.Backspace:
                    case ConsoleKey.Delete:
                    case ConsoleKey.Clear:
                    case ConsoleKey.F4:
                        cli.Write(' ');
                        cli.BackspaceToBeginning();
                        resetSelect();
                        return SelectByManualTyping(cli, collection, options);
                    case ConsoleKey.Escape:
                    case ConsoleKey.Pause:
                        cli.Write(' ');
                        cli.BackspaceToBeginning();
                        resetSelect();
                        RenderSelectResult(cli, null, options);
                        cli.WriteLine();
                        return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
                    case ConsoleKey.F1:
                        {
                            cli.BackspaceToBeginning();
                            resetSelect();
                            RenderSelectResult(cli, "?", options);
                            cli.WriteLine();
                            var item = collection.Get('?', out select);
                            return item == null
                                ? new SelectionResult<T>("?", SelectionResultTypes.Selected)
                                : new SelectionResult<T>("?", select, item.Data, item.Title);
                        }
                    case ConsoleKey.F12:
                        {
                            cli.BackspaceToBeginning();
                            resetSelect();
                            RenderSelectResult(cli, "?", options);
                            cli.WriteLine();
                            var item = collection.Get('?', out select);
                            return item == null
                                ? new SelectionResult<T>("?", SelectionResultTypes.Canceled)
                                : new SelectionResult<T>("?", select, item.Data, item.Title);
                        }
                    case ConsoleKey.F5:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                            select = 0;
                        break;
                    case ConsoleKey.PageUp:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                            select = 0;
                        else
                            select = Math.Max(0, temp.Item1 - temp.Item7);
                        break;
                    case ConsoleKey.PageDown:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                            select = list.Count - 1;
                        else
                            select = Math.Min(list.Count - 1, temp.Item1 + temp.Item7);
                        break;
                    case ConsoleKey.UpArrow:
                        if (select < temp.Item4)
                        {
                            select += list.Count - (list.Count % temp.Item4) - temp.Item4;
                            if (select >= list.Count) select = list.Count - 1;
                            else if (select < 0) select = 0;
                        }
                        else
                        {
                            select -= temp.Item4;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        select += temp.Item4;
                        if (select >= list.Count)
                        {
                            select %= temp.Item4;
                            if (select >= list.Count) select = list.Count - 1;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        select--;
                        if (select < 0) select = list.Count - 1;
                        break;
                    case ConsoleKey.RightArrow:
                        select++;
                        if (select >= list.Count) select = 0;
                        break;
                    case ConsoleKey.Home:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                            select = 0;
                        else
                            select = temp.Item1;
                        break;
                    case ConsoleKey.End:
                        if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                            select = list.Count - 1;
                        else
                            select = temp.Item1 + temp.Item2 - 1;
                        break;
                    default:
                        {
                            var item = collection.Get(key.KeyChar, out select);
                            if (item == null)
                            {
                                select = oldSelect;
                                continue;
                            }

                            cli.Write(' ');
                            cli.BackspaceToBeginning();
                            resetSelect();
                            RenderSelectResult(cli, item.Title, options);
                            cli.WriteLine();
                            return new SelectionResult<T>(item.Title, select, item.Data, item.Title);
                        }
                }
            }
        }

        private static SelectionResult<T> SelectByManualTyping<T>(StyleConsole cli, SelectionData<T> collection, SelectionConsoleOptions options)
        {
            cli.BackspaceToBeginning();
            cli.WriteImmediately(
                options.QuestionForegroundColor ?? options.ForegroundColor,
                options.QuestionBackgroundColor ?? options.BackgroundColor,
                options.ManualQuestion ?? options.Question);
            string s;
            try
            {
                s = cli.ReadLine();
            }
            catch (IOException)
            {
                return new SelectionResult<T>(string.Empty, SelectionResultTypes.NotSupported);
            }
            catch (InvalidOperationException)
            {
                return new SelectionResult<T>(string.Empty, SelectionResultTypes.NotSupported);
            }
            catch (ArgumentException)
            {
                return new SelectionResult<T>(string.Empty, SelectionResultTypes.NotSupported);
            }
            catch (NotSupportedException)
            {
                return new SelectionResult<T>(string.Empty, SelectionResultTypes.NotSupported);
            }

            var i = -1;
            SelectionItem<T> item = null;
            foreach (var ele in collection.ToList())
            {
                i++;
                if (ele.Title != s)
                    continue;
                item = ele;
                break;
            }

            return item == null
                ? new SelectionResult<T>(s, SelectionResultTypes.Typed)
                : new SelectionResult<T>(s, i, item.Data, item.Title, SelectionResultTypes.Typed);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <param name="select">The index of item selected.</param>
        /// <returns>The result of selection: offset, count, rows, columns, paging tips, customized tips, page size, item length.</returns>
        private static (int, int, int, int, bool, bool, int, int) RenderData<T>(this StyleConsole cli, List<SelectionItem<T>> collection, SelectionConsoleOptions options, int select)
        {
            var maxWidth = GetBufferWidth();
            var itemLen = options.Column.HasValue ? (int)Math.Floor(maxWidth * 1.0 / options.Column.Value) : maxWidth;
            if (options.MaxLength.HasValue) itemLen = Math.Min(options.MaxLength.Value, itemLen);
            if (options.MinLength.HasValue) itemLen = Math.Max(options.MinLength.Value, itemLen);
            if (itemLen > maxWidth) itemLen = maxWidth;
            var columns = (int)Math.Floor(maxWidth * 1.0 / itemLen);
            if (options.Column.HasValue && columns > options.Column.Value) columns = options.Column.Value;
            var maxRows = 50;
            try
            {
                maxRows = Console.BufferHeight - 5;
                if (maxRows < 1) maxRows = 50;
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
            catch (NotSupportedException)
            {
            }

            if (options.MaxRow.HasValue && options.MaxRow.Value < maxRows)
                maxRows = options.MaxRow.Value;
            var pageSize = columns * maxRows;
            var needPaging = collection.Count > pageSize;
            if (select >= collection.Count) select = collection.Count - 1;
            var list = collection;
            var offset = 0;
            if (select >= pageSize)
            {
                offset = (int)Math.Floor(select * 1.0 / pageSize) * pageSize;
                list = list.Skip(offset).Take(pageSize).ToList();
            }
            else if (needPaging)
            {
                list = list.Take(pageSize).ToList();
            }

            var i = offset;
            var lastColIndex = columns - 1;
            var rows = -1;
            SelectionItem<T> selItem = null;
            foreach (var item in list)
            {
                if (string.IsNullOrEmpty(item.Title)) continue;
                var isSel = i == select;
                if (isSel) selItem = item;
                RenderData(cli, item, options, isSel, itemLen);
                var indexInRow = i % columns;
                if (indexInRow == lastColIndex)
                    cli.Write(Environment.NewLine);
                else if (indexInRow == 0)
                    rows++;
                i++;
            }

            if (list.Count % columns > 0) cli.Write(Environment.NewLine);
            var hasPagingTips = false;
            var tipsP = options.PagingTips;
            if (needPaging && !string.IsNullOrEmpty(tipsP))
            {
                cli.Write(
                    options.PagingForegroundColor ?? options.ForegroundColor,
                    options.PagingBackgroundColor ?? options.BackgroundColor,
                    tipsP
                        .Replace("{from}", (offset + 1).ToString())
                        .Replace("{end}", (offset + list.Count).ToString())
                        .Replace("{count}", list.Count.ToString("g"))
                        .Replace("{size}", pageSize.ToString("g"))
                        .Replace("{total}", collection.Count.ToString("g")));
                cli.Write(Environment.NewLine);
                hasPagingTips = true;
            }

            var hasTips = false;
            if (!string.IsNullOrEmpty(options.Tips))
            {
                cli.Write(
                    options.TipsForegroundColor ?? options.ForegroundColor,
                    options.TipsBackgroundColor ?? options.BackgroundColor,
                    options.Tips);
                cli.Write(Environment.NewLine);
                hasTips = true;
            }

            RenderSelectResult(cli, selItem?.Title, options);
            return (offset, list.Count, rows, columns, hasPagingTips, hasTips, pageSize, itemLen);
        }

        private static void RenderSelectResult(StyleConsole cli, string value, SelectionConsoleOptions options)
        {
            cli.Write(
                options.QuestionForegroundColor ?? options.ForegroundColor,
                options.QuestionBackgroundColor ?? options.BackgroundColor,
                options.Question);
            if (!string.IsNullOrWhiteSpace(value))
                cli.WriteImmediately(options.ForegroundColor, options.BackgroundColor, value);
            else
                cli.Flush();
        }

        private static void RenderData<T>(StyleConsole cli, SelectionItem<T> item, SelectionConsoleOptions options, bool isSelect, int len)
        {
            var foreground = (isSelect ? options.SelectedForegroundColor : options.DefaultValueForegroundColor) ?? options.ForegroundColor;
            var background = (isSelect ? options.SelectedBackgroundColor : options.DefaultValueBackgroundColor) ?? options.BackgroundColor;
            var sb = new StringBuilder();
            var j = 0;
            var maxLen = len - 1;
            var curLeft = -1;
            try
            {
                curLeft = cli.CursorLeft;
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
            catch (NotSupportedException)
            {
            }

            foreach (var c in (isSelect ? options.SelectedPrefix : options.Prefix) ?? string.Empty)
            {
                var c2 = c;
                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        j++;
                        c2 = ' ';
                        break;
                    case '\0':
                    case '\b':
                        continue;
                    default:
                        j += GetLetterWidth(c);
                        break;
                }

                if (j >= maxLen) break;
                sb.Append(c2);
            }

            foreach (var c in item.Title)
            {
                var c2 = c;
                switch (c)
                {
                    case '\t':
                    case '\r':
                    case '\n':
                        j++;
                        c2 = ' ';
                        break;
                    case '\0':
                    case '\b':
                        continue;
                    default:
                        j += GetLetterWidth(c);
                        break;
                }

                if (j >= maxLen) break;
                sb.Append(c2);
            }

            sb.Append(' ', len - j);
            cli.Write(foreground, background, sb);
            try
            {
                if (curLeft >= 0)
                {
                    curLeft += len;
                    if (cli.CursorLeft != len)
                        curLeft = cli.CursorLeft;
                }
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
            catch (NotSupportedException)
            {
            }
        }

        private static int GetLetterWidth(char c)
        {
            if (c < 0x2E80) return 1;
            return c < 0xA500 || (c >= 0xF900 && c < 0xFB00) || (c >= 0xFE30 && c < 0xFE70)
                ? 2
                : 1;
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="cli">The command line interface proxy.</param>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        private static SelectionResult<T> SelectForText<T>(StyleConsole cli, SelectionData<T> collection, SelectionConsoleOptions options)
        {
            var list = collection.ToList();
            cli.WriteLines(list.Select((ele, i) => $"#{i + 1}\t{ele.Title}"));
            cli.Write(
                options.QuestionForegroundColor ?? options.ForegroundColor,
                options.QuestionBackgroundColor ?? options.BackgroundColor,
                options.QuestionWhenNotSupported ?? options.ManualQuestion ?? options.Question);
            string text;
            try
            {
                text = cli.ReadLine();
            }
            catch (NotSupportedException)
            {
                return new SelectionResult<T>(null, SelectionResultTypes.NotSupported);
            }
            catch (InvalidOperationException)
            {
                return new SelectionResult<T>(null, SelectionResultTypes.NotSupported);
            }
            catch (IOException)
            {
                return new SelectionResult<T>(null, SelectionResultTypes.NotSupported);
            }
            catch (ArgumentException)
            {
                return new SelectionResult<T>(null, SelectionResultTypes.NotSupported);
            }

            if (string.IsNullOrEmpty(text))
            {
                cli.Write(
                    options.QuestionForegroundColor ?? options.ForegroundColor,
                    options.QuestionBackgroundColor ?? options.BackgroundColor,
                    options.QuestionWhenNotSupported ?? options.ManualQuestion ?? options.Question);
                text = cli.ReadLine();
                if (string.IsNullOrEmpty(text))
                    return new SelectionResult<T>(text, SelectionResultTypes.Canceled);
            }

            SelectionItem<T> item = null;
            int i;
            if (text.Trim().Length == 1)
            {
                item = collection.Get(text[0], out i);
                if (item != null)
                    return new SelectionResult<T>(text, i, item.Data, item.Title);
            }

            if (text.StartsWith("#") && int.TryParse(text.Substring(1).Trim(), out i) && i > 0 && i <= list.Count)
            {
                item = list[i];
                return new SelectionResult<T>(text, i, item.Data, item.Title);
            }

            i = -1;
            foreach (var ele in list)
            {
                i++;
                if (ele.Title != text)
                    continue;
                item = ele;
                break;
            }

            if (item != null)
                return new SelectionResult<T>(text, i, item.Data, item.Title, SelectionResultTypes.Typed);

            if (int.TryParse(text.Trim(), out i) && i > 0 && i <= list.Count)
            {
                item = list[i];
                return new SelectionResult<T>(text, i, item.Data, item.Title);
            }

            return new SelectionResult<T>(text, SelectionResultTypes.Typed);
        }
    }
}
