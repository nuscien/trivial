# 重试

在 `Trivial.Tasks` [命名空间](../) 中。

<!-- Start -->

你可以设置一个任务和重试策略，以便在任务失败时自动处理和重试。

## 线性重试策略

你可以创建一个重试策略，在指定的次数内以指定的时间间隔处理特定的处理程序。

```csharp
var retryPolicy = new LinearRetryPolicy(3, TimeSpan.FromSeconds(10));

// 所以下面的任务将抛出一个 InvalidOperationException，但它会重试 3 次，每次间隔 10 秒。
var proccessResult = retryPolicy.ProcessAsync(() =>
{
    throw new InvalidOperationException();
}, typeof(InvalidOperationException));
```

如果你想创建一个等待时间随每次处理增加的重试策略，可以在构造函数中设置第三个参数，表示增量的时间间隔。

## 自定义重试策略

你也可以创建一个自定义的重试策略。

```csharp
var retryPolicy = new CustomizedRetryPolicy(retryRecord =>
{
    // 这个函数需要返回等待重试的时间间隔；或者返回 null 来停止重试。
    return retryRecord.Length < 6 ? TimeSpan.FromSeconds(retryRecord.Length) : null;
});
// 这个例子与下面的例子相同。
// var retryPolicy = new LinearRetryPolicy(5, 0, TimeSpan.FromSeconds(1));
```

## 自定义异常处理程序

由于你可能希望自定义一些异常处理程序，而不是设置一个异常类型列表来捕获，你可以像下面这样编写代码。注意：你需要首先使用命名空间 `Trivial.Reflection`。

```csharp
// 创建一个实例。
var exceptionHanlder = new ExceptionHandler();

// 注册一个异常类型以捕获，不进行详细检查。
exceptionHandler.Add<InvalidOperationException>();

// 注册一个异常处理程序以捕获异常。
// 处理程序将指示是否需要捕获此异常，
// 并在需要抛出时返回异常，
// 即使返回的异常不是参数中的异常。
exceptionHandler.Add<ArgumentException>(ex =>
{
    return ex.ParamName == "name" ? null : ex;
});

// Usage.
var proccessResult = retryPolicy.ProcessAsync(() =>
{
    // Do something here.
}, exceptionHandler.GetException);
```

它会像 try-catch 一样逐个捕获这些异常。
