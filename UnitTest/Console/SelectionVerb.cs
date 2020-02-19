﻿using System;
using System.Collections.Generic;
using System.Text;
using SysConsole = System.Console;
using TrivialConsole = Trivial.Console;

namespace Trivial.Console
{
    class SelectionVerb : Verb
    {
        public override string Description => "Selection";

        public override void Process()
        {
            var col = new SelectionData();
            col.Add('a', "aaa", null, "first");
            col.Add('b', "bbb");
            col.Add('p', "password");
            col.Add('?', "help", null, string.Empty);
            for (var i = 0; i < 120; i++)
            {
                col.Add(i.ToString());
            }

            var result = LineUtilities.Select(col, new SelectionOptions
            {
                Column = 5,
                MaxRow = 10,
                ManualQuestion = "Type: ",
                TipsLine2 = "Hey..."
            });
            if (result.IsCanceled)
            {
                SysConsole.WriteLine("The operation is cancelled.");
                return;
            }

            SysConsole.WriteLine("-> {0}", result.Value);
            switch (result.Value)
            {
                case "aaa":
                case "bbb":
                    SysConsole.WriteLine(result.Title + '\t' + result.Value + '\t');
                    return;
                case "password":
                    var l = new Line();
                    var p = l.ReadPassword('*');
                    l.WriteLine();
                    l.WriteLine(p);
                    return;
                case "?":
                    SysConsole.WriteLine("This is a sample.");
                    return;
            }
        }
    }
}
