# 缓存

在 `Trivial.Data` [命名空间](../) 中。

<!-- Start -->

## 数据缓存集合

可以通过以下方式创建一个具有过期时间和数量限制的数据缓存集合。

```csharp
var cache = new DataCacheCollection<Model>(1000, TimeSpan.FromMinutes(20));
```

这样就可以从缓存中获取数据（如果存在），并在必要时初始化一个。

```csharp
if (!cache.TryGet("abcd", out item))
{
    item = new Model();
    cache["abcd"] = item;
}
```

## 带命名空间的缓存集合

如果需要在不同的命名空间中缓存数据，用于对同一类型的缓存进行分组隔离，可以使用 `NamespacedDataCacheCollection` 类型，其在添加和获取时，需要传入命名空间参数，从而避免命名冲突。

