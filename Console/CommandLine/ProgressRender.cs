using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security;

using Trivial.Collection;
using Trivial.Tasks;

namespace Trivial.CommandLine;

/// <summary>
/// The extensions for console renderer.
/// </summary>
public static partial class ConsoleRenderExtensions
{
    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="style">The options.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(this StyleConsole cli, ConsoleProgressStyle style)
        => WriteLine(cli, style, null);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
    /// <param name="progressSize">The progress size.</param>
    /// <param name="kind">The progress kind.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(this StyleConsole cli, ConsoleProgressStyle.Sizes progressSize, string caption, ConsoleProgressStyle.Kinds kind = ConsoleProgressStyle.Kinds.Full)
        => WriteLine(cli, progressSize != ConsoleProgressStyle.Sizes.None ? new ConsoleProgressStyle
        {
            Size = progressSize,
            Kind = kind
        } : null, caption);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="progressSize">The progress size.</param>
    /// <param name="kind">The progress kind.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(this StyleConsole cli, ConsoleProgressStyle.Sizes progressSize, ConsoleProgressStyle.Kinds kind = ConsoleProgressStyle.Kinds.Full)
        => WriteLine(cli, progressSize != ConsoleProgressStyle.Sizes.None ? new ConsoleProgressStyle
        {
            Size = progressSize,
            Kind = kind
        } : null, null);

    /// <summary>
    /// Writes a progress component, followed by the current line terminator, to the standard output stream.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="caption">The caption; or null if no caption. It will be better if it is less than 20 characters.</param>
    /// <param name="style">The style.</param>
    /// <returns>The progress result.</returns>
    public static OneProgress WriteLine(this StyleConsole cli, ConsoleProgressStyle style, string caption)
    {
        if (cli == null) cli = StyleConsole.Default;
        if (cli.Mode == StyleConsole.Modes.Text && cli.Handler == null)
        {
            var progress2 = new OneProgress();
            if (string.IsNullOrWhiteSpace(caption))
            {
                cli.WriteLine(Resource.Loading);
                progress2.ProgressChanged += (sender, ev) =>
                {
                    if (progress2.IsFailed || progress2.IsNotSupported)
                        cli.WriteLine($"{ev:#0%}  {Resource.Error}");
                    else if (progress2.IsCompleted)
                        cli.WriteLine($"√");
                };
            }
            else
            {
                cli.WriteLine($"{caption}  \t{Resource.Loading}");
                progress2.ProgressChanged += (sender, ev) =>
                {
                    if (progress2.IsFailed || progress2.IsNotSupported)
                        cli.WriteLine($"{caption}  \t{ev:#0%}  {Resource.Error}");
                    else if (progress2.IsCompleted)
                        cli.WriteLine($"{caption}  \t√");
                };
            }

            return progress2;
        }

        if (style == null) style = new ConsoleProgressStyle();
        var status = RenderData(cli, style, caption, null, null);
        var progress = new OneProgress();
        var top = TryGetCursorTop(cli) ?? -1;
        progress.ProgressChanged += (sender, ev) =>
        {
            var top2 = TryGetCursorTop(cli) ?? -1;
            var left2 = TryGetCursorLeft(cli) ?? 0;
            cli.Flush();
            if (cli.Mode == StyleConsole.Modes.Cmd && top >= 0 && top2 > top)
                cli.MoveCursorBy(0, top - top2 - 1);
            else
                cli.MoveCursorBy(0, -1);
            status = RenderData(cli, style, caption, progress, status);
            if (cli.Mode == StyleConsole.Modes.Cmd && top >= 0 && top2 > top)
                cli.MoveCursorTo(left2, top2);
        };
        return progress;
    }

