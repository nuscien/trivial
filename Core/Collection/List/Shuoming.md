# 集合

在 `Trivial.Collection` [命名空间](../)中。

<!-- Start -->

提供集合实用工具。

## 集合扩展

你可以使用 `IEnumerable<KeyValuePairs<TKey, TValue>>` 类实例和其他集合的扩展方法。请参阅 `ListExtensions` 类。

## 线程安全集合

- `ConcurrentList<T>` 使用锁来控制对列表的访问。
- `SynchronizedList<T>` 使用读写锁来控制对列表的访问。
