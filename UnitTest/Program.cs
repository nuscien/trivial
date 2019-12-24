using System;
using System.Threading.Tasks;

using Trivial.Console;

namespace Trivial.UnitTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("Trivial Sample");
            System.Console.WriteLine();

            var dispatcher = new Dispatcher();
            dispatcher.Register<Console.SelectionVerb>("selection");
            dispatcher.Register<Tasks.HitTasksVerb>("hittask");
            dispatcher.Register<Reflection.SingletonKeeperVerb>("singleton");
            dispatcher.Register<Maths.MathsVerb>("maths");
            dispatcher.Register<Security.TokenClientVerb>("token");
            dispatcher.Register<Net.HttpClientVerb>("http");
            dispatcher.Register<Tasks.RetryVerb>("retry");
            dispatcher.Register<Text.CsvVerb>("csv");
            dispatcher.RegisterHelp();
            dispatcher.RegisterExit();
            if (args.Length > 0)
            {
                dispatcher.Process(args);
            }
            else
            {
                System.Console.WriteLine("Type 'help' to get help.");
                await dispatcher.ProcessAsync(true);
                System.Console.WriteLine("Bye!");
            }
        }
    }
}
