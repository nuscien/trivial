using System;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    class Program
    {
        static Task Main(string[] args)
        {
            DefaultConsoleInterface.WriteLine("Trivial Sample");
            DefaultConsoleInterface.WriteLine();

            // TestConsoleInterface()；
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

        private void TestConsoleInterface()
        {
            DefaultConsoleInterface.Flush();
            var mode = ConsoleInterface.Default.Mode;
            ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Ansi;
            DefaultConsoleInterface.Write("ANSI " + new ConsoleTextStyle
            {
                ForegroundConsoleColor = ConsoleColor.Blue,
                Italic = true
            });
            DefaultConsoleInterface.ReadPassword('*');
            ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Cmd;
            DefaultConsoleInterface.Write(ConsoleColor.Blue, "CMD ");
            DefaultConsoleInterface.ReadPassword('*');
            ConsoleInterface.Default.Mode = ConsoleInterface.Modes.Text;
            DefaultConsoleInterface.Write(ConsoleColor.Blue, "TEXT ");
            DefaultConsoleInterface.ReadPassword('*');
            ConsoleInterface.Default.Mode = mode;
            DefaultConsoleInterface.WriteLine(mode.ToString());

            Console.WriteLine("Test");
            Console.WriteLine("abc\b\b\b\b\b\b\b\b        x");
            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.Write("abcdefg hijklmn");
            ConsoleInterface.Default.MoveCursorBy(-6, -2);
            ConsoleInterface.Default.Clear(ConsoleInterface.RelativeAreas.ToBeginningOfLine);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.Write("abcdefg hijklmn");
            ConsoleInterface.Default.MoveCursorBy(-6, -2);
            ConsoleInterface.Default.Clear(ConsoleInterface.RelativeAreas.ToEndOfLine);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.Write("abcdefg hijklmn");
            ConsoleInterface.Default.MoveCursorBy(-6, -2);
            ConsoleInterface.Default.Clear(ConsoleInterface.RelativeAreas.ToEndOfScreen);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.WriteLine("abcdefg hijklmn");
            Console.Write("abcdefg hijklmn");
            ConsoleInterface.Default.MoveCursorBy(-6, -2);
            ConsoleInterface.Default.Clear(ConsoleInterface.RelativeAreas.ToBeginningOfScreen);
            Console.WriteLine("x");
            Console.ReadLine();
            ConsoleInterface.Default.Clear(ConsoleInterface.RelativeAreas.Line);
        }
    }
}
