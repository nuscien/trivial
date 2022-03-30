using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.CommandLine;

namespace Trivial.Data;

/// <summary>
/// The International Article Number, a.k.a. European Article Number or EAN.
/// </summary>
public partial class InternationalArticleNumber
{
    /// <summary>
    /// Converts to boolean list and writes to the standard output stream.
    /// White represented as false, black represented as true.
    /// </summary>
    /// <param name="cli">The command line interface proxy.</param>
    /// <param name="style">The style that foreground represents black and background represents white.</param>
    /// <returns>The boolean list.</returns>
    /// <exception cref="InvalidOperationException">It was not an EAN-13 ro EAN-8 code.</exception>
    public List<bool> ToBarcode(StyleConsole cli, ConsoleTextStyle style = null)
    {
        List<bool> barcode;
        try
        {
            barcode = ToBarcode();
        }
        catch (InvalidOperationException)
        {
            cli.WriteLine(style, value);
            throw;
        }

        var col = new List<ConsoleText>();
        if (style == null) style = new ConsoleTextStyle(System.Drawing.Color.FromArgb(16, 16, 16), ConsoleColor.Black, System.Drawing.Color.FromArgb(206, 206, 206), ConsoleColor.Gray);
        var black = new ConsoleTextStyle(style.ForegroundRgbColor, style.ForegroundConsoleColor, style.ForegroundRgbColor, style.ForegroundConsoleColor);
        var white = new ConsoleTextStyle(style.BackgroundRgbColor, style.BackgroundConsoleColor, style.BackgroundRgbColor, style.BackgroundConsoleColor);
        var bg = new string(' ', 12 + barcode.Count);
        col.Add(bg, white);
        col.Add(Environment.NewLine);
        var s = value;
        cli.Flush();
        if (cli.Mode == StyleConsole.Modes.Text && cli.Handler == null)
        {
            cli.WriteLine(style, s);
            return barcode;
        }
        else if (barcode.Count + 12 > GetBufferSafeWidth(cli))
        {
            col.Clear();
            foreach (var b in barcode)
            {
                col.Add(' ', 1, b ? black : white);
            }

            col.Add(Environment.NewLine);
            col.Add(s, style);
            cli.WriteLine(col);
            return barcode;
        }
        else if (barcode.Count == 95 && s.Length == 13)
        {
            for (var i = 0; i < 4; i++)
            {
                col.Add(' ', 7, white);
                foreach (var b in barcode)
                {
                    col.Add(' ', 1, b ? black : white);
                }

                col.Add(' ', 5, white);
                col.Add(Environment.NewLine);
            }

            var isbn = false;
            if (s.StartsWith("97"))
            {
                if (s.StartsWith("9790"))
                {
                    isbn = true;
                    col.Add("ISMN 9 ", style);
                }
                else if (s.StartsWith("978") || s.StartsWith("979"))
                {
                    isbn = true;
                    col.Add("ISBN 9 ", style);
                }
                else if (s.StartsWith("977"))
                {
                    isbn = true;
                    col.Add("ISSN 9 ", style);
                }
            }
            
            if (!isbn)
            {
                col.Add(' ', 4, white);
                col.Add(s[0], 1, style);
                col.Add(' ', 2, white);
            }

            for (var i = 0; i < 3; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            for (var i = 1; i < 7; i++)
            {
                col.Add(' ', 3, white);
                col.Add(s[i], 1, style);
                col.Add(' ', 3, white);
            }

            for (var i = 45; i < 50; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            for (var i = 7; i < 13; i++)
            {
                col.Add(' ', 3, white);
                col.Add(s[i], 1, style);
                col.Add(' ', 3, white);
            }

            for (var i = 92; i < 95; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            col.Add(' ', 5, white);
            col.Add(Environment.NewLine);
        }
        else if (barcode.Count == 67 && s.Length == 8)
        {
            for (var i = 0; i < 4; i++)
            {
                col.Add(' ', 6, white);
                foreach (var b in barcode)
                {
                    col.Add(' ', 1, b ? black : white);
                }

                col.Add(' ', 6, white);
                col.Add(Environment.NewLine);
            }

            col.Add(' ', 6, white);
            for (var i = 0; i < 3; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            for (var i = 0; i < 4; i++)
            {
                col.Add(' ', 3, white);
                col.Add(s[i], 1, style);
                col.Add(' ', 3, white);
            }

            for (var i = 31; i < 36; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            for (var i = 4; i < 8; i++)
            {
                col.Add(' ', 3, white);
                col.Add(s[i], 1, style);
                col.Add(' ', 3, white);
            }

            for (var i = 64; i < 67; i++)
            {
                col.Add(' ', 1, barcode[i] ? black : white);
            }

            col.Add(' ', 6, white);
            col.Add(Environment.NewLine);
        }
        else if ((barcode.Count == 48 && s.Length == 5) || (barcode.Count == 21 && s.Length == 2))
        {
            col.Add(' ', 14, white);
            foreach (var c in s)
            {
                col.Add(c, 1, style);
                col.Add(' ', 8, white);
            }

            col.Add(' ', 1, white);
            col.Add(Environment.NewLine);
            for (var i = 0; i < 4; i++)
            {
                col.Add(' ', 6, white);
                foreach (var b in barcode)
                {
                    col.Add(' ', 1, b ? black : white);
                }

                col.Add(' ', 6, white);
                col.Add(Environment.NewLine);
            }
        }
        else
        {
            for (var i = 0; i < 4; i++)
            {
                col.Add(' ', 6, white);
                foreach (var b in barcode)
                {
                    col.Add(' ', 1, b ? black : white);
                }

                col.Add(' ', 6, white);
                col.Add(Environment.NewLine);
            }
        }

        col.Add(bg, white);
        (cli ?? StyleConsole.Default).WriteLine(col);
        return barcode;
    }

    private static int GetBufferSafeWidth(StyleConsole cli)
    {
        try
        {
            return (cli ?? StyleConsole.Default).BufferWidth - 1;
        }
        catch (System.IO.IOException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (System.Security.SecurityException)
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
