# Task flow

In the `Trivial.Tasks` [namespace](../).

<!-- Start -->

`TaskFlow<T>` provides lightweight observation of a task's result and failure, plus dependent steps. It does not replace `Task<T>`, schedule work, retry failures, or provide its own cancellation mechanism.

## Create and await a flow

```csharp
using System;
using System.Threading.Tasks;
using Trivial.Tasks;

var task = Task.FromResult(21);
var flow = new TaskFlow<int>(task);

// A factory is invoked immediately and exactly once, not when the flow is awaited.
var fromFactory = new TaskFlow<int>(() => Task.FromResult(21));

int value = await flow.ResultAsync(); // Returns the original task.
bool successful = await flow.WaitAsync(); // Synchronizes the flow's observed state.
```

Supply a non-null task or a non-null factory returning a non-null task. A factory exception thrown before returning a task escapes the constructor. An exception from the returned task is observed as a flow failure. A task in `TaskStatus.Created` is not started by the flow; its owner must start it.

| Member | Meaning |
| --- | --- |
| `State` | `Running` while observing, then `Success` or `Failure`. An already-completed task may reach its final state during construction. |
| `IsCompleted` | The flow has observed a terminal state, including failure or cancellation. It does not include handler completion. |
| `IsSuccessful` | The observed state is `Success`. |
| `TaskStatus` | The underlying task's status, which may change before the flow observes it. |
| `IsCanceled` | The underlying task is canceled. Cancellation also gives the flow the state `Failure`. |
| `IsExceptionThrownByPrevious` | The observed exception is a `PreviousTaskException` with a non-null `SourceObject` different from this flow. |

`WaitAsync()` returns `true` on success and otherwise rethrows the underlying exception, including cancellation. `WaitAsync(ignoreException: true)` returns `false` on failure or cancellation without throwing that exception; it does not recover the flow. Await `WaitAsync` before reading the final flow state. `ResultAsync()` returns the original task, so awaiting it alone does not guarantee that the flow's state has been updated.

## Observe results and exceptions

```csharp
var flow = new TaskFlow<int>(Task.FromResult(21));

bool delivered = await flow.ResultToAsync(value =>
{
	Console.WriteLine(value);
});

var failed = new TaskFlow<int>(Task.FromException<int>(
	new ArgumentNullException("input")));

bool handled = await failed.CatchAsync<ArgumentException>(error =>
{
	Console.WriteLine(error.ParamName);
}); // true: derived exception types match by default.

bool exactMatch = await failed.CatchAsync<ArgumentException>(error =>
{
	Console.WriteLine(error.Message);
}, exactType: true); // false: the runtime type is ArgumentNullException.

bool successful = await failed.WaitAsync(ignoreException: true); // Still false.
```

- `ResultToAsync` invokes the handler only on success and returns `true` after it completes; it returns `false` on failure or cancellation.
- `CatchAsync<TException>` invokes the handler only for a matching exception and returns `true` after it completes; it returns `false` on success or a type mismatch. It matches the outer exception, not `InnerException`. Cancellation exceptions can also be observed.
- Both methods accept synchronous actions and asynchronous handlers. Handler exceptions propagate through their returned tasks without changing the source flow's state or result.
- Observers are independent, not sequential transformation steps. Multiple matching observers can run; catching an exception neither consumes it nor prevents a later `WaitAsync()` from throwing.

### Fluent registration and type filtering

`ResultTo` and `Catch` return the same flow immediately, without awaiting their handlers. They do not expose handler exceptions. Prefer `ResultToAsync` and `CatchAsync` when you must await handler completion or handle its errors. Neither `WaitAsync()` nor `ResultAsync()` waits for registered observers.

```csharp
var flow = new TaskFlow<object>(Task.FromResult<object>("ready"));
flow.ResultTo<string>(text => Console.WriteLine(text))
	.ResultTo<int>(number => Console.WriteLine(number)) // Skipped.
	.Catch<Exception>(error => Console.WriteLine(error.Message));

await flow.WaitAsync(); // Waits for the source, not for the callbacks.
```

`ResultTo<TResult>` filters by runtime assignability (`result is TResult`), including base classes and interfaces. Null and incompatible values are skipped. The non-generic success handlers still receive a successful null result.

## Create dependent steps

```csharp
var source = new TaskFlow<int>(Task.FromResult(21));
TaskFlow<int> doubled = source.Then(value => value * 2);
TaskFlow<string> formatted = doubled.Then<string>(value => Task.FromResult($"Value: {value}"));

await formatted.WaitAsync();
string text = await formatted.ResultAsync(); // "Value: 42"

// ThenAsync returns Task<TResult> directly, not TaskFlow<TResult>.
string direct = await source.ThenAsync(value => Task.FromResult(value.ToString()));
```

`Then` creates a new flow. Its handler runs only after the predecessor succeeds, and an asynchronous handler is awaited before the new flow succeeds. A handler's own exception becomes the new flow's failure without being wrapped; it does not change the predecessor.

If the predecessor fails, the handler is skipped and the dependent task fails with `PreviousTaskException`. Its `SourceObject` is the immediately preceding flow and its `InnerException` is that flow's exception. Each additional dependent step adds another wrapper, preserving the failure chain. Catch `PreviousTaskException` on the dependent flow, rather than the root exception type.

```csharp
var source = new TaskFlow<int>(Task.FromException<int>(
	new InvalidOperationException("source failed")));
var next = source.Then(value => value * 2); // Handler is skipped.

await next.CatchAsync<PreviousTaskException>(error =>
{
	Console.WriteLine(ReferenceEquals(source, error.SourceObject)); // true
	Console.WriteLine(error.InnerException.Message); // "source failed"
});

bool successful = await next.WaitAsync(ignoreException: true); // false
```

## Cancellation

Cancel the underlying operation using its own cancellation token. If its task is canceled, dependent steps skip their handlers and propagate cancellation without a `PreviousTaskException` wrapper. Their `IsCanceled` is `true`, `State` is `Failure`, and `IsExceptionThrownByPrevious` is `false`.

A *faulted* task containing an `OperationCanceledException` is different: its `IsCanceled` is `false`, and dependent flows wrap that failure in `PreviousTaskException` like other predecessor failures.
