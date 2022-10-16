using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Trivial.CommandLine;

class Program
{
    static Task Main(string[] args)
    {
        DefaultConsole.WriteLine(new LinearGradientConsoleStyle(null, Color.FromArgb(15, 250, 250), Color.FromArgb(85, 168, 255))
        {
            Bold = true
        }, "Trivial Sample");
        DefaultConsole.WriteLine();
        var dispatcher = new CommandDispatcher();
        dispatcher.Register<Tasks.InterceptorVerb>("interceptor");
        dispatcher.Register<Reflection.SingletonKeeperVerb>("singleton");
        dispatcher.Register<Maths.MathsVerb>("maths");
        dispatcher.Register<Security.TokenClientVerb>("token");
        dispatcher.Register<Net.HttpClientVerb>("http");
        dispatcher.Register<Tasks.RetryVerb>("retry");
        dispatcher.Register<Text.CsvVerb>("csv");
        dispatcher.Register<ChemistryVerb>("chemistry");
        dispatcher.Register<SelectionVerb>("select");
        dispatcher.Register<Data.BarcodeVerb>("barcode");
        return dispatcher.ProcessAsync();
    }

    public static void TestConsoleInterface()
    {
        var cli = StyleConsole.Default;
        cli.Flush();
        var mode = StyleConsole.Default.Mode;
        cli.Write(ConsoleColor.Yellow, $"Pass ({mode}) ");
        using var secret = cli.ReadPassword('*', true);
        if (secret == null || secret.Length < 1)
        {
            StyleConsole.Default.Mode = mode;
            cli.Backspace(24);
            return;
        }

        cli.WriteLine();
        StyleConsole.Default.Mode = StyleConsole.Modes.Ansi;
        cli.Write("ANSI " + new ConsoleTextStyle
        {
            ForegroundConsoleColor = ConsoleColor.Blue,
            Italic = true
        });
        cli.ReadPassword('*');
        StyleConsole.Default.Mode = StyleConsole.Modes.Cmd;
        cli.Write(ConsoleColor.Blue, "CMD ");
        cli.ReadPassword('*');
        StyleConsole.Default.Mode = StyleConsole.Modes.Text;
        cli.Write(ConsoleColor.Blue, "TEXT ");
        cli.ReadPassword('*');
        StyleConsole.Default.Mode = mode;
        cli.WriteLine("abc\b\b\b\b\b\b\b\b~       " + mode.ToString());

        cli.WriteLine("1 abcdefg hijklmn");
        cli.WriteLine("1 abcdefg hijklmn");
        cli.WriteLine("1 abcdefg hijklmn");
        cli.Write("1 abcdefg hijklmn");
        cli.MoveCursorBy(-6, -2);
        cli.Clear(StyleConsole.RelativeAreas.ToBeginningOfLine);
        cli.WriteLine("x");
        cli.ReadLine();

        cli.WriteLine("2 abcdefg hijklmn");
        cli.WriteLine("2 abcdefg hijklmn");
        cli.WriteLine("2 abcdefg hijklmn");
        cli.Write("2 abcdefg hijklmn");
        cli.MoveCursorBy(-6, -2);
        cli.Clear(StyleConsole.RelativeAreas.ToEndOfLine);
        cli.WriteLine("x");
        cli.ReadLine();

        cli.WriteLine("3 abcdefg hijklmn");
        cli.WriteLine("3 abcdefg hijklmn");
        cli.WriteLine("3 abcdefg hijklmn");
        cli.Write("3 abcdefg hijklmn");
        cli.MoveCursorBy(-6, -2);
        cli.Clear(StyleConsole.RelativeAreas.ToEndOfScreen);
        cli.WriteLine("x");
        cli.ReadLine();

        cli.WriteLine("4 abcdefg hijklmn");
        cli.WriteLine("4 abcdefg hijklmn");
        cli.WriteLine("4 abcdefg hijklmn");
        cli.Write("4 abcdefg hijklmn");
        cli.MoveCursorBy(-6, -2);
        cli.Clear(StyleConsole.RelativeAreas.ToBeginningOfScreen);
        cli.WriteLine("x");
        cli.ReadLine();

        cli.Clear(StyleConsole.RelativeAreas.Line);
    }
}
