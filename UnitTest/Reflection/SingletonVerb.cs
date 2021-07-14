using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Reflection
{
    class SingletonKeeperVerb : CommandLine.BaseCommandVerb
    {
        public static string Description => "Singleton keeper and renew scheduler";

        protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
        {
            var singletonManager = new SingletonResolver();
            singletonManager.Register<Net.HttpClientVerb.NameAndDescription>();
            var num = 12;
            singletonManager.Register(num);
            singletonManager.Register("one", new Lazy<int>(() => 1));
            if (!singletonManager.TryResolve<Net.HttpClientVerb.NameAndDescription>(out _)) Console.WriteLine("Failed to resolve.");
            if (singletonManager.TryResolve("one", out num)) Console.WriteLine("Got! " + num);
            if (singletonManager.TryResolve(out num)) Console.WriteLine("Got! " + num);
            Console.WriteLine("Got! " + singletonManager.Resolve(typeof(int), "one"));
            Console.WriteLine();

            var count = 0;
            var keeper = new SingletonKeeper<int>(async () =>
            {
                await Task.Delay(50);
                return count += 1;
            });
            var refresher = new SingletonRenewTimer<int>(keeper, TimeSpan.FromMilliseconds(100));
            Console.WriteLine("Started scheduler.");
            Console.WriteLine(count);
            await Task.Delay(100);
            Console.WriteLine(count);
            await Task.Delay(100);
            Console.WriteLine(count);
            await Task.Delay(100);
            Console.WriteLine(count);
            Console.WriteLine("Force renew 3 times.");
            Task.WaitAll(refresher.RenewAsync(), refresher.RenewAsync(), refresher.RenewAsync());
            Console.WriteLine(count);
            refresher.IsPaused = true;
            Console.WriteLine("Paused.");
            await Task.Delay(100);
            Console.WriteLine(count);
            await Task.Delay(200);
            Console.WriteLine(count);
            refresher.IsPaused = false;
            Console.WriteLine("Resumed.");
            await Task.Delay(200);
            Console.WriteLine(count);

            await Task.Delay(200);
            Console.WriteLine(count);
            Console.WriteLine("Disposed scheduler.");
            await Task.Delay(200);
            Console.WriteLine(count);
            Console.WriteLine("Renew again.");
            await keeper.RenewAsync();
            Console.WriteLine(count);
            Console.WriteLine("Disable renew by way 1.");
            keeper.FreezeRenew(TimeSpan.FromMilliseconds(100));
            await keeper.RenewIfCanAsync();
            Console.WriteLine(count);
            await Task.Delay(200);
            await keeper.RenewIfCanAsync();
            Console.WriteLine(count);
            Console.WriteLine("Disable renew by way 2.");
            keeper.LockRenewSpan = TimeSpan.FromMilliseconds(100);
            await keeper.RenewIfCanAsync();
            Console.WriteLine(count);
            await Task.Delay(200);
            await keeper.RenewIfCanAsync();
            Console.WriteLine(count);
            Console.WriteLine("Disable renew by way 3.");
            keeper.IsRenewDisabled = true;
            await keeper.RenewIfCanAsync();
            Console.WriteLine(count);
            Console.WriteLine("Done!");
        }
    }
}
