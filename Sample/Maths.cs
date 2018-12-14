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

            // Factorial.
            ConsoleLine.Write("20! = {0}.", Maths.Arithmetic.Factorial(20));
            ConsoleLine.End();
            ConsoleLine.Write("100! {1} {0}.", Maths.Arithmetic.FactorialApproximate(100), Maths.BooleanSymbols.SimilarSign);
            ConsoleLine.End();
            ConsoleLine.End(true);

            // English numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "English numbers");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.EnglishNumber.Default);
            WriteNumber(2305843009213693951, Maths.EnglishNumber.Default);
            WriteNumber(1000000001, Maths.EnglishNumber.Default);
            WriteNumber(10086, Maths.EnglishNumber.Default);
            WriteNumber(9000, Maths.EnglishNumber.Default);
            WriteNumber(2018, Maths.EnglishNumber.Default);
            WriteNumber(2004, Maths.EnglishNumber.Default);
            WriteNumber(1999, Maths.EnglishNumber.Default);
            WriteNumber(1024, Maths.EnglishNumber.Default);
            WriteNumber(1000, Maths.EnglishNumber.Default);
            WriteNumber(240, Maths.EnglishNumber.Default);
            WriteNumber(101, Maths.EnglishNumber.Default);
            WriteNumber(100, Maths.EnglishNumber.Default);
            WriteNumber(17, Maths.EnglishNumber.Default);
            WriteNumber(10, Maths.EnglishNumber.Default);
            WriteNumber(7, Maths.EnglishNumber.Default);
            WriteNumber(0, Maths.EnglishNumber.Default);
            WriteNumber(-12345, Maths.EnglishNumber.Default);
            WriteNumber(3.14159265, Maths.EnglishNumber.Default);
            WriteNumber(1.23e45, Maths.EnglishNumber.Default);
            WriteNumber(-10001.4567, Maths.EnglishNumber.Default);
            WriteNumber(-6.7800e90, Maths.EnglishNumber.Default);
            ConsoleLine.End(true);

            // Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumber.Simplified);
            WriteNumber(2305843009213693951, Maths.ChineseNumber.Simplified);
            WriteNumber(1000000001, Maths.ChineseNumber.Simplified);
            WriteNumber(10086, Maths.ChineseNumber.Simplified);
            WriteNumber(9000, Maths.ChineseNumber.Simplified);
            WriteNumber(2018, Maths.ChineseNumber.Simplified);
            WriteNumber(2004, Maths.ChineseNumber.Simplified);
            WriteNumber(1999, Maths.ChineseNumber.Simplified);
            WriteNumber(1024, Maths.ChineseNumber.Simplified);
            WriteNumber(1000, Maths.ChineseNumber.Simplified);
            WriteNumber(240, Maths.ChineseNumber.Simplified);
            WriteNumber(101, Maths.ChineseNumber.Simplified);
            WriteNumber(100, Maths.ChineseNumber.Simplified);
            WriteNumber(17, Maths.ChineseNumber.Simplified);
            WriteNumber(10, Maths.ChineseNumber.Simplified);
            WriteNumber(7, Maths.ChineseNumber.Simplified);
            WriteNumber(0, Maths.ChineseNumber.Simplified);
            WriteNumber(-12345, Maths.ChineseNumber.Simplified);
            WriteNumber(3.14159265, Maths.ChineseNumber.Simplified);
            WriteNumber(1.23e45, Maths.ChineseNumber.Simplified);
            WriteNumber(-10001.4567, Maths.ChineseNumber.Simplified);
            WriteNumber(-6.7800e90, Maths.ChineseNumber.Simplified);
            ConsoleLine.End(true);

            // Simplified Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Simplified Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(2305843009213693951, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(100000000000001, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(-9876543210, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(3.14159265, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(1.23e45, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(-10001.4567, Maths.ChineseNumber.SimplifiedUppercase);
            WriteNumber(-6.7800e90, Maths.ChineseNumber.SimplifiedUppercase);
            ConsoleLine.End(true);

            // Traditional Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumber.Traditional);
            WriteNumber(2305843009213693951, Maths.ChineseNumber.Traditional);
            WriteNumber(100000000000001, Maths.ChineseNumber.Traditional);
            WriteNumber(-9876543210, Maths.ChineseNumber.Traditional);
            WriteNumber(3.14159265, Maths.ChineseNumber.Traditional);
            WriteNumber(1.23e45, Maths.ChineseNumber.Traditional);
            WriteNumber(-10001.4567, Maths.ChineseNumber.Traditional);
            WriteNumber(-6.7800e90, Maths.ChineseNumber.Traditional);
            ConsoleLine.End(true);

            // Traditional Chinese uppercase numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Traditional Chinese uppercase number");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(2305843009213693951, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(100000000000001, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(-9876543210, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(3.14159265, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(1.23e45, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(-10001.4567, Maths.ChineseNumber.TraditionalUppercase);
            WriteNumber(-6.7800e90, Maths.ChineseNumber.TraditionalUppercase);
            ConsoleLine.End(true);
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
            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value), localInt.ToString(value, true));
            ConsoleLine.End();
        }

        private void WriteNumber(double value, Maths.INumberLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}.", value, localInt.ToString(value));
            ConsoleLine.End();
        }
    }
}
