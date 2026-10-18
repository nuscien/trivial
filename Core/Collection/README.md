# Collection

In `Trivial.Collection` namespace.

<!-- Start -->

Provides collection utilities.

## Extensions

You can use the extension methods for the instance of the `IEnumerable<KeyValuePairs<TKey, TValue>>` class and other collection. See `ListExtensions` class.

## Thread-safe collection

- `ConcurrentList<T>` uses a locker to control access to the list.
- `SynchronizedList<T>` uses reader writer lock slim to control access to the list.
