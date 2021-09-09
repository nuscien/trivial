using System;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine("Trivial Sample");
            Console.WriteLine();

            //DefaultConsoleInterface.Flush();
            //var mode = ConsoleInterface.Default.Mode;
            //ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Ansi;
            //DefaultConsoleInterface.Write("ANSI " + new ConsoleTextStyle
            //{
            //    ForegroundConsoleColor = ConsoleColor.Blue,
            //    Italic = true
            //});
            //DefaultConsoleInterface.ReadPassword('*');
            //DefaultConsoleInterface.WriteLine();
            //ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Cmd;
            //DefaultConsoleInterface.Write(ConsoleColor.Blue, "CMD ");
            //DefaultConsoleInterface.ReadPassword('*');
            //DefaultConsoleInterface.WriteLine();
            //ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Text;
            //DefaultConsoleInterface.Write(ConsoleColor.Blue, "TEXT ");
            //DefaultConsoleInterface.ReadPassword('*');
            //DefaultConsoleInterface.WriteLine();
            //ConsoleInterface.Default.Mode = mode;
            //DefaultConsoleInterface.WriteLine(mode.ToString());

            var dispatcher = new CommandDispatcher();
            dispatcher.Register<Tasks.InterceptorVerb>("interceptor");
            dispatcher.Register<Reflection.SingletonKeeperVerb>("singleton");
            dispatcher.Register<Maths.MathsVerb>("maths");
            dispatcher.Register<Security.TokenClientVerb>("token");
            dispatcher.Register<Net.HttpClientVerb>("http");
            dispatcher.Register<Tasks.RetryVerb>("retry");
            dispatcher.Register<Text.CsvVerb>("csv");
            dispatcher.Register<Chemistry.ChemistryVerb>("chemistry");
            return dispatcher.ProcessAsync(args);
        }
    }
}
