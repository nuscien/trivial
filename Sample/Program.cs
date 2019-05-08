using System;
using System.Threading.Tasks;

using Trivial.Console;

namespace Trivial.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Register<SelectionVerb>("selection");
            dispatcher.Register<HitTasksVerb>("hittask");
            dispatcher.Register<SingletonKeeperVerb>("singleton");
            dispatcher.Register<MathsVerb>("maths");
            dispatcher.Register<TokenClientVerb>("token");
            dispatcher.Register<HttpClientVerb>("http");
            dispatcher.Register<RetryVerb>("retry");
            dispatcher.Register<CsvVerb>("csv");
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
