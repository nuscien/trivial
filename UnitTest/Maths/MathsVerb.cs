using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Maths;

class MathsVerb : CommandLine.BaseCommandVerb
{
    public static string Description => "Maths library";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var verb = Arguments.Verb;
        if (verb.Count > 1)
        {
            var num = verb.Values[1];
            switch (verb.Values[0].ToLower())
            {
                case "en":
                case "english":
                    WriteNumber(num, EnglishNumerals.Default);
                    break;
                case "hans":
                case "zh-hans":
                case "chs":
                case "simplified":
                case "chinese":
                case "putonghua":
                    WriteNumber(num, ChineseNumerals.Simplified);
                    break;
                case "daxie":
                    WriteNumber(num, ChineseNumerals.SimplifiedUppercase);
                    break;
                case "hant":
                case "zh-hant":
                case "cht":
                case "traditional":
                    WriteNumber(num, ChineseNumerals.Traditional);
                    break;
                case "ja":
                case "japanese":
                    WriteNumber(num, JapaneseNumerals.Default);
                    break;
                case "kana":
                    WriteNumber(num, JapaneseNumerals.Kana);
                    break;
                case "roman":
                    {
                        if (Numbers.TryParseToInt32(num, 10, out var value) && value < 100000)
                            CommandLine.StyleConsole.Default.WriteLine($"{value}{Environment.NewLine}{RomanNumerals.Uppercase.ToString(value, true)}");
                        else
                            CommandLine.StyleConsole.Default.WriteLine(ConsoleColor.Red, "Expect an integer.");
                    }

                    break;
                case "parse":
                    {
                        if (Numbers.TryParseToInt64(num, 10, out var value))
                            CommandLine.StyleConsole.Default.WriteLine(value);
                        else
                            CommandLine.StyleConsole.Default.WriteLine(ConsoleColor.Red, "Parse failed.");
                    }

                    break;
            }

            return;
        }

