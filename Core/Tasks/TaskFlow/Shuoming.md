# 任务流

在 `Trivial.Tasks` [命名空间](../) 中。

<!-- Start -->

`TaskFlow<T>` 用于轻量级管理异步任务的执行结果、异常和后续流程。它不替代 `Task<T>`，也不负责调度、重试或主动取消任务。

## 创建与等待

```csharp
using System;
using System.Threading.Tasks;
using Trivial.Tasks;

var task = Task.FromResult(21);
var flow = new TaskFlow<int>(task);

// 工厂在构造时立即调用一次，而不是在等待时调用。
var fromFactory = new TaskFlow<int>(() => Task.FromResult(21));

int value = await flow.ResultAsync(); // 返回原始任务。
bool successful = await flow.WaitAsync(); // 等待流程观察到任务完成。
```

请传入非空任务，或返回非空任务的非空工厂。工厂在返回任务之前同步抛出的异常会直接从构造函数抛出；返回的任务发生的异常才会被流程记录。对于状态为 `TaskStatus.Created` 的任务，流程不会自动启动它，应由任务的拥有者启动。

| 成员 | 含义 |
| --- | --- |
| `State` | 观察期间为 `Running`，结束后为 `Success` 或 `Failure`。传入已完成的任务时，可能在构造期间就进入最终状态。 |
| `IsCompleted` | 流程已观察到成功、失败或取消，不代表观察回调也已执行完成。 |
| `IsSuccessful` | 已观察到成功完成，即 `State` 为 `Success`。 |
| `TaskStatus` | 原始任务的状态，可能先于流程的状态更新。 |
| `IsCanceled` | 原始任务是否被取消。取消时流程的 `State` 也为 `Failure`。 |
| `IsExceptionThrownByPrevious` | 记录的异常是否为 `PreviousTaskException`，且其 `SourceObject` 非空并且不是当前流程。 |

`WaitAsync()` 成功时返回 `true`，失败或取消时重新抛出原始异常。`WaitAsync(ignoreException: true)` 会抑制该异常并返回 `false`，但不会使流程恢复成功。读取最终流程状态前，应先等待 `WaitAsync`。`ResultAsync()` 返回原始任务，仅等待它不能保证流程状态已经更新。

## 观察结果和异常

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
}); // true：默认也匹配派生异常。

bool exactMatch = await failed.CatchAsync<ArgumentException>(error =>
{
	Console.WriteLine(error.Message);
}, exactType: true); // false：实际类型为 ArgumentNullException。

bool successful = await failed.WaitAsync(ignoreException: true); // 仍为 false。
```

- `ResultToAsync` 仅在成功时调用处理程序，等待其完成后返回 `true`；原始任务失败或取消时不调用，返回 `false`。
- `CatchAsync<TException>` 仅在异常类型匹配时调用处理程序，等待其完成后返回 `true`；成功或类型不匹配时返回 `false`。匹配的是外层异常，不会搜索 `InnerException`。取消异常也可以被观察。
- 两者均支持同步和异步处理程序。处理程序自身的异常通过返回的任务传播，不会改变原流程的状态和结果。
- 观察回调相互独立，不构成顺序转换步骤。多个匹配的处理程序均可执行；观察异常不会消费异常，也不会阻止后续 `WaitAsync()` 再次抛出异常。

### 链式注册和结果类型筛选

`ResultTo` 和 `Catch` 返回当前流程本身，不等待处理程序完成，也不向调用者暴露处理程序的异常。需要等待回调或处理回调异常时，应使用 `ResultToAsync` 和 `CatchAsync`。`WaitAsync()` 和 `ResultAsync()` 均不会等待这些观察回调。

```csharp
var flow = new TaskFlow<object>(Task.FromResult<object>("ready"));
flow.ResultTo<string>(text => Console.WriteLine(text))
	.ResultTo<int>(number => Console.WriteLine(number)) // 跳过。
	.Catch<Exception>(error => Console.WriteLine(error.Message));

await flow.WaitAsync(); // 等待原任务，不等待回调。
```

`ResultTo<TResult>` 使用运行时类型兼容性进行筛选（`result is TResult`），支持基类和接口；结果为 `null` 或类型不兼容时会跳过。非泛型的成功处理程序仍会收到成功返回的 `null`。

## 创建后续流程

```csharp
var source = new TaskFlow<int>(Task.FromResult(21));
TaskFlow<int> doubled = source.Then(value => value * 2);
TaskFlow<string> formatted = doubled.Then<string>(value => Task.FromResult($"Value: {value}"));

await formatted.WaitAsync();
string text = await formatted.ResultAsync(); // "Value: 42"

// ThenAsync 直接返回 Task<TResult>，而不是 TaskFlow<TResult>。
string direct = await source.ThenAsync(value => Task.FromResult(value.ToString()));
```

`Then` 返回新的流程，仅在前序成功时执行转换；异步转换完成后新流程才会成功。转换程序自身抛出的异常直接成为新流程的失败原因，不会被包装，也不会影响前序流程。

如果前序失败，转换程序不会执行，后续任务以 `PreviousTaskException` 失败。其 `SourceObject` 指向紧邻的前序流程，`InnerException` 保存该前序的异常。继续追加步骤会逐层包装，保留异常链。因此应在后续流程中捕获 `PreviousTaskException`，而不是直接匹配最初的异常类型。

```csharp
var source = new TaskFlow<int>(Task.FromException<int>(
	new InvalidOperationException("source failed")));
var next = source.Then(value => value * 2); // 不会执行转换。

await next.CatchAsync<PreviousTaskException>(error =>
{
	Console.WriteLine(ReferenceEquals(source, error.SourceObject)); // true
	Console.WriteLine(error.InnerException.Message); // "source failed"
});

bool successful = await next.WaitAsync(ignoreException: true); // false
```

## 取消

通过底层操作自身的取消令牌取消任务。若原始任务确实处于取消状态，后续流程会跳过处理程序，直接传播取消而不包装为 `PreviousTaskException`。后续流程的 `IsCanceled` 为 `true`，`State` 为 `Failure`，`IsExceptionThrownByPrevious` 为 `false`。

注意，处于失败状态但异常为 `OperationCanceledException` 的任务并不等于已取消任务：此时 `IsCanceled` 为 `false`，后续流程仍像处理其他前序失败一样包装为 `PreviousTaskException`。
