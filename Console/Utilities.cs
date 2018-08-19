using System;
using System.Collections.Generic;
using System.Text;

namespace Trivial.Console
{
    public static class Utilities
    {
        public static void Backspace(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                System.Console.Write('\u0008');
            }
        }

        public static HelpVerb RegisterHelp(this Dispatcher dispatcher, string furtherDesc = null)
        {
            var verb = new HelpVerb
            {
                FurtherDescription = furtherDesc
            };
            dispatcher.Register(new[] { "help", "?", "gethelp", "get-help" }, verb);
            return verb;
        }

        public static ExitVerb RegisterExit(this Dispatcher dispatcher, bool exitApp = false)
        {
            var verb = new ExitVerb
            {
                ExitApp = exitApp
            };
            dispatcher.Register(new[] { "exit", "quit", "bye", "goodbye" }, verb);
            return verb;
        }

    }
}
