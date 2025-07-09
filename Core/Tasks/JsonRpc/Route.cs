using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Reflection;

namespace Trivial.Tasks;

/// <summary>
/// The server route of JSON-RPC request handler.
/// </summary>
public class JsonRpcRequestRoute
{
    private Dictionary<string, JsonRpcRequestHandler> handlers = new();

    ///// <summary>
    ///// Gets a value indicating whether the method name is case-insensitive.
    ///// </summary>
    //public bool CaseInsensitive { get; set; }

    /// <summary>
    /// Gets all method names registerred.
    /// </summary>
    public IEnumerable<string> Methods => handlers.Keys;

    /// <summary>
    /// Registers a handler to process JSON-PRC request.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="description">The optional description of the handler.</param>
    /// <exception cref="ArgumentNullException">method is null.</exception>
    public void Register(string method, Func<JsonRpcRequestObject, BaseJsonRpcResponseObject> handler, string description = null)
    {
        if (method == null) throw ObjectConvert.ArgumentNull(nameof(method));
        if (handler == null) handlers.Remove(method);
        else handlers[method] = new Internal1JsonRpcRequestHandler(handler, description);
    }

    /// <summary>
    /// Registers a handler to process JSON-PRC request.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="description">The optional description of the handler.</param>
    /// <exception cref="ArgumentNullException">method is null.</exception>
    public void Register(string method, Func<JsonRpcRequestObject, Task<BaseJsonRpcResponseObject>> handler, string description = null)
    {
        if (method == null) throw ObjectConvert.ArgumentNull(nameof(method));
        if (handler == null) handlers.Remove(method);
        else handlers[method] = new Internal2JsonRpcRequestHandler(handler, description);
    }

    /// <summary>
    /// Registers a handler to process JSON-PRC request.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="handler">The handler.</param>
    /// <param name="description">The optional description of the handler.</param>
    /// <exception cref="ArgumentNullException">method is null.</exception>
    public void Register(string method, Func<JsonRpcRequestObject, CancellationToken, Task<BaseJsonRpcResponseObject>> handler, string description = null)
    {
        if (method == null) throw ObjectConvert.ArgumentNull(nameof(method));
        if (handler == null) handlers.Remove(method);
        else handlers[method] = new Internal3JsonRpcRequestHandler(handler, description);
    }

    /// <summary>
    /// Registers a handler to process JSON-PRC request.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="handler">The handler.</param>
    /// <exception cref="ArgumentNullException">method is null.</exception>
    public void Register(string method, JsonRpcRequestHandler handler)
    {
        if (method == null) throw ObjectConvert.ArgumentNull(nameof(method));
        if (handler == null)
        {
            handlers.Remove(method);
            return;
        }

        handlers[method] = handler;
        handler.OnRegisterInternal(this, method);
    }

    /// <summary>
    /// Gets the handler by given method name.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <returns>The handler registerred under the specific method name; or null, if not found.</returns>
    public JsonRpcRequestHandler GetHandler(string method)
    {
        if (method == null) return null;
        if (handlers.TryGetValue(method, out var handler)) return handler;
        return null;
    }

    /// <summary>
    /// Removes a handler.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string method)
        => method != null && handlers.Remove(method);

