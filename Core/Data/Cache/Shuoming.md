# 缓存

在 `Trivial.Data` [命名空间](../) 中。

## 数据缓存集合

可以通过以下方式创建一个具有过期时间和数量限制的数据缓存集合。

```csharp
var cache = new DataCacheCollection<Model>
{
    MaxCount = 1000,
    Expiration = TimeSpan.FromSeconds(100)
};
```

这样就可以从缓存中获取数据（如果存在），并在必要时初始化一个。

```csharp
if (!cache.TryGet("abcd", out item))
{
    item = new Model();
    cache["abcd"] = item;
}
```
