using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Trivial.Reflection;
using Trivial.Tasks;

namespace Trivial.Sample
{
    class SingletonKeeperVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "Singleton keeper and renew scheduler";

        public override async Task ProcessAsync()
        {
            var count = 0;
            var keeper = new SingletonKeeper<int>(async () =>
            {
                await Task.Delay(50);
                return count += 1;
            });
            using (var refresher = new SingletonRenewScheduler<int>(keeper, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(100)))
            {
                System.Console.WriteLine("Started scheduler.");
                System.Console.WriteLine(count);
                await Task.Delay(100);
                System.Console.WriteLine(count);
                await Task.Delay(100);
                System.Console.WriteLine(count);
                await Task.Delay(100);
                System.Console.WriteLine(count);
                await Task.Delay(100);
                System.Console.WriteLine(count);
                System.Console.WriteLine("Force renew 3 times.");
                Task.WaitAll(refresher.RenewAsync(), refresher.RenewAsync(), refresher.RenewAsync());
                System.Console.WriteLine(count);
                refresher.IsPaused = true;
                System.Console.WriteLine("Paused.");
                await Task.Delay(100);
                System.Console.WriteLine(count);
                await Task.Delay(200);
                System.Console.WriteLine(count);
                refresher.IsPaused = false;
                System.Console.WriteLine("Resumed.");
                await Task.Delay(200);
                System.Console.WriteLine(count);
            }

            await Task.Delay(200);
            System.Console.WriteLine(count);
            System.Console.WriteLine("Disposed scheduler.");
            await Task.Delay(200);
            System.Console.WriteLine(count);
            System.Console.WriteLine("Renew again.");
            await keeper.RenewAsync();
            System.Console.WriteLine(count);
            System.Console.WriteLine("Done!");
        }
    }
}
