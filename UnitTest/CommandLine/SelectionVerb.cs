using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Collection;
using Trivial.Text;

namespace Trivial.CommandLine
{
    class SelectionVerb : BaseCommandVerb
    {
        private static List<Color> blendColors;
        private static List<Color> baseColors;
        private static List<Color> saturateColors;

        public static string Description => "Selection";

        /// <inheritdoc />
        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            await RunAsync(null, cancellationToken);
            var cli = StyleConsole.Default;
            var col = new SelectionData();
            col.Add('a', "Aaa", null);
            col.Add('b', "Bbb 二号");
            col.Add('p', "Password");
            col.Add('l', "Progress");
            col.Add('j', "JSON");
            col.Add('e', "Error");
            col.Add('?', "Help", null);
            for (var i = 0; i < 16; i++)
            {
                var mix = (Drawing.ColorMixTypes)i;
                col.Add(mix.ToString(), mix);
            }

            col.Add("Saturate");
            col.Add("Brightness");
            for (var i = 0; i < 120; i++)
            {
                col.Add(i.ToString());
            }

            var result = cli.Select(col, new SelectionConsoleOptions
            {
                SelectedBackgroundConsoleColor = ConsoleColor.Cyan,
                SelectedForegroundConsoleColor = ConsoleColor.Black,
                TipsForegroundConsoleColor = ConsoleColor.Yellow,
                SelectedPrefix = "→ ",
                Column = 5,
                MaxRow = 10,
                ManualQuestion = "Type: "
            });
            if (result.IsCanceled)
            {
                cli.WriteLine("The operation is cancelled.");
                return;
            }

            if (result.IsNotSupported)
            {
                cli.WriteLine("The selection control is not supported.");
                return;
            }

            cli.WriteLine("-> {0}\t{1}", result.Value, result.InputType);
            switch ((result.Title ?? result.Value)?.ToLowerInvariant())
            {
                case "aaa":
                case "bbb":
                    cli.WriteLine(result.Value + '\t' + result.Title + '\t' + result.InputType);
                    return;
                case "password":
                    cli.Write("Type password: ");
                    var p = cli.ReadPassword(ConsoleColor.Yellow, '*');
                    cli.Write("Your password is ");
                    cli.Write(ConsoleColor.Magenta, Security.SecureStringExtensions.ToUnsecureString(p));
                    cli.WriteLine('.');
                    return;
                case "progress":
                    await ShowProgressAsync();
                    return;
                case "json":
                    cli.WriteLine(JsonUnitTest.CreateModel(DateTime.Now));
                    return;
                case "error":
                    cli.WriteLine(new InvalidOperationException("Test", new NotSupportedException()));
                    break;
                case "saturate":
                    InitColors();
                    for (var i = 0; i < 7; i++)
                    {
                        var saturate = (Drawing.RelativeSaturationLevels)i;
                        foreach (var item in saturateColors)
                        {
                            cli.Write(Drawing.ColorCalculator.Saturate(item, saturate), "■");
                        }

                        cli.WriteLine(saturate.ToString());
                    }

                    foreach (var item in saturateColors)
                    {
                        cli.Write(Drawing.ColorCalculator.ToggleBrightness(item), "■");
                    }

                    cli.WriteLine("Light-Dark");
                    foreach (var item in saturateColors)
                    {
                        cli.Write(Drawing.ColorCalculator.Reverse(item), "■");
                    }

                    cli.WriteLine("Reverse");
                    break;
                case "brightness":
                    InitColors();
                    for (var i = -4; i < 5; i++)
                    {
                        var saturate = (Drawing.RelativeSaturationLevels)i;
                        foreach (var item in saturateColors)
                        {
                            cli.Write(Drawing.ColorCalculator.Saturate(item, saturate), "■");
                        }

                        cli.WriteLine(saturate.ToString());
                    }

                    foreach (var item in saturateColors)
                    {
                        cli.Write(Drawing.ColorCalculator.ToggleBrightness(item), "■");
                    }

                    cli.WriteLine("Light-Dark");
                    foreach (var item in saturateColors)
                    {
                        cli.Write(Drawing.ColorCalculator.Reverse(item), "■");
                    }

                    cli.WriteLine("Reverse");
                    break;
                case "help":
                case "?":
                    cli.WriteLine("This is a sample.");
                    return;
            }

