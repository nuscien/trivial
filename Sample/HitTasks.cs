using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trivial.Tasks;

namespace Trivial.Sample
{
    class HitTasksVerb : Trivial.Console.AsyncVerb
    {
        private int checkCount = 0;

        private string value = "a";

        public override string Description => "Hit tasks";

        private void ReadArgument(HitTask<string> sender, HitEventArgs<string> ev)
        {
            value = ev.Argument;
        }

        private void Check(string expect, string message = null)
        {
            checkCount++;
            if (expect == value) return;
            throw new InvalidOperationException("#" + checkCount + " (" + value + " != " + expect + ") " + (message ?? string.Empty));
        }

        public override async Task ProcessAsync()
        {
            var taskTokens = new List<Task>();
            var action = (HitEventHandler<string>)ReadArgument;
            System.Console.WriteLine("Debounce");
            var task = HitTask.Debounce(action, TimeSpan.FromMilliseconds(200));
            taskTokens.Add(task.ProcessAsync("b"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("c"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("d"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("e"));
            Check("a");
            await Task.Delay(300);
            Check("e");
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
            Check("e");

            System.Console.WriteLine("Throttle");
            task = HitTask.Throttle(action, TimeSpan.FromMilliseconds(800));
            taskTokens.Add(task.ProcessAsync("f"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("g"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("h"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("i"));
            await Task.Delay(100);
            Check("f");
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
            Check("f");

            System.Console.WriteLine("Multiple");
            task = HitTask.Mutliple(action, 2, 4, TimeSpan.FromMilliseconds(200));
            taskTokens.Add(task.ProcessAsync("j"));
            Check("f");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("k"));
            Check("k");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("l"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("m"));
            await Task.Delay(100);
            Check("m");
            taskTokens.Add(task.ProcessAsync("n"));
            await Task.Delay(100);
            Check("m");
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
            Check("m");

            System.Console.WriteLine("Times 1");
            task = HitTask.Times(action, 2, 3, TimeSpan.FromMilliseconds(200));
            taskTokens.Add(task.ProcessAsync("o"));
            Check("m");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("p"));
            Check("m");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("q"));
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("r"));
            await Task.Delay(100);
            Check("m");
            taskTokens.Add(task.ProcessAsync("s"));
            await Task.Delay(100);
            Check("m");
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
            Check("m");

            System.Console.WriteLine("Times 2");
            task = HitTask.Times(action, 2, 3, TimeSpan.FromMilliseconds(200));
            taskTokens.Add(task.ProcessAsync("t"));
            Check("m");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("u"));
            Check("m");
            await Task.Delay(100);
            taskTokens.Add(task.ProcessAsync("v"));
            await Task.Delay(100);
            Check("m");
            Task.WaitAll(taskTokens.ToArray());
            taskTokens.Clear();
            Check("v");
        }
    }
}
