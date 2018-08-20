using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Trivial.Console
{
    /// <summary>
    /// The utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Enters a backspace to console to remove the last charactor.
        /// </summary>
        /// <param name="count">The count of the charactor to remove from end.</param>
        public static void Backspace(int count = 1)
        {
            for (var i = 0; i < count; i++)
            {
                System.Console.Write('\u0008');
            }
        }

        /// <summary>
        /// Registers a help verb.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="furtherDesc">Additional description which will be appended to the last.</param>
        /// <returns>The help verb instance.</returns>
        public static void RegisterHelp(this Dispatcher dispatcher, string furtherDesc = null)
        {
            dispatcher.Register<HelpVerb>(new[] { "help", "?", "gethelp", "get-help" });
        }

        /// <summary>
        /// Registers a exit verb.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="back">true if only for turning back; otherwise, false.</param>
        /// <returns>The exit verb instance.</returns>
        public static void RegisterExit(this Dispatcher dispatcher, bool back = false)
        {
            dispatcher.Register<ExitVerb>(new[] { "exit", "quit", "bye", "goodbye" });
        }

        /// <summary>
        /// Processes a command.
        /// </summary>
        /// <param name="cmd">The command string.</param>
        /// <returns>The output string.</returns>
        public static string Command(string cmd)
        {
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return output;
        }

        /// <summary>
        /// Write a line and an empty line to the standard output stream.
        /// </summary>
        /// <param name="str">The string to ouput.</param>
        internal static void WriteLine(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                System.Console.WriteLine(str);
                System.Console.WriteLine();
            }
        }
    }
}