        await Test();
    }

    private static async Task Test()
    {
        var cli = CommandLine.StyleConsole.Default;

        // Prime.
        cli.WriteLine("Arithmetic");
        await WriteIsPrimeAsync(524287);
        await WriteIsPrimeAsync(968455);
        await WriteIsPrimeAsync(2147483647, CancellationToken.None);
        await WriteIsPrimeAsync(21474836477, CancellationToken.None);
        try
        {
            var cancellation = new CancellationTokenSource();
            var isPrimeTask = WriteIsPrimeAsync(2305843009213693951, cancellation.Token);
            cancellation.Cancel();
            await isPrimeTask;
        }
        catch (OperationCanceledException)
        {
            cli.WriteLine("Operation is canceled succeeded.");
        }

        // GCD and LCM.
        cli.WriteLine("gcd({0}, {1}) = {2}.", 192, 128, Arithmetic.Gcd(192, 128));
        cli.WriteLine("gcd({0}, {1}) = {2}.", 67, 31, Arithmetic.Gcd(67, 31));
        cli.WriteLine("lcm({0}, {1}) = {2}.", 192, 128, Arithmetic.Lcm(192, 128));
        cli.WriteLine("lcm({0}, {1}) = {2}.", 67, 31, Arithmetic.Lcm(67, 31));

        // Factorial.
        cli.WriteLine("20! = {0}.", Arithmetic.Factorial(20));
        cli.WriteLine("100! {1} {0}.", Arithmetic.FactorialApproximate(100), BooleanSymbols.SimilarSign);
        cli.WriteLine();

        // English numbers.
        cli.WriteLine("English numbers");
        cli.WriteLine(EnglishNumerals.Default.ToApproximationString(9876543210));
        WriteNumber(9876543210, EnglishNumerals.Default);
        WriteNumber(2305843009213693951, EnglishNumerals.Default);
        WriteNumber(1000000001, EnglishNumerals.Default);
        WriteNumber(10086, EnglishNumerals.Default);
        WriteNumber(9000, EnglishNumerals.Default);
        WriteNumber(2018, EnglishNumerals.Default);
        WriteNumber(2004, EnglishNumerals.Default);
        WriteNumber(1999, EnglishNumerals.Default);
        WriteNumber(1024, EnglishNumerals.Default);
        WriteNumber(1000, EnglishNumerals.Default);
        WriteNumber(240, EnglishNumerals.Default);
        WriteNumber(101, EnglishNumerals.Default);
        WriteNumber(100, EnglishNumerals.Default);
        WriteNumber(17, EnglishNumerals.Default);
        WriteNumber(10, EnglishNumerals.Default);
        WriteNumber(7, EnglishNumerals.Default);
        WriteNumber(0, EnglishNumerals.Default);
        WriteNumber(-12345, EnglishNumerals.Default);
        WriteNumber(3.14159265, EnglishNumerals.Default);
        WriteNumber(1.23e45, EnglishNumerals.Default);
        WriteNumber(-10001.4567, EnglishNumerals.Default);
        WriteNumber(-6.7800e90, EnglishNumerals.Default);
        cli.WriteLine();

        // Simplified Chinese numbers.
        cli.WriteLine("Simplified Chinese number");
        cli.WriteLine(ChineseNumerals.Simplified.ToApproximationString(9876543210));
        WriteNumber(9876543210, ChineseNumerals.Simplified);
        WriteNumber(2305843009213693951, ChineseNumerals.Simplified);
        WriteNumber(1000000001, ChineseNumerals.Simplified);
        WriteNumber(10086, ChineseNumerals.Simplified);
        WriteNumber(9000, ChineseNumerals.Simplified);
        WriteNumber(2018, ChineseNumerals.Simplified);
        WriteNumber(2004, ChineseNumerals.Simplified);
        WriteNumber(1999, ChineseNumerals.Simplified);
        WriteNumber(1024, ChineseNumerals.Simplified);
        WriteNumber(1000, ChineseNumerals.Simplified);
        WriteNumber(240, ChineseNumerals.Simplified);
        WriteNumber(101, ChineseNumerals.Simplified);
        WriteNumber(100, ChineseNumerals.Simplified);
        WriteNumber(17, ChineseNumerals.Simplified);
        WriteNumber(10, ChineseNumerals.Simplified);
        WriteNumber(7, ChineseNumerals.Simplified);
        WriteNumber(0, ChineseNumerals.Simplified);
        WriteNumber(-12345, ChineseNumerals.Simplified);
        WriteNumber(3.14159265, ChineseNumerals.Simplified);
        WriteNumber(4e24, ChineseNumerals.Simplified);
        WriteNumber(1.23e45, ChineseNumerals.Simplified);
        WriteNumber(-10001.4567, ChineseNumerals.Simplified);
        WriteNumber(-6.7800e90, ChineseNumerals.Simplified);
        cli.WriteLine();

        // Simplified Chinese uppercase numbers.
        cli.WriteLine("Simplified Chinese uppercase number");
        WriteNumber(9876543210, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(2305843009213693951, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(100000000000001, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(-9876543210, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(3.14159265, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(1.23e45, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(-10001.4567, ChineseNumerals.SimplifiedUppercase);
        WriteNumber(-6.7800e90, ChineseNumerals.SimplifiedUppercase);
        cli.WriteLine();

        // Traditional Chinese numbers.
        cli.WriteLine("Traditional Chinese number");
        WriteNumber(9876543210, ChineseNumerals.Traditional);
        WriteNumber(2305843009213693951, ChineseNumerals.Traditional);
        WriteNumber(100000000000001, ChineseNumerals.Traditional);
        WriteNumber(-9876543210, ChineseNumerals.Traditional);
        WriteNumber(3.14159265, ChineseNumerals.Traditional);
        WriteNumber(1.23e45, ChineseNumerals.Traditional);
        WriteNumber(-10001.4567, ChineseNumerals.Traditional);
        WriteNumber(-6.7800e90, ChineseNumerals.Traditional);
        cli.WriteLine();

        // Traditional Chinese uppercase numbers.
        cli.WriteLine("Traditional Chinese uppercase number");
        WriteNumber(9876543210, ChineseNumerals.TraditionalUppercase);
        WriteNumber(2305843009213693951, ChineseNumerals.TraditionalUppercase);
        WriteNumber(100000000000001, ChineseNumerals.TraditionalUppercase);
        WriteNumber(-9876543210, ChineseNumerals.TraditionalUppercase);
        WriteNumber(3.14159265, ChineseNumerals.TraditionalUppercase);
        WriteNumber(1.23e45, ChineseNumerals.TraditionalUppercase);
        WriteNumber(-10001.4567, ChineseNumerals.TraditionalUppercase);
        WriteNumber(-6.7800e90, ChineseNumerals.TraditionalUppercase);
        cli.WriteLine();

        // Japanese numbers.
        cli.WriteLine("Japanese number");
        cli.WriteLine(JapaneseNumerals.Default.ToApproximationString(9876543210));
        WriteNumber(9876543210, JapaneseNumerals.Default);
        WriteNumber(2305843009213693951, JapaneseNumerals.Default);
        WriteNumber(1000000001, JapaneseNumerals.Default);
        WriteNumber(10086, JapaneseNumerals.Default);
        WriteNumber(9000, JapaneseNumerals.Default);
        WriteNumber(2018, JapaneseNumerals.Default);
        WriteNumber(2004, JapaneseNumerals.Default);
        WriteNumber(1999, JapaneseNumerals.Default);
        WriteNumber(1024, JapaneseNumerals.Default);
        WriteNumber(1000, JapaneseNumerals.Default);
        WriteNumber(240, JapaneseNumerals.Default);
        WriteNumber(101, JapaneseNumerals.Default);
        WriteNumber(100, JapaneseNumerals.Default);
        WriteNumber(17, JapaneseNumerals.Default);
        WriteNumber(10, JapaneseNumerals.Default);
        WriteNumber(7, JapaneseNumerals.Default);
        WriteNumber(0, JapaneseNumerals.Default);
        WriteNumber(-12345, JapaneseNumerals.Default);
        WriteNumber(3.14159265, JapaneseNumerals.Default);
        WriteNumber(4e24, JapaneseNumerals.Default);
        WriteNumber(1.23e45, JapaneseNumerals.Default);
        WriteNumber(-10001.4567, JapaneseNumerals.Default);
        WriteNumber(-6.7800e90, JapaneseNumerals.Default);
        cli.WriteLine();

        // Japanese Kana numbers.
        cli.WriteLine("Japanese Kana number");
        cli.WriteLine(JapaneseNumerals.Kana.ToApproximationString(9876543210));
        WriteNumber(9876543210, JapaneseNumerals.Kana);
        WriteNumber(2305843009213693951, JapaneseNumerals.Kana);
        WriteNumber(1000000001, JapaneseNumerals.Kana);
        WriteNumber(10086, JapaneseNumerals.Kana);
        WriteNumber(9000, JapaneseNumerals.Kana);
        WriteNumber(2018, JapaneseNumerals.Kana);
        WriteNumber(2004, JapaneseNumerals.Kana);
        WriteNumber(1999, JapaneseNumerals.Kana);
        WriteNumber(1024, JapaneseNumerals.Kana);
        WriteNumber(1000, JapaneseNumerals.Kana);
        WriteNumber(240, JapaneseNumerals.Kana);
        WriteNumber(101, JapaneseNumerals.Kana);
        WriteNumber(100, JapaneseNumerals.Kana);
        WriteNumber(17, JapaneseNumerals.Kana);
        WriteNumber(10, JapaneseNumerals.Kana);
        WriteNumber(7, JapaneseNumerals.Kana);
        WriteNumber(0, JapaneseNumerals.Kana);
        WriteNumber(-12345, JapaneseNumerals.Kana);
        WriteNumber(3.14159265, JapaneseNumerals.Kana);
        WriteNumber(4e24, JapaneseNumerals.Kana);
        WriteNumber(1.23e45, JapaneseNumerals.Kana);
        WriteNumber(-10001.4567, JapaneseNumerals.Kana);
        WriteNumber(-6.7800e90, JapaneseNumerals.Kana);
        cli.WriteLine();

        // Location
        var location = new Geography.Geolocation(new Geography.Latitude(148, 100, 17), new Geography.Longitude(120.5), 10);
        cli.WriteLine("{0} {1} {2}", location.Latitude.Type, location.Longitude.Type, location.Latitude.Value);
        cli.WriteLine(location.ToString());
    }

    private static async Task WriteIsPrimeAsync(int value)
        => CommandLine.StyleConsole.Default.WriteLine(
            "{0} is {1}a prime number. Prev {2}, next {3}.",
            value,
            Arithmetic.IsPrime(value) ? string.Empty : "NOT ",
            await Arithmetic.PreviousPrimeAsync(value),
            await Arithmetic.NextPrimeAsync(value)
            );

    private static async Task WriteIsPrimeAsync(long value, CancellationToken cancellationToken)
        => CommandLine.StyleConsole.Default.WriteLine("{0} is {1}a prime number.", value, await Arithmetic.IsPrimeAsync(value, cancellationToken) ? string.Empty : "NOT ");

    private static void WriteNumber(long value, INumberLocalization localInt)
        => CommandLine.StyleConsole.Default.WriteLine("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));

    private static void WriteNumber(double value, INumberLocalization localInt)
        => CommandLine.StyleConsole.Default.WriteLine("{0}: {1}.", value, localInt.ToString(value));

    private static void WriteNumber(string str, INumberLocalization localInt)
    {
        var cli = CommandLine.StyleConsole.Default;
        if (!long.TryParse(str, out var value))
        {
            if (!double.TryParse(str, out var dV))
            {
                cli.WriteLine($"{dV}{Environment.NewLine}{localInt.ToString(dV)}.");
                return;
            }
            else if (!Numbers.TryParseToInt64(str, 10, out value))
            {
                cli.WriteLine(ConsoleColor.Red, "Expect a number.");
                return;
            }
        }

        cli.WriteLine($"{value}{Environment.NewLine}{ localInt.ToString(value, false)}{Environment.NewLine}{localInt.ToString(value, true)}.");
    }
}
