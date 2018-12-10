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
            WriteIsPrime(524287);
            WriteIsPrime(524291);
            await WriteIsPrimeAsync(2147483647, CancellationToken.None);
            await WriteIsPrimeAsync(21474836479, CancellationToken.None);
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
            WriteNumber(9876543210, Maths.Numbers.English);
            WriteNumber(2305843009213693951, Maths.Numbers.English);
            WriteNumber(1000000001, Maths.Numbers.English);
            WriteNumber(10086, Maths.Numbers.English);
            WriteNumber(9000, Maths.Numbers.English);
            WriteNumber(2018, Maths.Numbers.English);
            WriteNumber(2004, Maths.Numbers.English);
            WriteNumber(1999, Maths.Numbers.English);
            WriteNumber(1024, Maths.Numbers.English);
            WriteNumber(1000, Maths.Numbers.English);
            WriteNumber(240, Maths.Numbers.English);
            WriteNumber(101, Maths.Numbers.English);
            WriteNumber(100, Maths.Numbers.English);
            WriteNumber(17, Maths.Numbers.English);
            WriteNumber(10, Maths.Numbers.English);
            WriteNumber(7, Maths.Numbers.English);
            WriteNumber(0, Maths.Numbers.English);
            WriteNumber(-12345, Maths.Numbers.English);
            ConsoleLine.End(true);

            // Chinese numbers.
            ConsoleLine.Write(ConsoleColor.Magenta, "Chinese numbers");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.Numbers.Chinese);
            WriteNumber(2305843009213693951, Maths.Numbers.Chinese);
            WriteNumber(1000000001, Maths.Numbers.Chinese);
            WriteNumber(10086, Maths.Numbers.Chinese);
            WriteNumber(9000, Maths.Numbers.Chinese);
            WriteNumber(2018, Maths.Numbers.Chinese);
            WriteNumber(2004, Maths.Numbers.Chinese);
            WriteNumber(1999, Maths.Numbers.Chinese);
            WriteNumber(1024, Maths.Numbers.Chinese);
            WriteNumber(1000, Maths.Numbers.Chinese);
            WriteNumber(240, Maths.Numbers.Chinese);
            WriteNumber(101, Maths.Numbers.Chinese);
            WriteNumber(100, Maths.Numbers.Chinese);
            WriteNumber(17, Maths.Numbers.Chinese);
            WriteNumber(10, Maths.Numbers.Chinese);
            WriteNumber(7, Maths.Numbers.Chinese);
            WriteNumber(0, Maths.Numbers.Chinese);
            WriteNumber(-12345, Maths.Numbers.Chinese);
            ConsoleLine.End(true);

            // Chinese numbers in upper case.
            ConsoleLine.Write(ConsoleColor.Magenta, "Chinese numbers (upper)");
            ConsoleLine.End();
            WriteNumber(9876543210, Maths.Numbers.UpperCaseChinese);
            WriteNumber(2305843009213693951, Maths.Numbers.UpperCaseChinese);
            WriteNumber(100000000000001, Maths.Numbers.UpperCaseChinese);
            WriteNumber(-9876543210, Maths.Numbers.UpperCaseChinese);
            ConsoleLine.End(true);
        }

        private void WriteIsPrime(int value)
        {
            ConsoleLine.Write("{0} is {1}a prime number.", value, Maths.Arithmetic.IsPrime(value) ? string.Empty : "NOT ");
            ConsoleLine.End();
        }

        private async Task WriteIsPrimeAsync(long value, CancellationToken cancellationToken)
        {
            ConsoleLine.Write("{0} is {1}a prime number.", value, await Maths.Arithmetic.IsPrimeAsync(value, cancellationToken) ? string.Empty : "NOT ");
            ConsoleLine.End();
        }

        private void WriteNumber(long value, Maths.IIntegerLocalization localInt)
        {
            ConsoleLine.Write("{0}: {1}; {2}.", value, localInt.ToString(value), localInt.ToString(value, true));
            ConsoleLine.End();
        }
    }
}
