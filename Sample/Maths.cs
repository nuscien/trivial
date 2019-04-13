using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Sample
{
    class MathsVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "Maths library";

        public override async Task ProcessAsync()
        {
            if (VerbParameter.Count >= 2)
            {
                switch (VerbParameter.Values[0].ToLower())
                {
                    case "en":
                    case "english":
                        WriteNumber(VerbParameter.Values[1], Maths.EnglishNumerals.Default);
                        break;
                    case "zh-hans":
                    case "simplified":
                    case "chinese":
                    case "putonghua":
                        WriteNumber(VerbParameter.Values[1], Maths.ChineseNumerals.Simplified);
                        break;
                    case "daxie":
                        WriteNumber(VerbParameter.Values[1], Maths.ChineseNumerals.SimplifiedUppercase);
                        break;
                    case "zh-hant":
                    case "traditional":
                        WriteNumber(VerbParameter.Values[1], Maths.ChineseNumerals.Traditional);
                        break;
                    case "ja":
                    case "japanese":
                        WriteNumber(VerbParameter.Values[1], Maths.JapaneseNumerals.Default);
                        break;
                    case "kana":
                        WriteNumber(VerbParameter.Values[1], Maths.JapaneseNumerals.Kana);
                        break;
                }

                return;
            }

            await Test();
        }

        private async Task Test()
        {
            // Prime.
            ConsoleLine.Write(ConsoleColor.Magenta, "Arithmetic");
            ConsoleLine.End();
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
                ConsoleLine.Write("Operation is canceled succeeded.");
                ConsoleLine.End();
            }

            // GCD and LCM.
            ConsoleLine.Write("gcd({0}, {1}) = {2}.", 192, 128, Maths.Arithmetic.Gcd(192, 128));
            ConsoleLine.End();
            ConsoleLine.Write("gcd({0}, {1}) = {2}.", 67, 31, Maths.Arithmetic.Gcd(67, 31));
            ConsoleLine.End();
            ConsoleLine.Write("lcm({0}, {1}) = {2}.", 192, 128, Maths.Arithmetic.Lcm(192, 128));
            ConsoleLine.End();
            ConsoleLine.Write("lcm({0}, {1}) = {2}.", 67, 31, Maths.Arithmetic.Lcm(67, 31));
            ConsoleLine.End();

            // Factorial.
            ConsoleLine.Write("20! = {0}.", Maths.Arithmetic.Factorial(20));
            ConsoleLine.End();
            ConsoleLine.Write("100! {1} {0}.", Maths.Arithmetic.FactorialApproximate(100), Maths.BooleanSymbols.SimilarSign);
            ConsoleLine.End();
            ConsoleLine.End(true);

            // English numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "English numbers");
            ConsoleLine.End();
            ConsoleLine.WriteLine(Maths.EnglishNumerals.Default.ToApproximationString(9876543210));
            WriteNumber(9876543210, Maths.EnglishNumerals.Default);
            WriteNumber(2305843009213693951, Maths.EnglishNumerals.Default);
            WriteNumber(1000000001, Maths.EnglishNumerals.Default);
            WriteNumber(10086, Maths.EnglishNumerals.Default);
            WriteNumber(9000, Maths.EnglishNumerals.Default);
            WriteNumber(2018, Maths.EnglishNumerals.Default);
            WriteNumber(2004, Maths.EnglishNumerals.Default);
            WriteNumber(1999, Maths.EnglishNumerals.Default);
            WriteNumber(1024, Maths.EnglishNumerals.Default);
            WriteNumber(1000, Maths.EnglishNumerals.Default);
            WriteNumber(240, Maths.EnglishNumerals.Default);
            WriteNumber(101, Maths.EnglishNumerals.Default);
            WriteNumber(100, Maths.EnglishNumerals.Default);
            WriteNumber(17, Maths.EnglishNumerals.Default);
            WriteNumber(10, Maths.EnglishNumerals.Default);
            WriteNumber(7, Maths.EnglishNumerals.Default);
            WriteNumber(0, Maths.EnglishNumerals.Default);
            WriteNumber(-12345, Maths.EnglishNumerals.Default);
            WriteNumber(3.14159265, Maths.EnglishNumerals.Default);
            WriteNumber(1.23e45, Maths.EnglishNumerals.Default);
            WriteNumber(-10001.4567, Maths.EnglishNumerals.Default);
            WriteNumber(-6.7800e90, Maths.EnglishNumerals.Default);
            ConsoleLine.End(true);

            // Simplified Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(Maths.ChineseNumerals.Simplified.ToApproximationString(9876543210));
            WriteNumber(9876543210, Maths.ChineseNumerals.Simplified);
            WriteNumber(2305843009213693951, Maths.ChineseNumerals.Simplified);
            WriteNumber(1000000001, Maths.ChineseNumerals.Simplified);
            WriteNumber(10086, Maths.ChineseNumerals.Simplified);
            WriteNumber(9000, Maths.ChineseNumerals.Simplified);
            WriteNumber(2018, Maths.ChineseNumerals.Simplified);
            WriteNumber(2004, Maths.ChineseNumerals.Simplified);
            WriteNumber(1999, Maths.ChineseNumerals.Simplified);
            WriteNumber(1024, Maths.ChineseNumerals.Simplified);
            WriteNumber(1000, Maths.ChineseNumerals.Simplified);
            WriteNumber(240, Maths.ChineseNumerals.Simplified);
            WriteNumber(101, Maths.ChineseNumerals.Simplified);
            WriteNumber(100, Maths.ChineseNumerals.Simplified);
            WriteNumber(17, Maths.ChineseNumerals.Simplified);
            WriteNumber(10, Maths.ChineseNumerals.Simplified);
            WriteNumber(7, Maths.ChineseNumerals.Simplified);
            WriteNumber(0, Maths.ChineseNumerals.Simplified);
            WriteNumber(-12345, Maths.ChineseNumerals.Simplified);
            WriteNumber(3.14159265, Maths.ChineseNumerals.Simplified);
            WriteNumber(4e24, Maths.ChineseNumerals.Simplified);
            WriteNumber(1.23e45, Maths.ChineseNumerals.Simplified);
            WriteNumber(-10001.4567, Maths.ChineseNumerals.Simplified);
            WriteNumber(-6.7800e90, Maths.ChineseNumerals.Simplified);
            ConsoleLine.End(true);

            // Simplified Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(2305843009213693951, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(100000000000001, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-9876543210, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(3.14159265, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(1.23e45, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-10001.4567, Maths.ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-6.7800e90, Maths.ChineseNumerals.SimplifiedUppercase);
            ConsoleLine.End(true);

            // Traditional Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumerals.Traditional);
            WriteNumber(2305843009213693951, Maths.ChineseNumerals.Traditional);
            WriteNumber(100000000000001, Maths.ChineseNumerals.Traditional);
            WriteNumber(-9876543210, Maths.ChineseNumerals.Traditional);
            WriteNumber(3.14159265, Maths.ChineseNumerals.Traditional);
            WriteNumber(1.23e45, Maths.ChineseNumerals.Traditional);
            WriteNumber(-10001.4567, Maths.ChineseNumerals.Traditional);
            WriteNumber(-6.7800e90, Maths.ChineseNumerals.Traditional);
            ConsoleLine.End(true);

            // Traditional Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(2305843009213693951, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(100000000000001, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(-9876543210, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(3.14159265, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(1.23e45, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(-10001.4567, Maths.ChineseNumerals.TraditionalUppercase);
            WriteNumber(-6.7800e90, Maths.ChineseNumerals.TraditionalUppercase);
            ConsoleLine.End(true);

            // Japanese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Japanese number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(Maths.JapaneseNumerals.Default.ToApproximationString(9876543210));
            WriteNumber(9876543210, Maths.JapaneseNumerals.Default);
            WriteNumber(2305843009213693951, Maths.JapaneseNumerals.Default);
            WriteNumber(1000000001, Maths.JapaneseNumerals.Default);
            WriteNumber(10086, Maths.JapaneseNumerals.Default);
            WriteNumber(9000, Maths.JapaneseNumerals.Default);
            WriteNumber(2018, Maths.JapaneseNumerals.Default);
            WriteNumber(2004, Maths.JapaneseNumerals.Default);
            WriteNumber(1999, Maths.JapaneseNumerals.Default);
            WriteNumber(1024, Maths.JapaneseNumerals.Default);
            WriteNumber(1000, Maths.JapaneseNumerals.Default);
            WriteNumber(240, Maths.JapaneseNumerals.Default);
            WriteNumber(101, Maths.JapaneseNumerals.Default);
            WriteNumber(100, Maths.JapaneseNumerals.Default);
            WriteNumber(17, Maths.JapaneseNumerals.Default);
            WriteNumber(10, Maths.JapaneseNumerals.Default);
            WriteNumber(7, Maths.JapaneseNumerals.Default);
            WriteNumber(0, Maths.JapaneseNumerals.Default);
            WriteNumber(-12345, Maths.JapaneseNumerals.Default);
            WriteNumber(3.14159265, Maths.JapaneseNumerals.Default);
            WriteNumber(4e24, Maths.JapaneseNumerals.Default);
            WriteNumber(1.23e45, Maths.JapaneseNumerals.Default);
            WriteNumber(-10001.4567, Maths.JapaneseNumerals.Default);
            WriteNumber(-6.7800e90, Maths.JapaneseNumerals.Default);
            ConsoleLine.End(true);

            // Japanese Kana numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Japanese Kana number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(Maths.JapaneseNumerals.Kana.ToApproximationString(9876543210));
            WriteNumber(9876543210, Maths.JapaneseNumerals.Kana);
            WriteNumber(2305843009213693951, Maths.JapaneseNumerals.Kana);
            WriteNumber(1000000001, Maths.JapaneseNumerals.Kana);
            WriteNumber(10086, Maths.JapaneseNumerals.Kana);
            WriteNumber(9000, Maths.JapaneseNumerals.Kana);
            WriteNumber(2018, Maths.JapaneseNumerals.Kana);
            WriteNumber(2004, Maths.JapaneseNumerals.Kana);
            WriteNumber(1999, Maths.JapaneseNumerals.Kana);
            WriteNumber(1024, Maths.JapaneseNumerals.Kana);
            WriteNumber(1000, Maths.JapaneseNumerals.Kana);
            WriteNumber(240, Maths.JapaneseNumerals.Kana);
            WriteNumber(101, Maths.JapaneseNumerals.Kana);
            WriteNumber(100, Maths.JapaneseNumerals.Kana);
            WriteNumber(17, Maths.JapaneseNumerals.Kana);
            WriteNumber(10, Maths.JapaneseNumerals.Kana);
            WriteNumber(7, Maths.JapaneseNumerals.Kana);
            WriteNumber(0, Maths.JapaneseNumerals.Kana);
            WriteNumber(-12345, Maths.JapaneseNumerals.Kana);
            WriteNumber(3.14159265, Maths.JapaneseNumerals.Kana);
            WriteNumber(4e24, Maths.JapaneseNumerals.Kana);
            WriteNumber(1.23e45, Maths.JapaneseNumerals.Kana);
            WriteNumber(-10001.4567, Maths.JapaneseNumerals.Kana);
            WriteNumber(-6.7800e90, Maths.JapaneseNumerals.Kana);
            ConsoleLine.End(true);

            // Location
            var location = new Geography.Geolocation(new Geography.Latitude(148, 100, 17), new Geography.Longitude(120.5), 10);
            ConsoleLine.Write("{0} {1} {2}", location.Latitude.Type, location.Longitude.Type, location.Latitude.Value);
            ConsoleLine.End();
            ConsoleLine.Write(location.ToString());
            ConsoleLine.End();
        }

        private async Task WriteIsPrimeAsync(int value)
        {
            ConsoleLine.Write(
                "{0} is {1}a prime number. Prev {2}, next {3}.",
                value,
                Maths.Arithmetic.IsPrime(value) ? string.Empty : "NOT ",
                await Maths.Arithmetic.PreviousPrimeAsync(value),
                await Maths.Arithmetic.NextPrimeAsync(value)
                );
            ConsoleLine.End();
        }

        private async Task WriteIsPrimeAsync(long value, CancellationToken cancellationToken)
        {
            ConsoleLine.Write("{0} is {1}a prime number.", value, await Maths.Arithmetic.IsPrimeAsync(value, cancellationToken) ? string.Empty : "NOT ");
            ConsoleLine.End();
        }

        private void WriteNumber(long value, Maths.INumberLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
            ConsoleLine.End();
        }

        private void WriteNumber(double value, Maths.INumberLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}.", value, localInt.ToString(value));
            ConsoleLine.End();
        }

        private void WriteNumber(string str, Maths.INumberLocalization localInt)
        {
            if (!long.TryParse(str, out long value))
            {
                ConsoleLine.Write("Expect a number.");
                ConsoleLine.End();
            }

            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
            ConsoleLine.End();
        }
    }
}
