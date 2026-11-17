# 异常处理

在 `Trivial.Reflection` [命名空间](../)中。

<!-- Start -->

可以按照自己的需求处理异常。

## 自定义异常处理器

由于可能希望自定义一些异常处理器，而不是设置一个异常类型列表来捕获，可以像下面这样编写代码。

```csharp
// 创建一个实例。
var exceptionHanlder = new ExceptionHandler();

// 注册一个异常类型以捕获而不进行详细检查。
exceptionHandler.Add<InvalidOperationException>();

// 为要捕获的异常注册一个处理程序。
// 该处理程序将指示是否需要捕获此异常，
// 并在需要抛出时返回异常，
// 即使返回的异常不是参数中的那个。
exceptionHandler.Add<ArgumentException>(ex =>
{
    return ex.ParamName == "name" ? null : ex;
});

// 使用示例。
var proccessResult = retryPolicy.ProcessAsync(() =>
{
    // 在这里执行某些操作。
}, exceptionHandler.GetException);
```

这个逻辑将像 try-catch 一样逐个捕获异常。
