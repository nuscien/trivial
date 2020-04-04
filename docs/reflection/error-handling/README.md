# Error handling

You can handle the exception as what you want.

In `Trivial.Reflection` [namespace](../) of `Trivial.dll` [library](../../).

## Customized exception handler

Since you may customize some exception handlers instead to set a list of exception type for catching, you can code like below. P.S.: You need use namespace `Trivial.Tasks` firstly.

```csharp
// Create an instance.
var exceptionHanlder = new ExceptionHandler();

// Register an exception type to catch without details check.
exceptionHandler.Add<InvalidOperationException>();

// Register a handler for an exception to catch.
// The handler will indicate whether this exception need catch,
// and return the exception if need throw,
// even if the exception returned is not the one in parameter.
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

It will catch the exception by these one by one like try-catch.
