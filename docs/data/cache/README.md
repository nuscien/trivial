# Cache

Data cache.

In `Trivial.Data` [namespace](../) of `Trivial.dll` [library](../../).

## Data cache collection

You can create a collection to cache data with expiration and count limitation by following way.

```csharp
var cache = new DataCacheCollection<Model>
{
    MaxCount = 1000,
    Expiration = TimeSpan.FromSeconds(100)
};
```

So that you can get the data from the cache if has, and initialize one if necessary.

```csharp
if (!cache.TryGet("abcd", out item))
{
    item = new Model();
    cache["abcd"] = item;
}
```