            if (result.Data is Drawing.ColorMixTypes mixType)
            {
                InitColors();
                cli.Write("Blend");
                foreach (var item in blendColors)
                {
                    cli.Write(item, "■");
                }

                cli.WriteLine();
                cli.Write("Base ");
                foreach (var item in baseColors)
                {
                    cli.Write(item, "■");
                }

                cli.WriteLine();
                cli.Write("Mixed");
                var colors = Drawing.ColorCalculator.Mix(mixType, blendColors, baseColors);
                foreach (var item in colors)
                {
                    cli.Write(item, "■");
                }
            }
        }

        public static async Task ShowProgressAsync()
        {
            var cli = StyleConsole.Default;
            var progress = cli.WriteLine(new ConsoleProgressStyle
            {
                Kind = ConsoleProgressStyle.Kinds.AngleBracket,
                BarConsoleColor = ConsoleColor.White,
                BackgroundConsoleColor = ConsoleColor.DarkBlue
            }, "Running 1:");
            cli.WriteLine("And another one...");
            var progress2 = cli.WriteLine(new ConsoleProgressStyle(), "Running 2:");
            for (var i = 0; i <= 50; i++)
            {
                await Task.Delay(10);
                progress.Report(0.02 * i);
                progress2.Report(0.03 * i);
                if (cli.Mode == StyleConsole.Modes.Text) continue;
                cli.BackspaceToBeginning();
                cli.WriteImmediately("Update to {0} (total 50).", i);
            }

            cli.WriteLine(" And following is failed.");
            progress = cli.WriteLine(new ConsoleProgressStyle(), "Running 3:");
            await progress.IncreaseAsync(null, 0.02, 0.6, 10);
            progress.Fail();
        }

        private static void InitColors()
        {
            if (blendColors == null)
                blendColors = new List<Color>
                {
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x80, 0x80, 0x80),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(0x80, 0x80, 0x80),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                };
            if (baseColors == null)
                baseColors = new List<Color>
                {
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(0x80, 0x80, 0x80),
                    Color.FromArgb(0, 0, 0),
                    Color.FromArgb(0x80, 0x80, 0x80),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                };
            if (saturateColors == null)
                saturateColors = new List<Color>
                {
                    Color.FromArgb(0xFF, 0, 0),
                    Color.FromArgb(0xFF, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0),
                    Color.FromArgb(0, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0xFF),
                    Color.FromArgb(0xFF, 0, 0xFF),
                    Color.FromArgb(0x99, 0x66, 0x66),
                    Color.FromArgb(0x99, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x99),
                    Color.FromArgb(0x66, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x99),
                    Color.FromArgb(0x99, 0x66, 0x33),
                    Color.FromArgb(0x33, 0x99, 0x66),
                    Color.FromArgb(0x66, 0x33, 0x99),
                    Color.FromArgb(0x99, 0x33, 0x66),
                    Color.FromArgb(0x66, 0x99, 0x33),
                    Color.FromArgb(0x33, 0x66, 0x99),
                    Color.FromArgb(0xFF, 0x99, 0x99),
                    Color.FromArgb(0x99, 0xFF, 0x99),
                    Color.FromArgb(0x99, 0x99, 0xFF),
                    Color.FromArgb(0x66, 0, 0),
                    Color.FromArgb(0, 0x66, 0),
                    Color.FromArgb(0, 0, 0x66),
                    Color.FromArgb(0x80, 0x80, 0x80),
                    Color.FromArgb(0xFF, 0xFF, 0xFF),
                    Color.FromArgb(0, 0, 0)
                };
        }
    }
}
