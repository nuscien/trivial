# Cache

In `Trivial.Data` [namespace](../).

<!-- Start -->

Data cache.

## Data cache collection

You can create a collection to cache data with expiration and count limitation by following way.

```csharp
var cache = new DataCacheCollection<Model>(1000, TimeSpan.FromMinutes(20));
```

So that you can get the data from the cache if has, and initialize one if necessary.

```csharp
if (!cache.TryGet("abcd", out item))
{
    item = new Model();
    cache["abcd"] = item;
}
```

## Namespaced data cache collection

If you need to cache data in different namespaces for grouping and isolation of the same type of cache, you can use the `NamespacedDataCacheCollection` type, which requires a namespace parameter when adding and getting, thus avoiding naming conflicts.
