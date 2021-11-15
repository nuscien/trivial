using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The utilities.
    /// </summary>
    public static class LineUtilities
    {
        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        /// <param name="doNotRemoveOutput">true if just only move cursor back and keep output; otherwise, false.</param>
        public static void Backspace(int count = 1, bool doNotRemoveOutput = false)
        {
            var str = new StringBuilder();
            str.Append('\b', count);
            if (!doNotRemoveOutput)
            {
                str.Append(' ', count);
                str.Append('\b', count);
            }

            System.Console.Write(str.ToString());
        }

        /// <summary>
        /// Clear current line.
        /// </summary>
        /// <param name="row">The additional row index to clear; or null, if clear the line where the cursor is.</param>
        public static void ClearLine(int? row = null)
        {
            try
            {
                SetCursorPosition(System.Console.BufferWidth - 1, row ?? System.Console.CursorTop);
                Backspace(System.Console.CursorLeft);
            }
            catch (SecurityException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (IOException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void Write(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var c = System.Console.ForegroundColor;
            SetColor(foregroundColor, false);
            if (arg.Length > 0) System.Console.Write(value, arg);
            else System.Console.Write(value);
            SetColor(c, false);
        }

        /// <summary>
        /// Writes the specified string value to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void Write(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            var fore = System.Console.ForegroundColor;
            var back = System.Console.BackgroundColor;
            SetColor(foregroundColor, false);
            SetColor(backgroundColor, true);
            if (arg.Length > 0) System.Console.Write(value, arg);
            else System.Console.Write(value);
            SetColor(fore, false);
            SetColor(back, true);
        }

        /// <summary>
        /// Writes the current line terminator, to the standard output stream.
        /// </summary>
        public static void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes a progress component, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>The progress result.</returns>
        public static ProgressLineResult WriteLine(ProgressLineOptions options)
        {
            return WriteLine(null, options);
        }

        /// <summary>
        /// Writes a progress component, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="progressSize">The progress size.</param>
        /// <param name="style">The progress style.</param>
        /// <returns>The progress result.</returns>
        public static ProgressLineResult WriteLine(string caption, ProgressLineOptions.Sizes progressSize, ProgressLineOptions.Styles style = ProgressLineOptions.Styles.Full)
        {
            return WriteLine(caption, progressSize != ProgressLineOptions.Sizes.None ? new ProgressLineOptions
            {
                Size = progressSize,
                Style = style
            } : null);
        }

        /// <summary>
        /// Writes a progress component, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="progressSize">The progress size.</param>
        /// <param name="style">The progress style.</param>
        /// <returns>The progress result.</returns>
        public static ProgressLineResult WriteLine(ProgressLineOptions.Sizes progressSize, ProgressLineOptions.Styles style = ProgressLineOptions.Styles.Full)
        {
            return WriteLine(null, progressSize != ProgressLineOptions.Sizes.None ? new ProgressLineOptions
            {
                Size = progressSize,
                Style = style
            } : null);
        }

        /// <summary>
        /// Writes a progress component, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
        /// <param name="options">The options.</param>
        /// <returns>The progress result.</returns>
        public static ProgressLineResult WriteLine(string caption, ProgressLineOptions options)
        {
            if (options == null) options = ProgressLineOptions.Empty;
            var winWidth = 70;
            try
            {
                winWidth = System.Console.WindowWidth;
            }
            catch (IOException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }

            var width = options.Size switch
            {
                ProgressLineOptions.Sizes.None => 0,
                ProgressLineOptions.Sizes.Short => winWidth > 70 ? 20 : 10,
                ProgressLineOptions.Sizes.Wide => winWidth > 88 ? 60 : 40,
                _ => winWidth > 70 ? (winWidth > 88 ? 40 : 30) : 20
            };

            var sb = new StringBuilder();
            var barChar = options.Style switch
            {
                ProgressLineOptions.Styles.AngleBracket => '>',
                ProgressLineOptions.Styles.Plus => '+',
                ProgressLineOptions.Styles.Sharp => '#',
                ProgressLineOptions.Styles.X => options.UsePendingBackgroundForAll ? 'X' : 'x',
                ProgressLineOptions.Styles.O => options.UsePendingBackgroundForAll ? 'O' : 'o',
                _ => ' ',
            };
            var pendingChar = options.UsePendingBackgroundForAll ? ' ' : (options.Style switch
            {
                ProgressLineOptions.Styles.AngleBracket => '=',
                ProgressLineOptions.Styles.Plus => '-',
                ProgressLineOptions.Styles.Sharp => '-',
                ProgressLineOptions.Styles.X => '.',
                ProgressLineOptions.Styles.O => '.',
                _ => ' ',
            });
            sb.Append(pendingChar, width);
            var fore = System.Console.ForegroundColor;
            var back = System.Console.BackgroundColor;
            var background = options.BackgroundColor ?? back;
            var barBackground = background;
            var progressBackground = background;
            if (barChar == ' ')
            {
                barBackground = options.BarColor;
                progressBackground = options.PendingColor;
            }
            else if (pendingChar == ' ')
            {
                progressBackground = barBackground = options.PendingColor;
            }

            if (!string.IsNullOrEmpty(caption))
            {
                SetColor(options.CaptionColor, false);
                SetColor(background, true);
                System.Console.Write(caption);
                if (!options.IgnoreCaptionSeparator)
                {
                    #pragma warning disable IDE0056
                    var lastChar = caption[caption.Length - 1];
                    #pragma warning restore IDE0056
                    if (lastChar != ' ' && lastChar != '\r' && lastChar != '\n' && lastChar != '\t') System.Console.Write(' ');
                }
            }

            var progress = new ProgressLineResult();
            var left = 0;
            try
            {
                left = System.Console.CursorLeft;
            }
            catch (IOException)
            {
                progress.IsNotSupported = true;
                System.Console.WriteLine();
                return progress;
            }
            catch (PlatformNotSupportedException)
            {
                progress.IsNotSupported = true;
                System.Console.WriteLine();
                return progress;
            }

            var top = System.Console.CursorTop;
            if (options.Size == ProgressLineOptions.Sizes.Full)
            {
                width = winWidth - left - 6;
                sb.Clear();
                sb.Append(pendingChar, width);
            }

            SetColor(options.PendingColor, false);
            SetColor(progressBackground, true);
            System.Console.Write(sb.ToString());
            SetColor(options.ValueColor ?? fore, false);
            SetColor(background, true);
            System.Console.WriteLine(" ... ");
            SetColor(fore, false);
            SetColor(back, true);
            var locker = new object();
            progress.ProgressChanged += (sender, ev) =>
            {
                var left2 = System.Console.CursorLeft;
                var top2 = System.Console.CursorTop;
                var fore2 = System.Console.ForegroundColor;
                var back2 = System.Console.BackgroundColor;
                if (width > 0)
                {
                    var w = (int)Math.Round(width * ev);
                    var sb2 = new StringBuilder();
                    sb2.Append(barChar, w);
                    var sb3 = new StringBuilder();
                    sb3.Append(pendingChar, width - w);
                    System.Console.CursorLeft = left;
                    System.Console.CursorTop = top;
                    SetColor(progress.IsFailed ? options.ErrorColor : options.BarColor, false);
                    SetColor(progress.IsFailed ? options.ErrorColor : barBackground, true);
                    System.Console.Write(sb2.ToString());
                    SetColor(options.PendingColor, false);
                    SetColor(progressBackground, true);
                    System.Console.Write(sb3.ToString());
                    SetColor(options.ValueColor ?? fore2, false);
                    SetColor(options.BackgroundColor ?? back2, true);
                    System.Console.Write(' ');
                }
                else
                {
                    System.Console.CursorLeft = left;
                    System.Console.CursorTop = top;
                    SetColor(options.ValueColor ?? fore2, false);
                    SetColor(options.BackgroundColor ?? back2, true);
                }

                System.Console.Write(ev.ToString("#0%"));
                System.Console.CursorLeft = left2;
                System.Console.CursorTop = top2;
                SetColor(fore2, false);
                SetColor(back2, true);
            };
            return progress;
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteLine(ConsoleColor foregroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, value, arg);
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes the specified string value, followed by the current line terminator, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="backgroundColor">The background color of the console.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteLine(ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, string value, params object[] arg)
        {
            Write(foregroundColor, backgroundColor, value, arg);
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes a line and an empty line to the standard output stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="arg">An array of objects to write using format.</param>
        public static void WriteDoubleLines(string value, params object[] arg)
        {
            if (string.IsNullOrEmpty(value)) return;
            System.Console.WriteLine(value, arg);
            System.Console.WriteLine();
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(IEnumerable<string> col)
        {
            if (col == null) return;
            foreach (var item in col)
            {
                System.Console.WriteLine(item);
            }
        }

        /// <summary>
        /// Writes the current line terminator for each item, to the standard output stream.
        /// </summary>
        /// <param name="foregroundColor">The foreground color of the console.</param>
        /// <param name="col">The string collection to write. Each one in a line.</param>
        public static void WriteLines(ConsoleColor foregroundColor, IEnumerable<string> col)
        {
            if (col == null) return;
            var c = System.Console.ForegroundColor;
            SetColor(foregroundColor, false);
            foreach (var item in col)
            {
                System.Console.WriteLine(item);
            }

            SetColor(c, false);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<object> Select(SelectionData collection, SelectionOptions options = null)
        {
            return Select<object>(collection, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="convert">The converter.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(IEnumerable<T> collection, Func<T, SelectionItem<T>> convert, SelectionOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection.Select(convert));
            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(IEnumerable<SelectionItem<T>> collection, SelectionOptions options = null)
        {
            var c = new SelectionData<T>();
            c.AddRange(collection);
            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="path">The parent foler path.</param>
        /// <param name="options">The selection display options.</param>
        /// <param name="searchPattern">The search string to match against the names of directories and files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, but it doesn't support regular expressions.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="ArgumentException">searchPattern contains one or more invalid characters defined by the System.IO.Path.GetInvalidPathChars method.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, SelectionOptions options = null, string searchPattern = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            var col = string.IsNullOrEmpty(searchPattern) ? path.GetFileSystemInfos() : path.GetFileSystemInfos(searchPattern);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <param name="path">The parent foler path.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static SelectionResult<FileSystemInfo> Select(DirectoryInfo path, Func<FileSystemInfo, bool> predicate, SelectionOptions options = null)
        {
            var c = new SelectionData<FileSystemInfo>();
            IEnumerable<FileSystemInfo> col = path.GetFileSystemInfos();
            if (predicate != null) col = col.Where(predicate);
            foreach (var f in col)
            {
                c.Add(f.Name, f);
            }

            return Select(c, options);
        }

        /// <summary>
        /// Writes a collection of item for selecting.
        /// </summary>
        /// <typeparam name="T">The type of data.</typeparam>
        /// <param name="collection">The collection data.</param>
        /// <param name="options">The selection display options.</param>
        /// <returns>The result of selection.</returns>
        public static SelectionResult<T> Select<T>(SelectionData<T> collection, SelectionOptions options = null)
        {
            if (collection == null) return null;
            if (options == null) options = new SelectionOptions();
            var maxWidth = 0;
            try
            {
                maxWidth = System.Console.BufferWidth;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }

            if (maxWidth < 1)
            {
                if (string.IsNullOrWhiteSpace(options.QuestionWhenNotSupported)) return new SelectionResult<T>(null, SelectionResultTypes.NotSupported);
                Write(options.QuestionForegroundColor, options.QuestionBackgroundColor, options.QuestionWhenNotSupported);
                var inputStr = System.Console.ReadLine();
                return new SelectionResult<T>(inputStr, SelectionResultTypes.Typed)
                {
                    IsNotSupported = true
                };
            }

            var itemLen = options.Column.HasValue ? (int)Math.Floor(maxWidth * 1.0 / options.Column.Value) : maxWidth;
            if (options.MaxLength.HasValue) itemLen = Math.Min(options.MaxLength.Value, itemLen);
            if (options.MinLength.HasValue) itemLen = Math.Max(options.MinLength.Value, itemLen);
            var columns = (int)Math.Floor(maxWidth * 1.0 / itemLen);
            if (options.Column.HasValue && columns > options.Column.Value) columns = options.Column.Value;
            var itemLen2 = Math.Max(1, itemLen - 1);
            if (System.Console.CursorLeft > 0) System.Console.WriteLine();
            try
            {
                System.Console.CursorTop--;
                System.Console.CursorTop++;
                for (var i = 0; i < 24; i++)
                {
                    System.Console.WriteLine();
                }

                System.Console.CursorTop -= 24;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (PlatformNotSupportedException)
            {
            }

            var top = System.Console.CursorTop;
            var offset = 0;
            var selected = 0;
            var oldSelected = -1;
            var list = collection.CopyList();
            var keys = new Dictionary<char, SelectionItem<T>>();
            foreach (var item in list)
            {
                if (!item.Hotkey.HasValue) continue;
                keys[item.Hotkey.Value] = item;
            }

            list = list.Where(item =>
            {
                return !string.IsNullOrEmpty(item.Title ?? item.Value);
            }).ToList();
            if (list.Count == 0 || itemLen < 1) return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
            var pageSize = options.MaxRow.HasValue ? options.MaxRow.Value * columns : list.Count;
            var prefix = options.Prefix;
            var selectedPrefix = options.SelectedPrefix;
            var tips = options.Tips;
            var tips2 = options.TipsLine2;
            var tipsP = options.PagingTips;
            var question = options.Question;
            var questionM = options.ManualQuestion;
            var fore = options.ForegroundColor;
            var back = options.BackgroundColor;
            var foreSel = options.SelectedForegroundColor;
            var backSel = options.SelectedBackgroundColor;
            var foreQ = options.QuestionForegroundColor;
            var backQ = options.QuestionBackgroundColor;
            var foreTip = options.TipsForegroundColor;
            var backTip = options.TipsBackgroundColor;
            var forePag = options.PagingForegroundColor;
            var backPag = options.PagingBackgroundColor;
            var foreDef = options.DefaultValueForegroundColor;
            var backDef = options.DefaultValueBackgroundColor;
            var inputTop = -1;
            var inputLeft = -1;
            if (!fore.HasValue && !back.HasValue && !foreSel.HasValue && !backSel.HasValue && prefix == null && selectedPrefix == null)
            {
                foreSel = System.Console.BackgroundColor;
                backSel = System.Console.ForegroundColor;
            }

            void change(int selIndex)
            {
                var isSel = selIndex == selected;
                if (selIndex < 0) selIndex = selected;
                var selItem = list[selIndex];
                var k = selIndex - offset;
                var rowI = (int)Math.Floor(k * 1.0 / columns);
                var curLeft = (k % columns) * itemLen;
                var str = ((isSel ? selectedPrefix : prefix) ?? string.Empty) + (selItem.Title ?? selItem.Value);
                if (str.Length > itemLen2) str = str.Substring(0, itemLen2);
                var curLeft2 = curLeft + itemLen2;
                var curLeftDiff = itemLen2;
                SetCursorPosition(curLeft, top + rowI);
                var strOffset = (int)Math.Floor(str.Length * 1.0 / 2);
                if (isSel) Write(foreSel, backSel, str.Substring(0, strOffset));
                else Write(fore, back, str.Substring(0, strOffset));
                for (var i = strOffset; i < str.Length; i++)
                {
                    curLeftDiff = System.Console.CursorLeft - curLeft2;
                    if (curLeftDiff > 0)
                    {
                        Backspace(curLeftDiff);
                        break;
                    }
                    else if (curLeftDiff == 0)
                    {
                        break;
                    }

                    var charactor = str[i];
                    if (isSel) Write(foreSel, backSel, charactor.ToString());
                    else Write(fore, back, charactor.ToString());
                }

                curLeftDiff = System.Console.CursorLeft - curLeft2;
                if (curLeftDiff < 0)
                {
                    var spaces = new StringBuilder();
                    spaces.Append(' ', -curLeftDiff);
                    if (isSel) Write(foreSel, backSel, spaces.ToString());
                    else Write(fore, back, spaces.ToString());
                }

                if (inputTop >= 0) SetCursorPosition(inputLeft, inputTop);
            }

            void select()
            {
                if (oldSelected != selected)
                {
                    if (selected < 0) selected = 0;
                    else if (selected >= list.Count) selected = list.Count - 1;
                    if (selected < offset || selected >= offset + pageSize || inputTop < 0)
                    {
                        render();
                        return;
                    }

                    if (oldSelected >= 0 && oldSelected >= offset && oldSelected < offset + pageSize) change(oldSelected);
                    change(selected);
                }

                SetCursorPosition(inputLeft, inputTop);
                var sel = list[selected];
                if (question != null) Write(foreDef, backDef, sel.Value);
                oldSelected = selected;
            }

            void render()
            {
                if (selected < 0) selected = 0;
                else if (selected >= list.Count) selected = list.Count - 1;
                if (selected < offset || selected >= offset + pageSize) offset = (int)Math.Floor(selected * 1.0 / pageSize) * pageSize;
                var k = 0;
                SetCursorPosition(0, top);
                ClearLine();
                SelectionData.Some(list, (item, i, j) =>
                {
                    if (j >= pageSize) return true;
                    k = j;
                    var str = ((selected == i ? selectedPrefix : prefix) ?? string.Empty) + (item.Title ?? item.Value);
                    var index = j % columns;
                    var row = (int)Math.Floor(j * 1.0 / columns);
                    if (str.Length > itemLen2) str = str.Substring(0, itemLen2);
                    var curLeft = index * itemLen;
                    var curLeft2 = curLeft + itemLen2;
                    var curTop = top + row;
                    SetCursorPosition(curLeft, curTop);
                    if (selected == i) Write(foreSel, backSel, str);
                    else Write(fore, back, str);
                    if (System.Console.CursorTop != curTop)
                    {
                        Backspace(System.Console.CursorLeft);
                        SetCursorPosition(maxWidth - 1, curTop);
                    }

                    if (System.Console.CursorLeft > curLeft2)
                    {
                        Backspace(System.Console.CursorLeft - curLeft2);
                    }
                    else if (System.Console.CursorLeft < curLeft2)
                    {
                        var spaces = new StringBuilder();
                        spaces.Append(' ', curLeft2 - System.Console.CursorLeft);
                        if (selected == i) Write(foreSel, backSel, spaces.ToString());
                        else Write(fore, back, spaces.ToString());
                    }

                    if (index == columns - 1)
                    {
                        System.Console.WriteLine();
                        ClearLine();
                    }

                    return false;
                }, offset);
                if ((k + 1) % columns > 0)
                {
                    System.Console.WriteLine();
                    ClearLine();
                }

                if (tipsP != null && pageSize < list.Count)
                {
                    WriteLine(fore, back, tipsP
                        .Replace("{from}", (offset + 1).ToString())
                        .Replace("{end}", (offset + k + 1).ToString())
                        .Replace("{count}", k.ToString())
                        .Replace("{size}", pageSize.ToString())
                        .Replace("{total}", list.Count.ToString()));
                    ClearLine();
                }

                if (inputTop > 0)
                {
                    var curTop = System.Console.CursorTop;
                    for (var i = inputTop; i >= curTop; i--)
                    {
                        ClearLine(i);
                    }
                }

                if (question != null) Write(foreQ, backQ, question);
                inputTop = System.Console.CursorTop;
                inputLeft = System.Console.CursorLeft;
                var sel = list[selected];
                if (question != null) Write(foreDef, backDef, sel.Value);
                oldSelected = selected;
            }

            var tipsTop = -1;
            var tipsTop2 = -1;
            void showTips()
            {
                if (string.IsNullOrWhiteSpace(tips)) return;
                var curTop = System.Console.CursorTop;
                var curLeft = System.Console.CursorLeft;
                System.Console.WriteLine();
                System.Console.WriteLine();
                tipsTop = System.Console.CursorTop;
                if (string.IsNullOrWhiteSpace(tips2))
                {
                    Write(foreTip, backTip, tips);
                }
                else
                {
                    WriteLine(foreTip, backTip, tips);
                    Write(foreTip, backTip, tips2);
                }

                tipsTop2 = Math.Max(tipsTop, System.Console.CursorTop);
                SetCursorPosition(curLeft, curTop);
            }

            render();
            showTips();

            while (true)
            {
                var key = System.Console.ReadKey();
                if (inputTop > 0 && System.Console.CursorTop >= inputTop)
                {
                    for (var i = System.Console.CursorTop; i > inputTop; i--)
                    {
                        ClearLine(i);
                    }

                    if (inputLeft >= 0 && System.Console.CursorLeft != inputLeft)
                    {
                        SetCursorPosition(maxWidth - 1, inputTop);
                        Backspace(System.Console.CursorLeft - inputLeft);
                    }
                }

                if (tipsTop > 0)
                {
                    for (var i = tipsTop; i <= tipsTop2; i++)
                    {
                        ClearLine(i);
                    }

                    tipsTop = -1;
                }

                if (inputTop > 0) SetCursorPosition(inputLeft, inputTop);
                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Select)
                {
                    var sel = list[selected];
                    if (question != null) System.Console.WriteLine(sel.Value);
                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else if (key.Key == ConsoleKey.Backspace || key.Key == ConsoleKey.Delete || key.Key == ConsoleKey.Clear)
                {
                    if (questionM == null)
                    {
                        change(-1);
                        if (question != null) System.Console.WriteLine();
                        return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
                    }

                    change(-1);
                    Backspace(System.Console.CursorLeft);
                    Write(foreQ, backQ, questionM);
                    var inputStr = System.Console.ReadLine();
                    return new SelectionResult<T>(inputStr, SelectionResultTypes.Typed);
                }
                else if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Pause)
                {
                    change(-1);
                    if (question != null) System.Console.WriteLine();
                    return new SelectionResult<T>(string.Empty, SelectionResultTypes.Canceled);
                }
                else if (key.Key == ConsoleKey.F1)
                {
                    if (keys.ContainsKey('?'))
                    {
                        change(-1);
                        var sel = keys['?'];
                        if (question != null) System.Console.WriteLine(sel.Value);
                        return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                    }

                    showTips();
                    select();
                }
                else if (key.Key == ConsoleKey.F12)
                {
                    showTips();
                    select();
                }
                else if (key.Key == ConsoleKey.F5)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = 0;
                    }

                    render();
                }
                else if (key.Key == ConsoleKey.PageUp)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        var selLeft = selected % columns;
                        selected = offset + selLeft;
                        select();
                        continue;
                    }

                    if (offset < 1)
                    {
                        select();
                        continue;
                    }

                    offset -= pageSize;
                    if (offset < 0) offset = 0;
                    selected = offset;
                    render();
                }
                else if (key.Key == ConsoleKey.PageDown)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        var selLeft = selected % columns;
                        var sel2 = offset + pageSize - columns + selLeft;
                        while (sel2 > list.Count)
                        {
                            sel2 -= columns;
                        }

                        if (sel2 < offset) sel2 = offset;
                        selected = sel2;
                        select();
                        continue;
                    }

                    if (offset + pageSize >= list.Count)
                    {
                        select();
                        continue;
                    }

                    offset += pageSize;
                    selected = offset;
                    render();
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (selected - columns >= 0) selected -= columns;
                    select();
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (selected + columns < list.Count) selected += columns;
                    select();
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (selected - 1 >= 0) selected--;
                    select();
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    if (selected + 1 < list.Count) selected++;
                    select();
                }
                else if (key.Key == ConsoleKey.Home)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = 0;
                    }
                    else
                    {
                        var rowI = (int)Math.Floor(selected * 1.0 / columns);
                        selected = rowI * columns;
                    }

                    select();
                }
                else if (key.Key == ConsoleKey.End)
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        selected = list.Count - 1;
                    }
                    else
                    {
                        var rowI = (int)Math.Floor(selected * 1.0 / columns);
                        var toSel = (rowI + 1) * columns - 1;
                        if (toSel < list.Count) selected = toSel;
                        else selected = list.Count - 1;
                    }

                    select();
                }
                else if (keys.ContainsKey(key.KeyChar))
                {
                    var sel = keys[key.KeyChar];
                    var isUnselected = true;
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i] != sel) continue;
                        isUnselected = false;
                        selected = i;
                        select();
                        if (question != null) System.Console.WriteLine();
                        break;
                    }

                    if (isUnselected)
                    {
                        change(-1);
                        if (question != null) System.Console.WriteLine(sel.Value);
                    }

                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else if (key.Key == ConsoleKey.Spacebar)
                {
                    var sel = list[selected];
                    if (question != null) System.Console.WriteLine(sel.Value);
                    return new SelectionResult<T>(sel.Value, selected, sel.Data, sel.Title);
                }
                else
                {
                    select();
                }
            }
        }

        /// <summary>
        /// Converts a secure string to unsecure string.
        /// </summary>
        /// <param name="value">The secure string to convert.</param>
        /// <returns>The unsecure string.</returns>
        internal static string ToUnsecureString(SecureString value)
        {
            if (value == null) return null;
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        internal static void SetColor(ConsoleColor color, bool background)
        {
            try
            {
                if (background) System.Console.BackgroundColor = color;
                else System.Console.ForegroundColor = color;
            }
            catch (IOException)
            {
            }
            catch (SecurityException)
            {
            }
        }

        internal static void SetColor(ConsoleColor? color, bool background)
        {
            if (color.HasValue) SetColor(color.Value, background);
        }

        private static void SetCursorPosition(int left, int top)
        {
            try
            {
                System.Console.SetCursorPosition(left, top);
            }
            catch (ArgumentOutOfRangeException)
            {
                if (System.Console.BufferHeight > top) throw;
#pragma warning disable CA1416
                try
                {
                    System.Console.BufferHeight = top + 64;
                }
                catch (OverflowException)
                {
                    System.Console.BufferHeight = int.MaxValue;
                }
                catch (PlatformNotSupportedException)
                {
                }
                catch (IOException)
                {
                }
                catch (SecurityException)
                {
                }
#pragma warning restore CA1416

                System.Console.SetCursorPosition(left, top);
            }
        }
    }
}
