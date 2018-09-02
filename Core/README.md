This library includes task utilities.

# Tasks

Just add following namespace to your code file to use.

```csharp
using Trivial.Tasks;
```

## Debounce

Maybe a handler will be asked to process several times in a short time but you just want to process once at the last time because the previous ones are obsolete, e.g. real-time search. You can use following method to do so.

```csharp
var task = HitTask.debound(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

## Throttle

A handler to be frozen for a while after it has processed.

```csharp
var task = HitTask.throttle(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(10000));

// Somewhere to raise.
task.ProcessAsync();
```

## Multiple

A handler to process for the specific times and it will be reset after a while.

```csharp
var task = HitTask.multiple(() => {
    // Do something...
}, 10, null, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

## Times

A handler to process for the specific times only and it will be reset after a while, e.g. double click.

```csharp
var task = HitTask.times(() => {
    // Do something...
}, 2, 2, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

# Mathematics

Just add following namespace to your code file to use.

```csharp
using Trivial.Maths;
```

# Security

Just add following namespace to your code file to use.

```csharp
using Trivial.Security;
```
