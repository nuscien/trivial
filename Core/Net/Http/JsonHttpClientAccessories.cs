using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.Reflection;
using Trivial.Tasks;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Net;

/// <summary>
/// The maker to create JSON HTTP client.
/// </summary>
public interface IJsonHttpClientMaker
{
    /// <summary>
    /// Creates a JSON HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of response.</typeparam>
    /// <param name="callback">An optional callback raised on data received.</param>
    /// <returns>A new JSON HTTP client.</returns>
    JsonHttpClient<T> Create<T>(Action<ReceivedEventArgs<T>> callback = null);
}

/// <summary>
/// The Collection for JSON HTTP client.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public interface IJsonHttpClientCache<T>
{
    /// <summary>
    /// Tries to get the Collection.
    /// It will be used only for the request with GET method.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="authorization">The authentication header value.</param>
    /// <param name="cache">The Collection.</param>
    /// <returns>true if contains Collection; otherwise, false.</returns>
    public bool TryGet(Uri uri, AuthenticationHeaderValue authorization, out T cache);

    /// <summary>
    /// Occurs on the message sending.
    /// This is used to check and clean up the Collection expired.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="method">The HTTP method.</param>
    /// <remarks>
    /// The related Collection should be cleaned up if a modification or deletion action of the resource is sent.
    /// </remarks>
    public void OnSend(Uri uri, HttpMethod method);

    /// <summary>
    /// Occurs on receive data.
    /// It will be used only for the request with GET method.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="authorization">The authentication header value.</param>
    /// <param name="headers">The response headers.</param>
    /// <param name="body">The response body.</param>
    public void OnReceive(Uri uri, AuthenticationHeaderValue authorization, HttpResponseHeaders headers, T body);
}

/// <summary>
/// The event arguments on sending.
/// </summary>
/// <param name="requestMessage">The HTTP request message.</param>
public class SendingEventArgs(HttpRequestMessage requestMessage) : EventArgs
{
    /// <summary>
    /// Gets the HTTP request message.
    /// </summary>
    public HttpRequestMessage RequestMessage { get; } = requestMessage;

    /// <summary>
    /// Gets the request URI.
    /// </summary>
    public Uri RequestUri => RequestMessage?.RequestUri;

    /// <summary>
    /// Gets the HTTP request method.
    /// </summary>
    public HttpMethod Method => RequestMessage.Method ?? HttpMethod.Get;

    /// <summary>
    /// Gets the HTTP version used.
    /// </summary>
    public Version HttpVersion => RequestMessage.Version;
}

