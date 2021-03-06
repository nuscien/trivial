﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Maths
{
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
                }

                return;
            }

            await Test();
        }

        private async Task Test()
        {
            // Prime.
            Console.WriteLine("Arithmetic");
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
                Console.WriteLine("Operation is canceled succeeded.");
            }

            // GCD and LCM.
            Console.WriteLine("gcd({0}, {1}) = {2}.", 192, 128, Arithmetic.Gcd(192, 128));
            Console.WriteLine("gcd({0}, {1}) = {2}.", 67, 31, Arithmetic.Gcd(67, 31));
            Console.WriteLine("lcm({0}, {1}) = {2}.", 192, 128, Arithmetic.Lcm(192, 128));
            Console.WriteLine("lcm({0}, {1}) = {2}.", 67, 31, Arithmetic.Lcm(67, 31));

            // Factorial.
            Console.WriteLine("20! = {0}.", Arithmetic.Factorial(20));
            Console.WriteLine("100! {1} {0}.", Arithmetic.FactorialApproximate(100), BooleanSymbols.SimilarSign);
            Console.WriteLine();

            // English numbers.
            Console.WriteLine("English numbers");
            Console.WriteLine(EnglishNumerals.Default.ToApproximationString(9876543210));
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
            Console.WriteLine();

            // Simplified Chinese numbers.
            Console.WriteLine("Simplified Chinese number");
            Console.WriteLine(ChineseNumerals.Simplified.ToApproximationString(9876543210));
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
            Console.WriteLine();

            // Simplified Chinese uppercase numbers.
            Console.WriteLine("Simplified Chinese uppercase number");
            WriteNumber(9876543210, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(2305843009213693951, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(100000000000001, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-9876543210, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(3.14159265, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(1.23e45, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-10001.4567, ChineseNumerals.SimplifiedUppercase);
            WriteNumber(-6.7800e90, ChineseNumerals.SimplifiedUppercase);
            Console.WriteLine();

            // Traditional Chinese numbers.
            Console.WriteLine("Traditional Chinese number");
            WriteNumber(9876543210, ChineseNumerals.Traditional);
            WriteNumber(2305843009213693951, ChineseNumerals.Traditional);
            WriteNumber(100000000000001, ChineseNumerals.Traditional);
            WriteNumber(-9876543210, ChineseNumerals.Traditional);
            WriteNumber(3.14159265, ChineseNumerals.Traditional);
            WriteNumber(1.23e45, ChineseNumerals.Traditional);
            WriteNumber(-10001.4567, ChineseNumerals.Traditional);
            WriteNumber(-6.7800e90, ChineseNumerals.Traditional);
            Console.WriteLine();

            // Traditional Chinese uppercase numbers.
            Console.WriteLine("Traditional Chinese uppercase number");
            WriteNumber(9876543210, ChineseNumerals.TraditionalUppercase);
            WriteNumber(2305843009213693951, ChineseNumerals.TraditionalUppercase);
            WriteNumber(100000000000001, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-9876543210, ChineseNumerals.TraditionalUppercase);
            WriteNumber(3.14159265, ChineseNumerals.TraditionalUppercase);
            WriteNumber(1.23e45, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-10001.4567, ChineseNumerals.TraditionalUppercase);
            WriteNumber(-6.7800e90, ChineseNumerals.TraditionalUppercase);
            Console.WriteLine();

            // Japanese numbers.
            Console.WriteLine("Japanese number");
            Console.WriteLine(JapaneseNumerals.Default.ToApproximationString(9876543210));
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
            Console.WriteLine();

            // Japanese Kana numbers.
            Console.WriteLine("Japanese Kana number");
            Console.WriteLine(JapaneseNumerals.Kana.ToApproximationString(9876543210));
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
            Console.WriteLine();

            // Location
            var location = new Geography.Geolocation(new Geography.Latitude(148, 100, 17), new Geography.Longitude(120.5), 10);
            Console.WriteLine("{0} {1} {2}", location.Latitude.Type, location.Longitude.Type, location.Latitude.Value);
            Console.WriteLine(location.ToString());
        }

        private async Task WriteIsPrimeAsync(int value)
        {
            Console.WriteLine(
                "{0} is {1}a prime number. Prev {2}, next {3}.",
                value,
                Arithmetic.IsPrime(value) ? string.Empty : "NOT ",
                await Arithmetic.PreviousPrimeAsync(value),
                await Arithmetic.NextPrimeAsync(value)
                );
        }

        private async Task WriteIsPrimeAsync(long value, CancellationToken cancellationToken)
        {
            Console.WriteLine("{0} is {1}a prime number.", value, await Arithmetic.IsPrimeAsync(value, cancellationToken) ? string.Empty : "NOT ");
        }

        private void WriteNumber(long value, INumberLocalization localInt)
        {
            Console.WriteLine("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
        }

        private void WriteNumber(double value, INumberLocalization localInt)
        {
            Console.WriteLine("{0}: {1}.", value, localInt.ToString(value));
        }

        private void WriteNumber(string str, INumberLocalization localInt)
        {
            if (!long.TryParse(str, out long value))
            {
                if (!double.TryParse(str, out double dV))
                {
                    Console.WriteLine("Expect a number.");
                    return;
                }

                Console.WriteLine("{0}: {1}.", dV, localInt.ToString(dV));
                return;
            }

            Console.WriteLine("{0}: {1}; {2}.", value, localInt.ToString(value, false), localInt.ToString(value, true));
        }
    }
}
