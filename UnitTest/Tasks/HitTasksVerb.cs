using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    class HitTasksVerb : Console.AsyncVerb
    {
        private int checkCount = 0;

        private string value = "a";

        public override string Description => "Hit tasks";

        public override async Task ProcessAsync()
        {
            await TestKeyFeaturesAsync();
            System.Console.WriteLine();
            TestEquipartitionTaskAsync();
        }

        private async Task TestKeyFeaturesAsync()
        {
            System.Console.WriteLine("Hit task testing.");
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

        private void TestEquipartitionTaskAsync()
        {
            System.Console.WriteLine("Equipartition task testing.");
            var tasks = new EquipartitionTaskContainer();
            tasks.Created += (sender, ev) =>
            {
                if (ev.NewValue == null)
                {
                    System.Console.WriteLine("N/A");
                    return;
                }

                System.Console.WriteLine($"{ev.NewValue.Description ?? ev.NewValue.Id}\t{ev.Key}\tCreated");
            };
            var task1 = tasks.Create("a", "o", 5, "task1");
            WriteLine(task1);
            var task2 = tasks.Create("a", "p", 1, "task2");
            WriteLine(task2);
            var task3 = tasks.Create("b", "q", 2, "task3");
            WriteLine(task3);

            System.Console.WriteLine("Pick from " + (task1.Description ?? task1.Id));
            var f = task1.Pick();
            f = task1.Pick();
            WriteLine(task1);
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            WriteLine(task1);
            f = task1.Pick();
            WriteLine(task1);
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Failure);
            WriteLine(task1);
            System.Console.WriteLine(f.State);
            f = task1.Pick();
            System.Console.WriteLine(f.State);
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Fatal);
            System.Console.WriteLine(f.State);
            WriteLine(task1);
            WriteLine(task2);
            WriteLine(task3);

            System.Console.WriteLine("Pick from " + (task3.Description ?? task3.Id));
            f = task3.Pick();
            f = task3.Pick();
            WriteLine(task1);
            WriteLine(task2);
            WriteLine(task3);
            var fStr = f.ToJsonString();
            System.Console.WriteLine(fStr);
            f = EquipartitionTask.Fragment.Parse(fStr);
            fStr = f.ToQueryData().ToString();
            System.Console.WriteLine(fStr);
            f = EquipartitionTask.Fragment.Parse(fStr);
            task1.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            WriteLine(task1);
            WriteLine(task2);
            WriteLine(task3);
            task3.UpdateFragment(f, EquipartitionTask.FragmentStates.Success);
            WriteLine(task1);
            WriteLine(task2);
            WriteLine(task3);
            f = task3.Pick();
            System.Console.WriteLine("No more fragment in task3.");
            if (f != null)
            {
                System.Console.WriteLine("Error!");
                return;
            }

            task3.UpdateFragment(task3[0], EquipartitionTask.FragmentStates.Success);
            WriteLine(task1);
            WriteLine(task2);
            WriteLine(task3);

            System.Console.WriteLine("Done!");
        }

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

        private void WriteLine(EquipartitionTask t)
        {
            System.Console.WriteLine($"{t.Description ?? t.Id}\t{t.JobId}\t{t.GetProcessingFragments().Count()} + {t.GetDoneFragments().Count()} / {t.Count}");
        }
    }
}
