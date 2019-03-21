using System;
using Trivial.Console;

namespace Trivial.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dispatcher = new Dispatcher();
            dispatcher.Register<SelectionVerb>("selection");
            dispatcher.Register<HitTasksVerb>("hittask");
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
                dispatcher.Process(true);
                System.Console.WriteLine("Bye!");
            }
        }
    }
}