/// <summary>
/// The event arguments on received.
/// </summary>
public class ReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="ev">The event arguments.</param>
    public ReceivedEventArgs(ReceivedEventArgs ev)
    {
        if (ev == null) return;
        Header = ev.Header;
        ReasonPhrase = ev.ReasonPhrase;
        IsSuccessStatusCode = ev.IsSuccessStatusCode;
        StatusCode = ev.StatusCode;
        Content = ev.Content;
        Version = ev.Version;
        RequestMessage = ev.RequestMessage;
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="responseMessage">The HTTP response message.</param>
    public ReceivedEventArgs(HttpResponseMessage responseMessage)
    {
        if (responseMessage == null) return;
        Header = responseMessage.Headers;
        ReasonPhrase = responseMessage.ReasonPhrase;
        IsSuccessStatusCode = responseMessage.IsSuccessStatusCode;
        StatusCode = responseMessage.StatusCode;
        Content = responseMessage.Content;
        Version = responseMessage.Version;
        RequestMessage = responseMessage.RequestMessage;
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="statusCode">The status code of the HTTP response.</param>
    /// <param name="content">The content of a HTTP response message.</param>
    /// <param name="header">The collection of HTTP response headers.</param>
    public ReceivedEventArgs(HttpStatusCode statusCode, HttpContent content = null, HttpResponseHeaders header = null)
    {
        StatusCode = statusCode;
        Content = content;
        Header = header;
        var statusInt = (int)statusCode;
        IsSuccessStatusCode = statusInt >= 200 && statusInt < 300;
    }

    /// <summary>
    /// Gets the collection of HTTP response headers.
    /// </summary>
    public HttpResponseHeaders Header { get; }

    /// <summary>
    /// Gets the reason phrase which typically is sent by servers together with the status code.
    /// </summary>
    public string ReasonPhrase { get; }

    /// <summary>
    /// Gets a value that indicates if the HTTP response was successful.
    /// </summary>
    public bool IsSuccessStatusCode { get; }

    /// <summary>
    /// Gets the status code of the HTTP response.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Gets the content of a HTTP response message.
    /// </summary>
    public HttpContent Content { get; }

    /// <summary>
    /// Gets the HTTP message version.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Gets the request message which led to this response message.
    /// </summary>
    public HttpRequestMessage RequestMessage { get; }
}

/// <summary>
/// The event arguments on received.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class ReceivedEventArgs<T> : ReceivedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="ev">The event arguments.</param>
    public ReceivedEventArgs(object result, ReceivedEventArgs ev) : base(ev)
    {
        if (result == null) return;
        try
        {
            Result = (T)result;
        }
        catch (InvalidCastException)
        {
        }
    }

    /// <summary>
    /// Initializes a new instance of the ReceivedEventArgs class.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <param name="responseMessage">The HTTP response message.</param>
    public ReceivedEventArgs(T result, HttpResponseMessage responseMessage) : base(responseMessage)
    {
        Result = result;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    public T Result { get; }

    /// <summary>
    /// Converts to a specific received event arguments.
    /// </summary>
    /// <typeparam name="U">The type of result.</typeparam>
    /// <returns>A received data event arguments.</returns>
    public ReceivedEventArgs<U> ConvertTo<U>()
        => new(Result, this);
}

/// <summary>
/// The Collection of JSON HTTP client.
/// </summary>
/// <typeparam name="T">The type of the result.</typeparam>
public class JsonHttpClientCache<T> : IJsonHttpClientCache<T>
{
    private string authorization;

    /// <summary>
    /// Initializes a new instance of the JsonHttpClientCache class.
    /// </summary>
    public JsonHttpClientCache()
    {
        Collection = new();
    }

    /// <summary>
    /// Initializes a new instance of the JsonHttpClientCache class.
    /// </summary>
    /// <param name="collection">The inner store.</param>
    public JsonHttpClientCache(DataCacheCollection<T> collection)
    {
        Collection = collection ?? new();
    }

    /// <summary>
    /// Gets the cache collection.
    /// </summary>
    public DataCacheCollection<T> Collection { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the Collection is read-only.
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Tries to get the Collection.
    /// It will be used only for the request with GET method.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="authorization">The authentication header value.</param>
    /// <param name="cache">The Collection.</param>
    /// <returns>true if contains Collection; otherwise, false.</returns>
    public bool TryGet(Uri uri, AuthenticationHeaderValue authorization, out T cache)
    {
        if (string.IsNullOrWhiteSpace(uri?.OriginalString))
        {
            cache = default;
            return false;
        }

        return Collection.TryGet(uri.OriginalString, out cache);
    }

    /// <summary>
    /// Removes all Collection.
    /// </summary>
    public void Clear()
        => Collection.Clear();

    /// <summary>
    /// Occurs on the message sending.
    /// This is used to check and clean up the Collection expired.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="method">The HTTP method.</param>
    /// <remarks>
    /// The related Collection should be cleaned up if a modification or deletion action of the resource is sent.
    /// </remarks>
    void IJsonHttpClientCache<T>.OnSend(Uri uri, HttpMethod method)
    {
        var m = method?.Method?.Trim();
        if (string.IsNullOrEmpty(m) || string.IsNullOrWhiteSpace(uri?.OriginalString)) return;
        switch (m.ToUpperInvariant())
        {
            case "DELETE":
            case "PATCH":
            case "POST":
            case "PUT":
                Collection.Remove(uri.OriginalString);
                break;
        }
    }

    /// <summary>
    /// Tests if the HTTP response need Collection.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="authorization">The authentication header value.</param>
    /// <param name="headers">The response headers.</param>
    /// <returns>true if Collection; otherwise, false.</returns>
    protected virtual bool NeedCache(Uri uri, AuthenticationHeaderValue authorization, HttpResponseHeaders headers)
        => true;

    /// <summary>
    /// Occurs on receive data.
    /// It will be used only for the request with GET method.
    /// </summary>
    /// <param name="uri">The request URI.</param>
    /// <param name="authorization">The authentication header value.</param>
    /// <param name="headers">The response headers.</param>
    /// <param name="body">The response body.</param>
    void IJsonHttpClientCache<T>.OnReceive(Uri uri, AuthenticationHeaderValue authorization, HttpResponseHeaders headers, T body)
    {
        if (IsReadOnly || string.IsNullOrWhiteSpace(uri?.OriginalString)) return;
        var auth = this.authorization;
        this.authorization = authorization?.Parameter;
        if (authorization?.Parameter != auth) Collection.Clear();
        var control = headers?.CacheControl;
        if (control == null || control.NoStore)
        {
            Collection.Add(uri.OriginalString, body);
            return;
        }

        var expire = control.MaxAge;
        Collection.Add(uri.OriginalString, body, expire);
    }
}
