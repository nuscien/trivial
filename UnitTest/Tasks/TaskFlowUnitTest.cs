using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Trivial.Tasks;

/// <summary>
/// Tests task flow observation, handlers, and dependent flows.
/// </summary>
[TestClass]
public class TaskFlowUnitTest
{
    /// <summary>
    /// Tests both constructors and the transition from running to success.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestSuccessfulTaskAsync(bool useFactory)
    {
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var factoryCalls = 0;
        var flow = useFactory ? new TaskFlow<int>(() =>
        {
            factoryCalls++;
            return source.Task;
        }) : new TaskFlow<int>(source.Task);
        Assert.AreEqual(useFactory ? 1 : 0, factoryCalls);
        Assert.AreSame(source.Task, flow.ResultAsync());
        Assert.AreEqual(TaskStatus.WaitingForActivation, flow.TaskStatus);
        Assert.AreEqual(TaskFlowStates.Running, flow.State);
        Assert.IsFalse(flow.IsCompleted);
        Assert.IsFalse(flow.IsSuccessful);
        Assert.IsFalse(flow.IsCanceled);
        Assert.IsFalse(flow.IsExceptionThrownByPrevious);

        var wait = flow.WaitAsync();
        var received = 0;
        var observe = flow.ResultToAsync((Action<int>)(value => received = value));
        Assert.IsFalse(wait.IsCompleted);
        Assert.IsFalse(observe.IsCompleted);
        source.SetResult(42);

        Assert.IsTrue(await wait);
        Assert.IsTrue(await observe);
        Assert.AreEqual(42, received);
        Assert.AreEqual(42, await flow.ResultAsync());
        Assert.AreEqual(TaskFlowStates.Success, flow.State);
        Assert.AreEqual(TaskStatus.RanToCompletion, flow.TaskStatus);
        Assert.IsTrue(flow.IsCompleted);
        Assert.IsTrue(flow.IsSuccessful);
        Assert.IsFalse(flow.IsCanceled);
        Assert.IsTrue(await flow.WaitAsync(true));
        Assert.AreEqual(useFactory ? 1 : 0, factoryCalls);
        Assert.IsFalse(await flow.CatchAsync<Exception>((Action<Exception>)(_ => Assert.Fail("Success must not invoke a catch handler."))));
        Assert.IsFalse(await flow.CatchAsync<Exception>(_ => Task.FromException(new InvalidOperationException())));
    }

    /// <summary>
    /// Tests that synchronous factory errors escape construction.
    /// </summary>
    [TestMethod]
    public void TestFactoryException()
    {
        var error = new InvalidOperationException("factory");
        var actual = Assert.ThrowsExactly<InvalidOperationException>(() => new TaskFlow<int>((Func<Task<int>>)(() => throw error)));
        Assert.AreSame(error, actual);
    }