    /// <summary>
    /// Removes a set of handler.
    /// </summary>
    /// <param name="methods">The method name.</param>
    /// <returns>The count of items removed.</returns>
    public int Remove(IEnumerable<string> methods)
    {
        var i = 0;
        foreach (var method in methods)
        {
            if (Remove(method)) i++;
        }

        return i;
    }

#if NETCOREAPP
    /// <summary>
    /// Removes a handler.
    /// </summary>
    /// <param name="method">The name of the method to be invoked.</param>
    /// <param name="handler">The handler removed; or null, if not found.</param>
    /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
    public bool Remove(string method, out JsonRpcRequestHandler handler)
    {
        if (method == null)
        {
            handler = null;
            return false;
        }
        
        return handlers.Remove(method, out handler);
    }
#endif

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object.</returns>
    public async Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, CancellationToken cancellationToken = default)
    {
        if (request?.Method == null) return null;
        if (!handlers.TryGetValue(request.Method, out var handler)) return null;
        return await handler.ProcessInternalAsync(request, this, cancellationToken);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object collection.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object collection.</returns>
    public async IAsyncEnumerable<BaseJsonRpcResponseObject> ProcessAsync(IEnumerable<JsonRpcRequestObject> request, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (request == null) yield break;
        foreach (var req in request)
        {
            yield return await ProcessAsync(req, cancellationToken);
        }
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object.</returns>
    public Task<BaseJsonRpcResponseObject> ProcessAsync(string request, CancellationToken cancellationToken = default)
    {
        var req = JsonSerializer.Deserialize<JsonRpcRequestObject>(request);
        return ProcessAsync(req, cancellationToken);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object collection.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object collection.</returns>
    public IAsyncEnumerable<BaseJsonRpcResponseObject> ProcessBatchAsync(string request, CancellationToken cancellationToken = default)
    {
        var req = JsonSerializer.Deserialize<IEnumerable<JsonRpcRequestObject>>(request);
        return ProcessAsync(req, cancellationToken);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object.</returns>
    public Task<BaseJsonRpcResponseObject> ProcessAsync(Stream request, CancellationToken cancellationToken = default)
    {
        var req = JsonSerializer.Deserialize<JsonRpcRequestObject>(request);
        return ProcessAsync(req, cancellationToken);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object collection.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object collection.</returns>
    public IAsyncEnumerable<BaseJsonRpcResponseObject> ProcessBatchAsync(Stream request, CancellationToken cancellationToken = default)
    {
        var req = JsonSerializer.Deserialize<IEnumerable<JsonRpcRequestObject>>(request);
        return ProcessAsync(req, cancellationToken);
    }
}

/// <summary>
/// The event arguments used by processing a JSON-RPC request and returning the response.
/// </summary>
/// <param name="route">The handler route.</param>
/// <param name="request">The request object.</param>
/// <param name="response">The response object.</param>
public sealed class JsonRpcRequestHandlerEventArgs(JsonRpcRequestRoute route, JsonRpcRequestObject request, BaseJsonRpcResponseObject response) : EventArgs
{
    /// <summary>
    /// The handler route.
    /// </summary>
    public JsonRpcRequestRoute Route { get; } = route;

    /// <summary>
    /// The JSON-RPC request object.
    /// </summary>
    public JsonRpcRequestObject Request { get; } = request;

    /// <summary>
    /// The JSON-RPC response object.
    /// </summary>
    public BaseJsonRpcResponseObject Response { get; } = response;
}

/// <summary>
/// The handler to process JSON-RPC request object to resolve response object.
/// </summary>
public abstract class JsonRpcRequestHandler
{
    /// <summary>
    /// Adds or removes an event handler occurred on the handler is registerred into a route.
    /// </summary>
    public event SourcePropertyEventHandler<JsonRpcRequestRoute, string> Registerred;

    /// <summary>
    /// Adds or removes an event handler occurred on the handler is processed.
    /// </summary>
    public event EventHandler<JsonRpcRequestHandlerEventArgs> Processed;

    /// <summary>
    /// Initializes a new instance of the JsonRpcRequestHandler class.
    /// </summary>
    protected JsonRpcRequestHandler()
    {
    }

    /// <summary>
    /// Initializes a new instance of the JsonRpcRequestHandler class.
    /// </summary>
    /// <param name="description">The description of the handler.</param>
    protected JsonRpcRequestHandler(string description)
    {
        Description = description;
    }

    /// <summary>
    /// Gets or sets the description of the handler.
    /// </summary>
    public string Description { get; protected set; }

    /// <summary>
    /// Occurs on the registering this handler into a route.
    /// </summary>
    /// <param name="route">The handler route.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    protected virtual void OnRegister(JsonRpcRequestRoute route, string method)
    {
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object.</param>
    /// <param name="route">The handler route.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object.</returns>
    protected abstract Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default);

    /// <summary>
    /// Occurs on the registering this handler into a route.
    /// </summary>
    /// <param name="route">The handler route.</param>
    /// <param name="method">The name of the method to be invoked.</param>
    internal void OnRegisterInternal(JsonRpcRequestRoute route, string method)
    {
        OnRegister(route, method);
        Registerred?.Invoke(this, route, method);
    }

    /// <summary>
    /// Processes.
    /// </summary>
    /// <param name="request">The JSON-RPC request object.</param>
    /// <param name="route">The handler route.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The JSON-RPC response object.</returns>
    internal async Task<BaseJsonRpcResponseObject> ProcessInternalAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
    {
        var response = await ProcessAsync(request, route, cancellationToken);
        Processed?.Invoke(this, new(route, request, response));
        return response;
    }
}

internal class Internal1JsonRpcRequestHandler(Func<JsonRpcRequestObject, BaseJsonRpcResponseObject> handler, string description) : JsonRpcRequestHandler(description)
{
    /// <inheritdoc />
    protected async override Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
    {
        if (handler == null) return null;
        await Task.CompletedTask;
        return handler?.Invoke(request);
    }
}

internal class Internal2JsonRpcRequestHandler(Func<JsonRpcRequestObject, Task<BaseJsonRpcResponseObject>> handler, string description) : JsonRpcRequestHandler(description)
{
    /// <inheritdoc />
    protected async override Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
    {
        if (handler == null) return null;
        return await handler?.Invoke(request);
    }
}

internal class Internal3JsonRpcRequestHandler(Func<JsonRpcRequestObject, CancellationToken, Task<BaseJsonRpcResponseObject>> handler, string description) : JsonRpcRequestHandler(description)
{
    /// <inheritdoc />
    protected async override Task<BaseJsonRpcResponseObject> ProcessAsync(JsonRpcRequestObject request, JsonRpcRequestRoute route, CancellationToken cancellationToken = default)
    {
        if (handler == null) return null;
        return await handler?.Invoke(request, cancellationToken);
    }
}
