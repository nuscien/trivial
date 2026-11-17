# Singleton and renew

在 `Trivial.Reflection` [命名空间](../)中。

<!-- Start -->

一种管理单例实例的方式。

## 单例解析器

可以将一组对象或它们的初始化注册到 `SingletonResolver` 中，以便将来获取它们。

```csharp
// 注册一个值。
SingletonResolver.Instance.Register<int>(56);

// 获取单例值。
var num = SingletonResolver.Instance.Resolve<int>(); // num -> 56

// 使用键注册一个值。
SingletonResolver.Instance.Register<int>("seven", 7);

// 使用键获取单例值。
num = SingletonResolver.Instance.Resolve<int>("seven"); // num -> 7

// 更多测试。
num = SingletonResolver.Instance.Resolve<int>(); // num -> 56
SingletonResolver.Instance.Register<int>(() => 32);
num = SingletonResolver.Instance.Resolve<int>(); // num -> 32
num = SingletonResolver.Instance.Resolve<int>("seven"); // num -> 7
```

## 单例管理器

可以创建一个单例管理器，通过它可以获取实例并在需要时自动更新。

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

然后可以在任何地方获取实例。更新操作是线程安全的，并且在过期时会根据需要自动发生。

```csharp
var instance = new SampleSingletonKeeper();
var i = await instance.GetAsync();
```

## 单例更新定时器

由于 `SingletonKeeper` 会在调用 `GetAsync` 成员方法并且实例过期时自动更新实例，可能希望让它按计划更新。

```csharp
// 单例管理器将每隔 2 秒更新一次。
var scheduler = new SingletonRenewTimer<int>(instance, TimeSpan.FromSeconds(2));

// 当不希望它再运行时，释放调度器。
scheduler.Dispose();
```
