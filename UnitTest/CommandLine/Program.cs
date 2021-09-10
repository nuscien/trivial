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
#if DEBUG
            TestConsoleInterface();
#endif
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

        private static void TestConsoleInterface()
        {
            DefaultConsoleInterface.Flush();
            var mode = ConsoleInterface.Default.Mode;
            DefaultConsoleInterface.Write(ConsoleColor.Yellow, $"Pass ({mode}) ");
            using var secret = DefaultConsoleInterface.ReadPassword('*', true);
            if (secret.Length < 1)
            {
                ConsoleInterface.Default.Mode = mode;
                DefaultConsoleInterface.Backspace(24);
                return;
            }

            DefaultConsoleInterface.WriteLine();
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
            DefaultConsoleInterface.WriteLine("abc\b\b\b\b\b\b\b\b~       " + mode.ToString());

            Console.WriteLine("1 abcdefg hijklmn");
            Console.WriteLine("1 abcdefg hijklmn");
            Console.WriteLine("1 abcdefg hijklmn");
            Console.Write("1 abcdefg hijklmn");
            DefaultConsoleInterface.MoveCursorBy(-6, -2);
            DefaultConsoleInterface.Clear(ConsoleInterface.RelativeAreas.ToBeginningOfLine);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("2 abcdefg hijklmn");
            Console.WriteLine("2 abcdefg hijklmn");
            Console.WriteLine("2 abcdefg hijklmn");
            Console.Write("2 abcdefg hijklmn");
            DefaultConsoleInterface.MoveCursorBy(-6, -2);
            DefaultConsoleInterface.Clear(ConsoleInterface.RelativeAreas.ToEndOfLine);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("3 abcdefg hijklmn");
            Console.WriteLine("3 abcdefg hijklmn");
            Console.WriteLine("3 abcdefg hijklmn");
            Console.Write("3 abcdefg hijklmn");
            DefaultConsoleInterface.MoveCursorBy(-6, -2);
            DefaultConsoleInterface.Clear(ConsoleInterface.RelativeAreas.ToEndOfScreen);
            Console.WriteLine("x");
            Console.ReadLine();

            Console.WriteLine("4 abcdefg hijklmn");
            Console.WriteLine("4 abcdefg hijklmn");
            Console.WriteLine("4 abcdefg hijklmn");
            Console.Write("4 abcdefg hijklmn");
            DefaultConsoleInterface.MoveCursorBy(-6, -2);
            DefaultConsoleInterface.Clear(ConsoleInterface.RelativeAreas.ToBeginningOfScreen);
            Console.WriteLine("x");
            Console.ReadLine();

            DefaultConsoleInterface.Clear(ConsoleInterface.RelativeAreas.Line);
        }
    }
}
