# Hit task

You can create a task controller to manage when a handler shoud be raised.

In `Trivial.Tasks` [namespace](./) of `Trivial.dll` library.

### Debounce

You may request to call a specific action several times in a short time but only the last one should be processed and previous ones should be ignored.
A sample scenario is real-time search.

```csharp
var task = HitTask.Debounce(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

### Throttle

You may want to request to call an action only once in a short time even if you request to call several times. The rest will be ignored.

```csharp
var task = HitTask.Throttle(() => {
    // Do something...
}, TimeSpan.FromMilliseconds(10000));

// Somewhere to raise.
task.ProcessAsync();
```

### Times

You can define an action can be only processed only when request to call in the specific times range and others will be ignored.
A sample scenario is double click.

```csharp
var task = HitTask.Times(() => {
    // Do something...
}, 2, 2, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

### Multiple

A handler to process for the specific times and it will be reset after a while.

```csharp
var task = HitTask.Multiple(() => {
    // Do something...
}, 10, null, TimeSpan.FromMilliseconds(200));

// Somewhere to raise.
task.ProcessAsync();
```

