using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trivial.Tasks
{
    class InterceptorVerb : Console.AsyncVerb
    {
        private int checkCount = 0;

        private string value = "a";

        public override string Description => "Interceptor";

        public override async Task ProcessAsync()
        {
            await TestKeyFeaturesAsync();
            System.Console.WriteLine();
            TestEquipartitionTaskAsync();
        }

        private async Task TestKeyFeaturesAsync()
        {
            System.Console.WriteLine("Interceptor testing.");

            System.Console.WriteLine("Debounce");
            var task = new Interceptor<string>(
                v => value = v,
                InterceptorPolicy.Debounce(TimeSpan.FromMilliseconds(200))
                );
            _ = task.InvokeAsync("b");
            await Task.Delay(100);
            _ = task.InvokeAsync("c");
            await Task.Delay(100);
            _ = task.InvokeAsync("d");
            await Task.Delay(100);
            _ = task.InvokeAsync("e");
            Check("a");
            await Task.Delay(300);
            Check("e");
            await task.WaitAsync();
            Check("e");

            System.Console.WriteLine("Throttle");
            task.ResetDuration();
            task.Policy = InterceptorPolicy.Throttle(TimeSpan.FromMilliseconds(800));
            _ = task.InvokeAsync("f");
            await Task.Delay(100);
            _ = task.InvokeAsync("g");
            await Task.Delay(100);
            _ = task.InvokeAsync("h");
            await Task.Delay(100);
            _ = task.InvokeAsync("i");
            await Task.Delay(100);
            Check("f");
            await task.WaitAsync();
            Check("f");

            System.Console.WriteLine("Multiple");
            task.ResetDuration();
            task.Policy = InterceptorPolicy.Mutliple(2, 4, TimeSpan.FromMilliseconds(200));
            _ = task.InvokeAsync("j");
            Check("f");
            await Task.Delay(100);
            _ = task.InvokeAsync("k");
            Check("k");
            await Task.Delay(100);
            _ = task.InvokeAsync("l");
            await Task.Delay(100);
            _ = task.InvokeAsync("m");
            await Task.Delay(100);
            Check("m");
            _ = task.InvokeAsync("n");
            await Task.Delay(100);
            Check("m");
            await task.WaitAsync();
            Check("m");

            System.Console.WriteLine("Times 1");
            task.ResetDuration();
            task.Policy = InterceptorPolicy.Times(2, 3, TimeSpan.FromMilliseconds(200));
            _ = task.InvokeAsync("o");
            Check("m");
            await Task.Delay(100);
            _ = task.InvokeAsync("p");
            Check("m");
            await Task.Delay(100);
            _ = task.InvokeAsync("q");
            await Task.Delay(100);
            _ = task.InvokeAsync("r");
            await Task.Delay(100);
            Check("m");
            _ = task.InvokeAsync("s");
            await Task.Delay(100);
            Check("m");
            await task.WaitAsync();
            Check("m");

            System.Console.WriteLine("Times 2");
            task.ResetDuration();
            task.Policy = InterceptorPolicy.Times(2, 3, TimeSpan.FromMilliseconds(200));
            _ = task.InvokeAsync("t");
            Check("m");
            await Task.Delay(100);
            _ = task.InvokeAsync("u");
            Check("m");
            await Task.Delay(100);
            _ = task.InvokeAsync("v");
            await Task.Delay(100);
            Check("m");
            await task.WaitAsync();
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