    private static string RenderData(StyleConsole cli, ConsoleProgressStyle style, string caption, OneProgress value, string status)
    {
        var maxWidth = GetBufferSafeWidth(cli);
        var width = style.Size switch
        {
            ConsoleProgressStyle.Sizes.None => 0,
            ConsoleProgressStyle.Sizes.Short => maxWidth > 70 ? 20 : 10,
            ConsoleProgressStyle.Sizes.Wide => maxWidth > 88 ? 60 : 40,
            ConsoleProgressStyle.Sizes.Full => maxWidth - 5,
            _ => maxWidth > 70 ? (maxWidth > 88 ? 40 : 30) : 20
        };
        var barChar = style.Kind switch
        {
            ConsoleProgressStyle.Kinds.AngleBracket => '>',
            ConsoleProgressStyle.Kinds.Plus => '+',
            ConsoleProgressStyle.Kinds.Sharp => '#',
            ConsoleProgressStyle.Kinds.X => 'x',
            ConsoleProgressStyle.Kinds.O => 'o',
            _ => ' ',
        };
        var pendingChar = style.Kind switch
        {
            ConsoleProgressStyle.Kinds.AngleBracket => '=',
            ConsoleProgressStyle.Kinds.Plus => '-',
            ConsoleProgressStyle.Kinds.Sharp => '-',
            ConsoleProgressStyle.Kinds.X => '.',
            ConsoleProgressStyle.Kinds.O => '.',
            _ => ' ',
        };
        var col = new List<ConsoleText>();
        if (!string.IsNullOrWhiteSpace(caption))
        {
            var sb = new StringBuilder();
            var j = style.IgnoreCaptionSeparator ? 0 : 1;
            foreach (var c in caption)
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

                sb.Append(c2);
            }

            if (!style.IgnoreCaptionSeparator) sb.Append(' ');
            col.Add(sb, new ConsoleTextStyle(style.CaptionRgbColor, style.CaptionConsoleColor, style.BackgroundRgbColor, style.BackgroundConsoleColor));
            if (style.Size == ConsoleProgressStyle.Sizes.Full)
                width -= j;
        }

        var v = value?.Value ?? -1;
        if (v > 1) v = 1;
        if (double.IsNaN(v))
        {
            cli.WriteLine(col);
            return null;
        }

        var w = (int)Math.Round(width * v);
        if (w < 0) w = 0;
        var isError = value?.IsFailed == true || value?.IsNotSupported == true;
        var isSucc = !isError && value?.IsSuccessful == true;
        var newStatus = $"{(isError ? "e" : (isSucc ? "s" : "p"))}{w}/{maxWidth}";
        if (status == newStatus)
        {
            cli.Flush();
            cli.MoveCursorBy(0, 1);
            return status;
        }

        if (barChar == ' ')
        {
            col.Add(barChar, w, new ConsoleTextStyle(null, null, isError ? style.ErrorRgbColor : style.BarRgbColor, isError ? style.ErrorConsoleColor : style.BarConsoleColor));
            col.Add(pendingChar, width - w, new ConsoleTextStyle(null, null, style.PendingRgbColor, style.PendingConsoleColor));
        }
        else
        {
            col.Add(barChar, w, new ConsoleTextStyle(isError ? style.ErrorRgbColor : style.BarRgbColor, isError ? style.ErrorConsoleColor : style.BarConsoleColor, style.BackgroundRgbColor, style.BackgroundConsoleColor));
            col.Add(pendingChar, width - w, new ConsoleTextStyle(style.PendingRgbColor, style.PendingConsoleColor, style.BackgroundRgbColor, style.BackgroundConsoleColor));
        }

        if (v >= 0)
        {
            var s = v.ToString("#0%");
            if (s.Length > 3) s = isSucc ? " √" : "99%";
            col.Add(" " + s, new ConsoleTextStyle(style.ValueRgbColor, style.ValueConsoleColor, style.BackgroundRgbColor, style.BackgroundConsoleColor));
        }

        cli.Flush();
        cli.Clear(StyleConsole.RelativeAreas.Line);
        cli.BackspaceToBeginning();
        cli.WriteLine(col);
        return status;
    }
}
