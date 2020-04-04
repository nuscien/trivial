# Singleton and renew

A way to manage the singleton instance.

In `Trivial.Reflection` [namespace](../) of `Trivial.dll` [library](../../).

## Singleton resolver

You can register a set of object or their initialization into `SingletonResolver` for getting any of them in future.

```csharp
// Register a value.
SingletonResolver.Instance.Register<int>(56);

// Get the singleton value.
var num = SingletonResolver.Instance.Resolve<int>(); // num -> 56

// Register a value with a key.
SingletonResolver.Instance.Register<int>("seven", 7);

// Get the singleton value with a key.
num = SingletonResolver.Instance.Resolve<int>("seven"); // num -> 7

// More test.
num = SingletonResolver.Instance.Resolve<int>(); // num -> 56
SingletonResolver.Instance.Register<int>(() => 32);
num = SingletonResolver.Instance.Resolve<int>(); // num -> 32
num = SingletonResolver.Instance.Resolve<int>("seven"); // num -> 7
```

## Singleton keeper

You can create a singleton keeper with the way to get the instance and automatically renew as need.

```csharp
class SampleSingletonKeeper : SingletonKeeper<int>
{
    public SampleSingletonKeeper() : base()
    {
    }

    public TimeSpan Expiration { get; set; } = TimeSpan.FromSeconds(1);

    protected override Task<bool> NeedRenewAsync()
    {
        return Task.FromResult(DateTime.Now - RefreshDate > Expiration);
    }

    protected override async Task<T> ResolveFromSourceAsync()
    {
        return Cache + 1;
    }
}
```

Then you can get the instance anywhere. The renew operation is thread-safe and will occur automatically as need when it expires.

```csharp
var instance = new SampleSingletonKeeper();
var i = await instance.GetAsync();
```

## Singleton renew timer

Since `SingletonKeeper` will auto renew the instance when you call `GetAsync` member method and it expires, you may want to ask the it renew in schedule.

```csharp
// The singleton keeper will renew in every 2s.
var scheduler = new SingletonRenewTimer<int>(instance, TimeSpan.FromSeconds(2)))

// Dispose the scheduler when you do not want it runs any more.
scheduler.Dispose();
```
