using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Trivial.Reflection;
using Trivial.Security;
using Trivial.Tasks;

namespace Trivial.UnitTest.Reflection
{
    class SingletonKeeperVerb : Trivial.Console.AsyncVerb
    {
        public override string Description => "Singleton keeper and renew scheduler";

        public override async Task ProcessAsync()
        {
            var singletonManager = new SingletonResolver();
            singletonManager.Register<Net.HttpClientVerb.NameAndDescription>();
            var num = 12;
            singletonManager.Register(num);
            singletonManager.Register("one", new Lazy<int>(() => 1));
            if (!singletonManager.TryResolve<Net.HttpClientVerb.NameAndDescription>(out _)) ConsoleLine.WriteLine("Failed to resolve.");
            if (singletonManager.TryResolve("one", out num)) ConsoleLine.WriteLine("Got! " + num);
            if (singletonManager.TryResolve(out num)) ConsoleLine.WriteLine("Got! " + num);
            ConsoleLine.WriteLine("Got! " + singletonManager.Resolve(typeof(int), "one"));
            ConsoleLine.WriteLine();

            var count = 0;
            var keeper = new SingletonKeeper<int>(async () =>
            {
                await Task.Delay(50);
                return count += 1;
            });
            using (var refresher = new SingletonRenewTimer<int>(keeper, TimeSpan.FromMilliseconds(100)))
            {
                ConsoleLine.WriteLine("Started scheduler.");
                ConsoleLine.WriteLine(count);
                await Task.Delay(100);
                ConsoleLine.WriteLine(count);
                await Task.Delay(100);
                ConsoleLine.WriteLine(count);
                await Task.Delay(100);
                ConsoleLine.WriteLine(count);
                ConsoleLine.WriteLine("Force renew 3 times.");
                Task.WaitAll(refresher.RenewAsync(), refresher.RenewAsync(), refresher.RenewAsync());
                ConsoleLine.WriteLine(count);
                refresher.IsPaused = true;
                ConsoleLine.WriteLine("Paused.");
                await Task.Delay(100);
                ConsoleLine.WriteLine(count);
                await Task.Delay(200);
                ConsoleLine.WriteLine(count);
                refresher.IsPaused = false;
                ConsoleLine.WriteLine("Resumed.");
                await Task.Delay(200);
                ConsoleLine.WriteLine(count);
            }

            await Task.Delay(200);
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Disposed scheduler.");
            await Task.Delay(200);
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Renew again.");
            await keeper.RenewAsync();
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Disable renew by way 1.");
            keeper.FreezeRenew(TimeSpan.FromMilliseconds(100));
            await keeper.RenewIfCanAsync();
            ConsoleLine.WriteLine(count);
            await Task.Delay(200);
            await keeper.RenewIfCanAsync();
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Disable renew by way 2.");
            keeper.LockRenewSpan = TimeSpan.FromMilliseconds(100);
            await keeper.RenewIfCanAsync();
            ConsoleLine.WriteLine(count);
            await Task.Delay(200);
            await keeper.RenewIfCanAsync();
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Disable renew by way 3.");
            keeper.IsRenewDisabled = true;
            await keeper.RenewIfCanAsync();
            ConsoleLine.WriteLine(count);
            ConsoleLine.WriteLine("Done!");
        }
    }
}
