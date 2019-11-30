using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Maths;

namespace Trivial.UnitTest.Maths
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
                        WriteNumber(VerbParameter.Values[1], EnglishNumerals.Default);
                        break;
                    case "zh-hans":
                    case "simplified":
                    case "chinese":
                    case "putonghua":
                        WriteNumber(VerbParameter.Values[1], ChineseNumerals.Simplified);
                        break;
                    case "daxie":
                        WriteNumber(VerbParameter.Values[1], ChineseNumerals.SimplifiedUppercase);
                        break;
                    case "zh-hant":
                    case "traditional":
                        WriteNumber(VerbParameter.Values[1], ChineseNumerals.Traditional);
                        break;
                    case "ja":
                    case "japanese":
                        WriteNumber(VerbParameter.Values[1], JapaneseNumerals.Default);
                        break;
                    case "kana":
                        WriteNumber(VerbParameter.Values[1], JapaneseNumerals.Kana);
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
            ConsoleLine.Write("gcd({0}, {1}) = {2}.", 192, 128, Arithmetic.Gcd(192, 128));
            ConsoleLine.End();
            ConsoleLine.Write("gcd({0}, {1}) = {2}.", 67, 31, Arithmetic.Gcd(67, 31));
            ConsoleLine.End();
            ConsoleLine.Write("lcm({0}, {1}) = {2}.", 192, 128, Arithmetic.Lcm(192, 128));
            ConsoleLine.End();
            ConsoleLine.Write("lcm({0}, {1}) = {2}.", 67, 31, Arithmetic.Lcm(67, 31));
            ConsoleLine.End();

            // Factorial.
            ConsoleLine.Write("20! = {0}.", Arithmetic.Factorial(20));
            ConsoleLine.End();
            ConsoleLine.Write("100! {1} {0}.", Arithmetic.FactorialApproximate(100), BooleanSymbols.SimilarSign);
            ConsoleLine.End();
            ConsoleLine.End(true);

            // English numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "English numbers");
            ConsoleLine.End();
            ConsoleLine.WriteLine(EnglishNumerals.Default.ToApproximationString(9876543210));
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
            ConsoleLine.End(true);

            // Simplified Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(ChineseNumerals.Simplified.ToApproximationString(9876543210));
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
            ConsoleLine.End(true);

            // Simplified Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(2305843009213693951, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(100000000000001, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-9876543210, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(3.14159265, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(1.23e45, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-10001.4567, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-6.7800e90, ChineseNumerals.SimplifiedUppercase);
            ConsoleLine.End(true);

            // Traditional Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese number");
            ConsoleLine.End();
            WriteNumber(9876543210, ChineseNumerals.Traditional);
            WriteNumber(2305843009213693951, ChineseNumerals.Traditional);
            WriteNumber(100000000000001, ChineseNumerals.Traditional);
            WriteNumber(-9876543210, ChineseNumerals.Traditional);
            WriteNumber(3.14159265, ChineseNumerals.Traditional);
            WriteNumber(1.23e45, ChineseNumerals.Traditional);
            WriteNumber(-10001.4567, ChineseNumerals.Traditional);
            WriteNumber(-6.7800e90, ChineseNumerals.Traditional);
            ConsoleLine.End(true);

            // Traditional Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, ChineseNumerals.TraditionalUppercase);
            WriteNumber(2305843009213693951, ChineseNumerals.TraditionalUppercase);
            WriteNumber(100000000000001, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-9876543210, ChineseNumerals.TraditionalUppercase);
            WriteNumber(3.14159265, ChineseNumerals.TraditionalUppercase);
            WriteNumber(1.23e45, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-10001.4567, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-6.7800e90, ChineseNumerals.TraditionalUppercase);
            ConsoleLine.End(true);

            // Japanese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Japanese number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(JapaneseNumerals.Default.ToApproximationString(9876543210));
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
            ConsoleLine.End(true);

            // Japanese Kana numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Japanese Kana number");
            ConsoleLine.End();
            ConsoleLine.WriteLine(JapaneseNumerals.Kana.ToApproximationString(9876543210));
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
                Arithmetic.IsPrime(value) ? string.Empty : "NOT ",
                await Arithmetic.PreviousPrimeAsync(value),
                await Arithmetic.NextPrimeAsync(value)
                );
            ConsoleLine.End();
        }

        private async Task WriteIsPrimeAsync(long value, CancellationToken cancellationToken)
        {
            ConsoleLine.Write("{0} is {1}a prime number.", value, await Arithmetic.IsPrimeAsync(value, cancellationToken) ? string.Empty : "NOT ");
            ConsoleLine.End();
        }

        private void WriteNumber(long value, INumberLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
            ConsoleLine.End();
        }

        private void WriteNumber(double value, INumberLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}.", value, localInt.ToString(value));
            ConsoleLine.End();
        }

        private void WriteNumber(string str, INumberLocalization localInt)
        {
            if (!long.TryParse(str, out long value))
            {
                if (!double.TryParse(str, out double dV))
                {
                    ConsoleLine.Write("Expect a number.");
                    ConsoleLine.End();
                    return;
                }

                ConsoleLine.Write("{0}: {1}.", dV, localInt.ToString(dV));
                ConsoleLine.End();
                return;
            }

            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
            ConsoleLine.End();
        }
    }
}
