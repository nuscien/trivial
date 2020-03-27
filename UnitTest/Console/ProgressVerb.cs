using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SysConsole = System.Console;
using TrivialConsole = Trivial.Console;

namespace Trivial.Console
{
    class ProgressVerb : AsyncVerb
    {
        public override string Description => "Progress";

        public override async Task ProcessAsync()
        {
            var progress = LineUtilities.WriteLine("Running 1: ", new ProgressLineOptions
            {
                Style = ProgressLineOptions.Styles.AngleBracket,
                BarColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkBlue
            });
            SysConsole.WriteLine("And another one...");
            var progress2 = ConsoleLine.WriteLine("Running 2: ", new ProgressLineOptions());
            for (var i = 0; i <= 50; i++)
            {
                await Task.Delay(10);
                progress.Report(0.02 * i);
                progress2.Report(0.03 * i);
                ConsoleLine.Clear();
                ConsoleLine.Write("Update to {0} (total 50).", i);
            }

            ConsoleLine.Write(" And following is failed.");
            ConsoleLine.End();
            progress = LineUtilities.WriteLine("Running 3: ", new ProgressLineOptions());
            await progress.IncreaseAsync(null, 0.02, 0.6, 10);
            progress.Fail();
            SysConsole.WriteLine();
        }
    }
}
