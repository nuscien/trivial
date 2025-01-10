using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;

namespace Trivial.Tasks;

class RetryVerb : CommandLine.BaseCommandVerb
{
    public static string Description => "Retry";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Retry 1");
        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
        var retry = new LinearRetryPolicy(3, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(100));
        await retry.ProcessAsync(() =>
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            throw new ApplicationException();
        }, typeof(ApplicationException));

        Console.WriteLine("Retry 2");
        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
        try
        {
            await retry.ProcessAsync(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
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

        Console.WriteLine("Retry 3");
        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
        await retry.ProcessAsync(() =>
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            throw new ApplicationException();
        }, h.GetException);

        Console.WriteLine("Retry 4");
        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
        await retry.ProcessAsync(() =>
        {
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
            throw new ArgumentException("name");
        }, h.GetException);

        Console.WriteLine("Retry 5");
        Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
        try
        {
            await retry.ProcessAsync(() =>
            {
                Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff"));
                throw new ArgumentException("other");
            }, h.GetException);
        }
        catch (ApplicationException)
        {
        }
    }
}