    /// <summary>
    /// Tests immediate and deferred failure, repeated waits, and skipped success handlers.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestFailedTaskAsync(bool alreadyCompleted)
    {
        var error = new InvalidOperationException("source");
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (alreadyCompleted) source.SetException(error);
        var flow = new TaskFlow<int>(source.Task);
        var observe = flow.ResultToAsync((Action<int>)(_ => Assert.Fail("Failure must not invoke a result handler.")));
        if (!alreadyCompleted) source.SetException(error);

        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.AreEqual(TaskFlowStates.Failure, flow.State);
        Assert.AreEqual(TaskStatus.Faulted, flow.TaskStatus);
        Assert.IsTrue(flow.IsCompleted);
        Assert.IsFalse(flow.IsSuccessful);
        Assert.IsFalse(flow.IsCanceled);
        Assert.IsFalse(flow.IsExceptionThrownByPrevious);
        Assert.IsFalse(await observe);
        Assert.IsFalse(await flow.ResultToAsync(_ => Task.FromException(new Exception("must not run"))));
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.WaitAsync()));
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.ResultAsync()));
        Assert.IsTrue(await flow.CatchAsync<InvalidOperationException>((Action<InvalidOperationException>)(ex => Assert.AreSame(error, ex))));
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.WaitAsync()));
    }

    /// <summary>
    /// Tests that asynchronous success observers are awaited independently of the source flow.
    /// </summary>
    [TestMethod]
    public async Task TestResultHandlerCompletionAsync()
    {
        var flow = new TaskFlow<int>(Task.FromResult(42));
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var received = 0;
        var observer = flow.ResultToAsync(async value =>
        {
            await release.Task;
            received = value;
        });
        Assert.IsTrue(await flow.WaitAsync());
        Assert.IsFalse(observer.IsCompleted);
        release.SetResult(true);
        Assert.IsTrue(await observer);
        Assert.AreEqual(42, received);
    }

    /// <summary>
    /// Tests fluent success registration and independent asynchronous callback completion.
    /// </summary>
    [TestMethod]
    public async Task TestFluentResultHandlersAsync()
    {
        var flow = new TaskFlow<int>(Task.FromResult(42));
        var received = 0;
        Assert.AreSame(flow, flow.ResultTo((Action<int>)(value => received = value)));
        Assert.AreEqual(42, received);
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var handled = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        Assert.AreSame(flow, flow.ResultTo(async value =>
        {
            await release.Task;
            handled.SetResult(value);
        }));
        Assert.IsTrue(await flow.WaitAsync());
        Assert.IsFalse(handled.Task.IsCompleted);
        release.SetResult(true);
        Assert.AreEqual(42, await handled.Task);
    }

    /// <summary>
    /// Tests runtime type filtering, including null and incompatible successful results.
    /// </summary>
    [TestMethod]
    [DataRow("value", true)]
    [DataRow(42, false)]
    [DataRow(null, false)]
    public async Task TestTypedResultHandlersAsync(object result, bool matches)
    {
        var flow = new TaskFlow<object>(Task.FromResult(result));
        var calls = 0;
        Assert.AreSame(flow, flow.ResultTo<string>((Action<string>)(value =>
        {
            Assert.AreEqual(result, value);
            calls++;
        })));
        Assert.AreSame(flow, flow.ResultTo<string>(value =>
        {
            Assert.AreEqual(result, value);
            calls++;
            return Task.CompletedTask;
        }));
        Assert.AreEqual(matches ? 2 : 0, calls);
        Assert.IsTrue(await flow.ResultToAsync((Action<object>)(value => Assert.AreEqual(result, value))));
        Assert.IsTrue(await flow.WaitAsync());

        var failed = new TaskFlow<object>(Task.FromException<object>(new InvalidOperationException()));
        Assert.AreSame(failed, failed.ResultTo<string>((Action<string>)(_ => calls++)));
        Assert.AreSame(failed, failed.ResultTo<string>(_ =>
        {
            calls++;
            return Task.CompletedTask;
        }));
        Assert.IsFalse(await failed.WaitAsync(true));
        Assert.AreEqual(matches ? 2 : 0, calls);
    }

    /// <summary>
    /// Tests that typed result handlers accept assignable interface types, not only exact types.
    /// </summary>
    [TestMethod]
    public void TestAssignableResultHandlers()
    {
        var flow = new TaskFlow<object>(Task.FromResult<object>("value"));
        var calls = 0;
        flow.ResultTo<IComparable>((Action<IComparable>)(value =>
        {
            Assert.AreEqual(0, value.CompareTo("value"));
            calls++;
        }));
        flow.ResultTo<IComparable>(value =>
        {
            Assert.AreEqual(0, value.CompareTo("value"));
            calls++;
            return Task.CompletedTask;
        });
        Assert.AreEqual(2, calls);
    }

    /// <summary>
    /// Tests exact and assignable exception matching for both awaitable handler overloads.
    /// </summary>
    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public async Task TestCatchMatchingAsync(bool exactType, bool asynchronous)
    {
        var error = new ArgumentNullException("value");
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var flow = new TaskFlow<int>(source.Task);
        var calls = 0;
        Action<ArgumentException> handler = ex =>
        {
            Assert.AreSame(error, ex);
            calls++;
        };
        var observation = asynchronous
            ? flow.CatchAsync<ArgumentException>(ex =>
            {
                handler(ex);
                return Task.CompletedTask;
            }, exactType)
            : flow.CatchAsync(handler, exactType);
        Assert.IsFalse(observation.IsCompleted);
        source.SetException(error);
        Assert.AreEqual(!exactType, await observation);
        Assert.AreEqual(exactType ? 0 : 1, calls);
        Assert.IsTrue(await flow.CatchAsync<ArgumentNullException>((Action<ArgumentNullException>)(ex => Assert.AreSame(error, ex)), true));
        Assert.IsTrue(await flow.CatchAsync<ArgumentNullException>(ex =>
        {
            Assert.AreSame(error, ex);
            return Task.CompletedTask;
        }, true));
        Assert.IsFalse(await flow.CatchAsync<InvalidOperationException>((Action<InvalidOperationException>)(_ => Assert.Fail("Wrong exception type."))));
        Assert.IsFalse(await flow.CatchAsync<InvalidOperationException>(_ => Task.FromException(new Exception("must not run"))));
    }

    /// <summary>
    /// Tests fluent catch registration, exact matching, and non-consuming exception observation.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestFluentCatchHandlersAsync(bool exactType)
    {
        var error = new ArgumentNullException("value");
        var flow = new TaskFlow<int>(Task.FromException<int>(error));
        var calls = 0;
        Assert.AreSame(flow, flow.Catch<ArgumentException>((Action<ArgumentException>)(ex =>
        {
            Assert.AreSame(error, ex);
            calls++;
        }), exactType));
        Assert.AreSame(flow, flow.Catch<ArgumentException>(ex =>
        {
            Assert.AreSame(error, ex);
            calls++;
            return Task.CompletedTask;
        }, exactType));
        Assert.AreEqual(exactType ? 0 : 2, calls);
        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => flow.WaitAsync()));
    }

    /// <summary>
    /// Tests that catch completion does not recover the failed source flow.
    /// </summary>
    [TestMethod]
    public async Task TestCatchHandlerCompletionAsync()
    {
        var error = new InvalidOperationException("source");
        var flow = new TaskFlow<int>(Task.FromException<int>(error));
        var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Exception received = null;
        var observer = flow.CatchAsync<Exception>(async ex =>
        {
            await release.Task;
            received = ex;
        });
        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.IsFalse(observer.IsCompleted);
        release.SetResult(true);
        Assert.IsTrue(await observer);
        Assert.AreSame(error, received);
        Assert.AreEqual(TaskFlowStates.Failure, flow.State);
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.WaitAsync()));
    }

    /// <summary>
    /// Tests that observer errors are returned to their callers without replacing the source outcome.
    /// </summary>
    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    [DataRow(true, true)]
    public async Task TestObserverExceptionsAsync(bool failedSource, bool asynchronous)
    {
        var sourceError = new InvalidOperationException("source");
        var handlerError = new ApplicationException("handler");
        var flow = new TaskFlow<int>(failedSource ? Task.FromException<int>(sourceError) : Task.FromResult(42));
        Task<bool> observation;
        if (failedSource)
        {
            observation = asynchronous
                ? flow.CatchAsync<Exception>(_ => Task.FromException(handlerError))
                : flow.CatchAsync<Exception>((Action<Exception>)(_ => throw handlerError));
        }
        else
        {
            observation = asynchronous
                ? flow.ResultToAsync(_ => Task.FromException(handlerError))
                : flow.ResultToAsync((Action<int>)(_ => throw handlerError));
        }

        Assert.AreSame(handlerError, await Assert.ThrowsExactlyAsync<ApplicationException>(() => observation));
        Assert.AreEqual(!failedSource, await flow.WaitAsync(true));
        Assert.IsFalse(flow.IsExceptionThrownByPrevious);
        if (failedSource)
            Assert.AreSame(sourceError, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.WaitAsync()));
        else
            Assert.AreEqual(42, await flow.ResultAsync());
    }

    /// <summary>
    /// Tests synchronous and asynchronous result transformations, including multiple dependent steps.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestThenSuccessAsync(bool asynchronous)
    {
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var flow = new TaskFlow<int>(source.Task);
        var calls = 0;
        Func<int, string> transform = value =>
        {
            calls++;
            return (value + 1).ToString();
        };
        var next = asynchronous
            ? flow.Then<string>(value => Task.FromResult(transform(value)))
            : flow.Then(transform);
        var last = next.Then(value => value.Length);
        Assert.AreNotSame<object>(flow, next);
        Assert.AreEqual(TaskFlowStates.Running, next.State);
        Assert.AreEqual(0, calls);
        source.SetResult(41);
        Assert.IsTrue(await last.WaitAsync());
        Assert.AreEqual(2, await last.ResultAsync());
        Assert.AreEqual("42", await next.ResultAsync());
        Assert.AreEqual(41, await flow.ResultAsync());
        Assert.AreEqual(1, calls);
        Assert.IsTrue(next.IsSuccessful);
        Assert.IsFalse(next.IsExceptionThrownByPrevious);
    }

    /// <summary>
    /// Tests that Then and ThenAsync await the transformation task rather than only the predecessor.
    /// </summary>
    [TestMethod]
    public async Task TestAsynchronousTransformationCompletionAsync()
    {
        var flow = new TaskFlow<int>(Task.FromResult(42));
        var result = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
        var calls = 0;
        Func<int, Task<string>> transform = value =>
        {
            Assert.AreEqual(42, value);
            calls++;
            return result.Task;
        };
        var next = flow.Then(transform);
        var direct = flow.ThenAsync(transform);
        Assert.AreEqual(2, calls);
        Assert.AreEqual(TaskFlowStates.Running, next.State);
        Assert.IsFalse(direct.IsCompleted);
        Assert.IsTrue(await flow.WaitAsync());
        result.SetResult("done");
        Assert.AreEqual("done", await direct);
        Assert.IsTrue(await next.WaitAsync());
        Assert.AreEqual("done", await next.ResultAsync());
    }

    /// <summary>
    /// Tests local handler failure without a predecessor wrapper.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestThenHandlerFailureAsync(bool asynchronous)
    {
        var error = new InvalidOperationException("handler");
        var flow = new TaskFlow<int>(Task.FromResult(42));
        var next = asynchronous
            ? flow.Then<string>(_ => Task.FromException<string>(error))
            : flow.Then((Func<int, string>)(_ => throw error));
        Assert.IsFalse(await next.WaitAsync(true));
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => next.WaitAsync()));
        Assert.IsFalse(next.IsExceptionThrownByPrevious);
        Assert.IsFalse(next.IsCanceled);
        Assert.IsTrue(await flow.WaitAsync());
        Assert.AreSame(error, await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => flow.ThenAsync<string>(_ => Task.FromException<string>(error))));
    }

    /// <summary>
    /// Tests skipped transformations and the source and nesting of predecessor exceptions.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestPreviousFailureAsync(bool asynchronous)
    {
        var error = new InvalidOperationException("source");
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var flow = new TaskFlow<int>(source.Task);
        var calls = 0;
        Func<int, int> transform = value =>
        {
            calls++;
            return value + 1;
        };
        var next = asynchronous ? flow.Then<int>(value => Task.FromResult(transform(value))) : flow.Then(transform);
        var direct = flow.ThenAsync<int>(value => Task.FromResult(transform(value)));
        source.SetException(error);
        var wrapped = await Assert.ThrowsExactlyAsync<PreviousTaskException>(() => next.WaitAsync());
        Assert.AreSame(flow, wrapped.SourceObject);
        Assert.AreSame(error, wrapped.InnerException);
        Assert.IsTrue(next.IsExceptionThrownByPrevious);
        Assert.IsFalse(next.IsCanceled);
        Assert.IsFalse(await next.CatchAsync<InvalidOperationException>((Action<InvalidOperationException>)(_ => Assert.Fail("Must match the outer exception."))));
        Assert.IsTrue(await next.CatchAsync<PreviousTaskException>((Action<PreviousTaskException>)(ex => Assert.AreSame(wrapped, ex))));
        var directError = await Assert.ThrowsExactlyAsync<PreviousTaskException>(() => direct);
        Assert.AreSame(flow, directError.SourceObject);
        Assert.AreSame(error, directError.InnerException);
        var last = next.Then(transform);
        var nested = await Assert.ThrowsExactlyAsync<PreviousTaskException>(() => last.WaitAsync());
        Assert.AreSame(next, nested.SourceObject);
        Assert.AreSame(wrapped, nested.InnerException);
        Assert.IsTrue(last.IsExceptionThrownByPrevious);
        Assert.AreEqual(0, calls);
    }

    /// <summary>
    /// Tests cancellation observation and propagation without predecessor exception wrappers.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestCancellationAsync(bool asynchronous)
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var flow = new TaskFlow<int>(Task.FromCanceled<int>(cancellation.Token));
        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.AreEqual(TaskFlowStates.Failure, flow.State);
        Assert.AreEqual(TaskStatus.Canceled, flow.TaskStatus);
        Assert.IsTrue(flow.IsCompleted);
        Assert.IsFalse(flow.IsSuccessful);
        Assert.IsTrue(flow.IsCanceled);
        Assert.IsFalse(flow.IsExceptionThrownByPrevious);
        var error = await Assert.ThrowsAsync<OperationCanceledException>(() => flow.WaitAsync());
        Assert.AreEqual(cancellation.Token, error.CancellationToken);
        Assert.AreEqual(cancellation.Token, (await Assert.ThrowsAsync<OperationCanceledException>(() => flow.ResultAsync())).CancellationToken);
        var catches = 0;
        Assert.IsTrue(await flow.CatchAsync<OperationCanceledException>((Action<OperationCanceledException>)(ex =>
        {
            Assert.AreEqual(cancellation.Token, ex.CancellationToken);
            catches++;
        })));
        Assert.AreEqual(1, catches);
        Assert.IsFalse(await flow.ResultToAsync((Action<int>)(_ => Assert.Fail("Canceled result handler."))));
        Assert.IsFalse(await flow.ResultToAsync(_ => Task.FromException(new Exception("must not run"))));
        var calls = 0;
        Func<int, int> transform = value =>
        {
            calls++;
            return value;
        };
        var next = asynchronous ? flow.Then<int>(value => Task.FromResult(transform(value))) : flow.Then(transform);
        var propagated = await Assert.ThrowsAsync<OperationCanceledException>(() => next.WaitAsync());
        Assert.AreEqual(cancellation.Token, propagated.CancellationToken);
        Assert.IsTrue(next.IsCanceled);
        Assert.IsFalse(next.IsExceptionThrownByPrevious);
        Assert.IsFalse(await next.WaitAsync(true));
        await Assert.ThrowsAsync<OperationCanceledException>(() => flow.ThenAsync<int>(value => Task.FromResult(transform(value))));
        Assert.AreEqual(0, calls);
    }

    /// <summary>
    /// Tests that a faulted task containing a cancellation exception is not treated as a canceled task.
    /// </summary>
    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task TestFaultedCancellationExceptionAsync(bool asynchronous)
    {
        var error = new OperationCanceledException("fault, not cancellation");
        var flow = new TaskFlow<int>(Task.FromException<int>(error));
        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.AreEqual(TaskStatus.Faulted, flow.TaskStatus);
        Assert.IsFalse(flow.IsCanceled);
        var next = asynchronous ? flow.Then<int>(value => Task.FromResult(value)) : flow.Then(value => value);
        var wrapped = await Assert.ThrowsExactlyAsync<PreviousTaskException>(() => next.WaitAsync());
        Assert.AreSame(error, wrapped.InnerException);
        Assert.AreSame(flow, wrapped.SourceObject);
        Assert.IsTrue(next.IsExceptionThrownByPrevious);
        Assert.IsFalse(next.IsCanceled);
    }

    /// <summary>
    /// Tests predecessor-source detection for manually supplied exceptions.
    /// </summary>
    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    public async Task TestPreviousExceptionSourceAsync(int sourceKind)
    {
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var flow = new TaskFlow<int>(source.Task);
        var inner = new InvalidOperationException("inner");
        var origin = sourceKind == 1 ? flow : new object();
        var error = sourceKind == 0 ? new PreviousTaskException("previous", inner) : new PreviousTaskException(origin, "previous", inner);
        source.SetException(error);
        Assert.IsFalse(await flow.WaitAsync(true));
        Assert.AreEqual(sourceKind == 2, flow.IsExceptionThrownByPrevious);
        Assert.AreEqual("previous", error.Message);
        Assert.AreSame(inner, error.InnerException);
        Assert.AreSame(sourceKind == 0 ? null : origin, error.SourceObject);
    }
}
