# Retry

You can set up a task and a retry policy so that the task will process and retry automatically expected when it fails.

In `Trivial.Tasks` [namespace](../) of `Trivial.dll` library.

## Linear retry policy

You can create a retry policy to process the specific handler within the specific times with the specific time span between two processing.

```csharp
var retryPolicy = new LinearRetryPolicy(3, TimeSpan.FromSeconds(10));

// So following task will throw an InvalidOperationException but it will retry 3 times with 10s interval.
var proccessResult = retryPolicy.ProcessAsync(() =>
{
    throw new InvalidOperationException();
}, typeof(InvalidOperationException));
```

If you want to create a retry policy that the waiting time increases per processing, you can set the 3rd argument in the constructor for the time span of the increment.

## Customized retry policy

You can also create a customized retry policy.

```csharp
var retryPolicy = new CustomizedRetryPolicy(retryRecord =>
{
    // This function need return the time span for waiting for retry; or null, to stop retry.
    return retryRecord.Length < 6 ? TimeSpan.FromSeconds(retryRecord.Length) : null;
});
// This example is as same as following.
// var retryPolicy = new LinearRetryPolicy(5, 0, TimeSpan.FromSeconds(1));
```

## Customized exception handler

Since you may customize some exception handlers instead to set a list of exception type for catching, you can code like below. P.S.: You need use namespace `Trivial.Reflection` firstly.

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
