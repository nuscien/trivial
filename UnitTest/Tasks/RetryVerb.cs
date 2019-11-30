using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Net;

namespace Trivial.UnitTest.Tasks
{
    public class RetryVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "Retry";

        public override async Task ProcessAsync()
        {
            ConsoleLine.WriteLine("Retry 1");
            ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            var retry = new LinearRetryPolicy(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(100));
            await retry.ProcessAsync(() =>
            {
                ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                throw new ApplicationException();
            }, typeof(ApplicationException));

            ConsoleLine.WriteLine("Retry 2");
            ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                await retry.ProcessAsync(() =>
                {
                    ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                    throw new InvalidOperationException();
                }, typeof(ApplicationException));
            }
            catch (InvalidOperationException)
            {
            }

            var h = new ExceptionHandler();
            h.Add<ApplicationException>(ex =>
            {
                return null;
            });
            h.Add<ArgumentException>(ex =>
            {
                if (ex.Message == "name") return null;
                return new ApplicationException();
            });

            ConsoleLine.WriteLine("Retry 3");
            ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            await retry.ProcessAsync(() =>
            {
                ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                throw new ApplicationException();
            }, h.GetException);

            ConsoleLine.WriteLine("Retry 4");
            ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            await retry.ProcessAsync(() =>
            {
                ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                throw new ArgumentException("name");
            }, h.GetException);

            ConsoleLine.WriteLine("Retry 5");
            ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                await retry.ProcessAsync(() =>
                {
                    ConsoleLine.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                    throw new ArgumentException("other");
                }, h.GetException);
            }
            catch (ApplicationException)
            {
            }
        }
    }
}
