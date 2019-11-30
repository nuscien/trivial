using System;
using System.Collections.Generic;
using System.Text;
using SysConsole = System.Console;
using TrivialConsole = Trivial.Console;

namespace Trivial.UnitTest.Console
{
    class SelectionVerb : TrivialConsole.Verb
    {
        public override string Description => "Selection";

        public override void Process()
        {
            var col = new TrivialConsole.SelectionData();
            col.Add('a', "aaa", null, "first");
            col.Add('b', "bbb");
            col.Add('?', "help", null, string.Empty);
            for (var i = 0; i < 120; i++)
            {
                col.Add(i.ToString());
            }

            var result = TrivialConsole.LineUtilities.Select(col, new TrivialConsole.SelectionOptions
            {
                Column = 5,
                MaxRow = 10,
                ManualQuestion = "Type: ",
                TipsLine2 = "Hey..."
            });
            SysConsole.WriteLine("-> {0}", result.Value);
        }
    }
}
