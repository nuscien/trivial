using System;
using System.Collections.Generic;
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
        public static string Description => "Selection";

        /// <inheritdoc />
        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            await RunAsync(null, cancellationToken);
            var cli = StyleConsole.Default;
            var col = new SelectionData();
            col.Add('a', "aaa", null);
            col.Add('b', "bbb");
            col.Add('p', "password");
            col.Add('l', "progress");
            col.Add('j', "json");
            col.Add('e', "error");
            col.Add('?', "help", null);
            for (var i = 0; i < 120; i++)
            {
                col.Add(i.ToString());
            }

            var result = cli.Select(col, new SelectionConsoleOptions
            {
                SelectedBackgroundColor = ConsoleColor.Cyan,
                SelectedForegroundColor = ConsoleColor.Black,
                TipsForegroundColor = ConsoleColor.Yellow,
                SelectedPrefix = "> ",
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

            cli.WriteLine("-> {0}", result.Value);
            switch (result.Title ?? result.Value)
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
                case "?":
                    cli.WriteLine("This is a sample.");
                    return;
            }
        }

        public static async Task ShowProgressAsync()
        {
            var cli = StyleConsole.Default;
            var progress = cli.WriteLine(new ConsoleProgressStyle
            {
                Kind = ConsoleProgressStyle.Kinds.AngleBracket,
                BarColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkBlue
            }, "Running 1:");
            Tasks.OneProgress progress2 = null;
            cli.WriteLine("And another one...");
            progress2 = cli.WriteLine(new ConsoleProgressStyle(), "Running 2:");
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
    }
}
