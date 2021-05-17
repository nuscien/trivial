using System;
using System.Threading.Tasks;

namespace Trivial.CommandLine
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine("Trivial Sample");
            Console.WriteLine();

            var dispatcher = new CommandDispatcher();
            dispatcher.Register<Tasks.InterceptorVerb>("interceptor");
            dispatcher.Register<Reflection.SingletonKeeperVerb>("singleton");
            dispatcher.Register<Maths.MathsVerb>("maths");
            dispatcher.Register<Security.TokenClientVerb>("token");
            dispatcher.Register<Net.HttpClientVerb>("http");
            dispatcher.Register<Tasks.RetryVerb>("retry");
            dispatcher.Register<Text.CsvVerb>("csv");
            dispatcher.Register<Chemistry.ChemicalElementVerb>("chemistry");
            return dispatcher.ProcessAsync(args);
        }
    }
}
