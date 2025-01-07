using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Trivial.Data;
using Trivial.IO;
using Trivial.Reflection;
using Trivial.Text;
using Trivial.Web;

namespace Trivial.Net;

/// <summary>
/// HTTP web client extension and helper.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Default block size.
    /// </summary>
    public const int DefaultBlockSize = 4194304; // 4M

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The instance.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClient value)
        => new InstanceObjectRef<HttpClient>(value);

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="client">The HTTP client created.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(out HttpClient client)
    {
        client = new();
        return new InstanceObjectRef<HttpClient>(client);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="onInit">A handler occurred on the HTTP client instance is initialized.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(Action<HttpClient> onInit)
    {
        var http = new HttpClient();
        onInit(http);
        return new InstanceObjectRef<HttpClient>(http);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value)
        => new InstanceObjectRef<HttpClient>(value is null ? new() : new(value));

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <param name="disposeHandler">true if the inner handler should be disposed when the HTTP client is disposed; otherwise, false, to reuse the inner handler.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value, bool disposeHandler)
        => new InstanceObjectRef<HttpClient>(new(value, disposeHandler));

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <param name="client">The HTTP client created.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value, out HttpClient client)
    {
        HttpClient http = value is null ? new() : new(value);
        client = http;
        return new InstanceObjectRef<HttpClient>(http);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <param name="disposeHandler">true if the inner handler should be disposed when the HTTP client is disposed; otherwise, false, to reuse the inner handler.</param>
    /// <param name="client">The HTTP client created.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value, bool disposeHandler, out HttpClient client)
    {
        HttpClient http = new(value, disposeHandler);
        client = http;
        return new InstanceObjectRef<HttpClient>(http);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <param name="disposeHandler">true if the inner handler should be disposed when the HTTP client is disposed; otherwise, false, to reuse the inner handler.</param>
    /// <param name="onInit">A handler occurred on the HTTP client instance is initialized.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value, bool disposeHandler, Action<HttpClient> onInit)
    {
        HttpClient http = new(value, disposeHandler);
        onInit?.Invoke(http);
        return new InstanceObjectRef<HttpClient>(http);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The instance.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(Lazy<HttpClient> value)
        => new LazyObjectRef<HttpClient>(value);

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="timeout">The duration to wait before the request times out.</param>
    /// <param name="maxResponseContentBufferSize">The maximum number of bytes to buffer when reading the response content; or null, by defaule, 2 gigabytes.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(TimeSpan timeout, long? maxResponseContentBufferSize = null)
    {
        var client = new HttpClient()
        {
            Timeout = timeout
        };
        if (maxResponseContentBufferSize.HasValue) client.MaxResponseContentBufferSize = maxResponseContentBufferSize.Value;
        return new InstanceObjectRef<HttpClient>(client);
    }

    /// <summary>
    /// Creates an object resolver for HTTP client.
    /// </summary>
    /// <param name="value">The handler of HTTP client instance responsible for processing the HTTP response.</param>
    /// <param name="disposeHandler">true if the inner handler should be disposed when the HTTP client is disposed; otherwise, false, to reuse the inner handler.</param>
    /// <param name="timeout">The duration to wait before the request times out.</param>
    /// <param name="maxResponseContentBufferSize">The maximum number of bytes to buffer when reading the response content; or null, by defaule, 2 gigabytes.</param>
    /// <returns>The object resolver of HTTP client.</returns>
    public static IObjectResolver<HttpClient> CreateResolver(HttpClientHandler value, bool disposeHandler, TimeSpan timeout, long? maxResponseContentBufferSize = null)
    {
        var client = new HttpClient(value, disposeHandler)
        {
            Timeout = timeout
        };
        if (maxResponseContentBufferSize.HasValue) client.MaxResponseContentBufferSize = maxResponseContentBufferSize.Value;
        return new InstanceObjectRef<HttpClient>(client);
    }

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="destination">The destination stream to write.</param>
    /// <param name="progress">The progress to report. The value is the length of the stream.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static Task CopyToAsync(this HttpContent httpContent, Stream destination, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        => CopyToAsync(httpContent, destination, StreamCopy.DefaultBufferSize, progress, cancellationToken);

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="destination">The destination stream to write.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
    /// <param name="progress">The progress to report. The value is the length of the stream.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task CopyToAsync(this HttpContent httpContent, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        if (destination == null) throw ObjectConvert.ArgumentNull(nameof(destination));
#if NET6_0_OR_GREATER
        var downloadingStream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        var downloadingStream = await httpContent.ReadAsStreamAsync();
#endif
        await StreamCopy.CopyToAsync(downloadingStream, destination, bufferSize, progress, cancellationToken);
    }

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static Task<FileInfo> WriteFileAsync(this HttpContent httpContent, string fileName, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        => WriteFileAsync(httpContent, fileName, StreamCopy.DefaultBufferSize, progress, cancellationToken);

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The file info instance to write.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static async Task<FileInfo> WriteFileAsync(this HttpContent httpContent, string fileName, int bufferSize, IProgress<double> progress = null, CancellationToken cancellationToken = default)
    {
        using (var fileStream = new FileStream(fileName, FileMode.Create))
        {
            var relativeProgress = progress != null && httpContent.Headers.ContentLength.HasValue ? new Progress<long>(totalBytes => progress.Report((double)totalBytes / httpContent.Headers.ContentLength.Value)) : null;
            await CopyToAsync(httpContent, fileStream, bufferSize, relativeProgress, cancellationToken);
        }

        progress?.Report(1);
        return new FileInfo(fileName);
    }

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="uri">The URI to download file.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The file info instance to write.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static async Task<FileInfo> WriteFileAsync(Uri uri, string fileName, IProgress<double> progress = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName) || uri == null) return null;
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromDays(2)
        };
#if NET6_0_OR_GREATER
        using var content = await httpClient.GetStreamAsync(uri, cancellationToken);
#else
        using var content = await httpClient.GetStreamAsync(uri);
#endif
        using var resp = await httpClient.GetAsync(uri, cancellationToken);
        return await WriteFileAsync(resp.Content, fileName, DefaultBlockSize, progress, cancellationToken);
    }

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static Task TryWriteFileAsync(HttpContent httpContent, string fileName, IProgress<double> progress = null, CancellationToken cancellationToken = default)
        => TryWriteFileAsync(httpContent, fileName, StreamCopy.DefaultBufferSize, progress, cancellationToken);

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The file info instance to write.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static Task<FileInfo> TryWriteFileAsync(HttpContent httpContent, string fileName, int bufferSize, IProgress<double> progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return WriteFileAsync(httpContent, fileName, bufferSize, progress, cancellationToken);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (FormatException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (ExternalException)
        {
        }

        return Task.FromResult<FileInfo>(null);
    }

    /// <summary>
    /// Loads the HTTP content into a stream of bytes and copies it to the file.
    /// </summary>
    /// <param name="uri">The URI to download file.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="progress">The progress to report, from 0 to 1.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The file info instance to write; or null, if failed.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="ArgumentException">The argument is invalid.</exception>
    /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
    /// <exception cref="IOException">An I/O error.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
    /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
    public static Task<FileInfo> TryWriteFileAsync(Uri uri, string fileName, IProgress<double> progress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return WriteFileAsync(uri, fileName, progress, cancellationToken);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
        catch (IOException)
        {
        }
        catch (FormatException)
        {
        }
        catch (SecurityException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (ExternalException)
        {
        }

        return Task.FromResult<FileInfo>(null);
    }

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="origin">A value of type seek origin indicating the reference point used to obtain the new position before deserialziation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, SeekOrigin origin, CancellationToken cancellationToken = default)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        var type = typeof(T);
#if NET6_0_OR_GREATER
        if (type == typeof(string)) return (T)(object)await httpContent.ReadAsStringAsync(cancellationToken);
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        if (type == typeof(string)) return (T)(object)await httpContent.ReadAsStringAsync();
        var stream = await httpContent.ReadAsStreamAsync();
#endif
        StreamCopy.TrySeek(stream, origin);
        if (type == typeof(JsonObjectNode)) return (T)(object)await JsonObjectNode.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(JsonArrayNode)) return (T)(object)await JsonArrayNode.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(JsonDocument)) return (T)(object)await JsonDocument.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(System.Text.Json.Nodes.JsonNode) || type.IsSubclassOf(typeof(System.Text.Json.Nodes.JsonNode))) return (T)(object)System.Text.Json.Nodes.JsonNode.Parse(stream);
        if (type.IsGenericType)
        {
            var generic = type.GetGenericTypeDefinition();
            if (generic == typeof(IAsyncEnumerable<>))
            {
                if (type == typeof(IAsyncEnumerable<ServerSentEventInfo>)) return (T)(object)ServerSentEventInfo.ParseAsync(stream);
                if (type == typeof(IAsyncEnumerable<JsonObjectNode>)) return (T)(object)ToJsonObjectNodesAsync(httpContent.Headers.ContentType?.MediaType, stream);
                if (type == typeof(IAsyncEnumerable<string>)) return (T)(object)CharsReader.ReadLinesAsync(stream, Encoding.UTF8);
            }
            else if (generic == typeof(IEnumerable<>))
            {
                if (type == typeof(IEnumerable<ServerSentEventInfo>)) return (T)(object)ServerSentEventInfo.Parse(stream);
                if (type == typeof(IEnumerable<JsonObjectNode>)) return (T)(object)ToJsonObjectNodes(httpContent.Headers.ContentType?.MediaType, stream);
                if (type == typeof(IEnumerable<string>)) return (T)(object)CharsReader.ReadLines(stream, Encoding.UTF8);
            }
            else if (generic == typeof(StreamingCollectionResult<>))
            {
                var genericType = type.GetGenericArguments().FirstOrDefault();
                if (genericType == null) return await JsonSerializer.DeserializeAsync<T>(stream, default(JsonSerializerOptions), cancellationToken);
                if (httpContent.Headers?.ContentType?.MediaType == WebFormat.ServerSentEventsMIME)
                    return (T)Activator.CreateInstance(type, new object[] { stream });
                var baseType = typeof(CollectionResult<>).MakeGenericType(new[] { genericType });
                var copy = await JsonSerializer.DeserializeAsync(stream, baseType, default(JsonSerializerOptions), cancellationToken);
                if (copy is null) return default;
                var con = type.GetConstructor(new[] { baseType });
                return (T)con.Invoke(new object[] { copy });
            }
        }

        if (type == typeof(QueryData)) return (T)(object)QueryData.ParseAsync(stream);
        if (type == typeof(Stream)) return (T)(object)stream;
        if (type == typeof(HttpContent)) return (T)(object)httpContent;
        return await JsonSerializer.DeserializeAsync<T>(stream, default(JsonSerializerOptions), cancellationToken);
    }

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, CancellationToken cancellationToken = default)
        => DeserializeJsonAsync<T>(httpContent, SeekOrigin.Current, cancellationToken);

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="origin">A value of type seek origin indicating the reference point used to obtain the new position before deserialziation.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, SeekOrigin origin, JsonSerializerOptions options, CancellationToken cancellationToken = default)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
#if NET6_0_OR_GREATER
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        var stream = await httpContent.ReadAsStreamAsync();
#endif
        StreamCopy.TrySeek(stream, origin);
        return await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
    }

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, JsonSerializerOptions options, CancellationToken cancellationToken = default)
        => DeserializeJsonAsync<T>(httpContent, SeekOrigin.Current, options, cancellationToken);

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The options for serialization.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, DataContractJsonSerializerSettings options)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        var stream = await httpContent.ReadAsStreamAsync();
        var serializer = options != null ? new DataContractJsonSerializer(typeof(T), options) : new DataContractJsonSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP XML content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The options for serialization.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeXmlAsync<T>(this HttpContent httpContent, DataContractSerializerSettings options = null)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        var stream = await httpContent.ReadAsStreamAsync();
        var serializer = options != null ? new DataContractSerializer(typeof(T), options) : new DataContractSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="deserializer">The XML serializer to read the object from the stream downloaded.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<object> DeserializeAsync(this HttpContent httpContent, XmlObjectSerializer deserializer)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        if (deserializer == null) throw ObjectConvert.ArgumentNull(nameof(deserializer));
        var stream = await httpContent.ReadAsStreamAsync();
        return deserializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="deserializer">The JSON deserializer.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeAsync<T>(this HttpContent httpContent, Func<string, T> deserializer)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        if (deserializer == null) throw ObjectConvert.ArgumentNull(nameof(deserializer));
        var str = await httpContent.ReadAsStringAsync();
        return deserializer(str);
    }

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <param name="options">The options for serialization.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static ValueTask<T> DeserializeJsonAsync<T>(this WebResponse webResponse, JsonSerializerOptions options = null)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        using var stream = webResponse.GetResponseStream();
        return JsonSerializer.DeserializeAsync<T>(stream, options);
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeJsonAsync<T>(this HttpContent httpContent, DataContractJsonSerializerSettings options, CancellationToken cancellationToken)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
        var serializer = options != null ? new DataContractJsonSerializer(typeof(T), options) : new DataContractJsonSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP XML content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeXmlAsync<T>(this HttpContent httpContent, DataContractSerializerSettings options, CancellationToken cancellationToken)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
        var serializer = options != null ? new DataContractSerializer(typeof(T), options) : new DataContractSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="deserializer">The XML serializer to read the object from the stream downloaded.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<object> DeserializeAsync(this HttpContent httpContent, XmlObjectSerializer deserializer, CancellationToken cancellationToken)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        if (deserializer == null) throw ObjectConvert.ArgumentNull(nameof(deserializer));
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
        return deserializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="deserializer">The JSON deserializer.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeAsync<T>(this HttpContent httpContent, Func<string, T> deserializer, CancellationToken cancellationToken)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
        if (deserializer == null) throw ObjectConvert.ArgumentNull(nameof(deserializer));
        var str = await httpContent.ReadAsStringAsync(cancellationToken);
        return deserializer(str);
    }

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <param name="options">The options for serialization.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static ValueTask<T> DeserializeJsonAsync<T>(this WebResponse webResponse, JsonSerializerOptions options, CancellationToken cancellationToken)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        using var stream = webResponse.GetResponseStream();
        return JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken);
    }
#endif

    /// <summary>
    /// Deserializes the HTTP JSON content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <param name="options">The options for serialization.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static T DeserializeJson<T>(this WebResponse webResponse, DataContractJsonSerializerSettings options)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        using var stream = webResponse.GetResponseStream();
        var serializer = options != null ? new DataContractJsonSerializer(typeof(T), options) : new DataContractJsonSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP XML content into an object as the specific type.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <param name="options">The options for serialization.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static T DeserializeXml<T>(this WebResponse webResponse, DataContractSerializerSettings options = null)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        using var stream = webResponse.GetResponseStream();
        var serializer = options != null ? new DataContractSerializer(typeof(T), options) : new DataContractSerializer(typeof(T));
        return (T)serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <param name="webResponse">The web response.</param>
    /// <param name="serializer">The serializer to read the object from the stream downloaded.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static object Deserialize(this WebResponse webResponse, XmlObjectSerializer serializer)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        if (serializer == null) throw ObjectConvert.ArgumentNull(nameof(serializer));
        using var stream = webResponse.GetResponseStream();
        return serializer.ReadObject(stream);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="webResponse">The web response.</param>
    /// <param name="deserializer">The JSON deserializer.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static async Task<T> DeserializeAsync<T>(this WebResponse webResponse, Func<string, T> deserializer, Encoding encoding = null)
    {
        if (webResponse == null) throw ObjectConvert.ArgumentNull(nameof(webResponse));
        if (deserializer == null) throw ObjectConvert.ArgumentNull(nameof(deserializer));
        using var stream = webResponse.GetResponseStream();
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        var str = await reader.ReadToEndAsync();
        return deserializer(str);
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="JsonException">Deserialization failure.</exception>
    public static async Task<StreamingCollectionResult<T>> DeserializeCollectionResultAsync<T>(this HttpContent httpContent, Action<StreamingCollectionResult<T>, ServerSentEventInfo> callback, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
    {
        if (httpContent == null) throw ObjectConvert.ArgumentNull(nameof(httpContent));
#if NET6_0_OR_GREATER
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        var stream = await httpContent.ReadAsStreamAsync();
#endif
        if (httpContent.Headers?.ContentType?.MediaType == WebFormat.ServerSentEventsMIME) return new(stream, callback, options);
        var copy = await JsonSerializer.DeserializeAsync<CollectionResult<T>>(stream, options, cancellationToken);
        var result = new StreamingCollectionResult<T>(copy);
        callback?.Invoke(result, null);
        return result;
    }

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="callback">The callback handler on Server-Sent Event info is received and processed.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="JsonException">Deserialization failure.</exception>
    public static Task<StreamingCollectionResult<T>> DeserializeCollectionResultAsync<T>(this HttpContent httpContent, Action<StreamingCollectionResult<T>> callback, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        => DeserializeCollectionResultAsync<T>(httpContent, callback == null ? null : (obj, e) => callback.Invoke(obj), options, cancellationToken);

    /// <summary>
    /// Deserializes the HTTP content into an object by the specific serializer.
    /// </summary>
    /// <typeparam name="T">The type of the result expected.</typeparam>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="options">The JSON serializer options.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The result serialized.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    /// <exception cref="JsonException">Deserialization failure.</exception>
    public static Task<StreamingCollectionResult<T>> DeserializeCollectionResultAsync<T>(this HttpContent httpContent, JsonSerializerOptions options = null, CancellationToken cancellationToken = default)
        => DeserializeCollectionResultAsync<T>(httpContent, null as Action<StreamingCollectionResult<T>, ServerSentEventInfo>, options, cancellationToken);

    /// <summary>
    /// Creates an HTTP content from a JSON of the specific object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <returns>The HTTP content a JSON of the specific object.</returns>
    public static StreamContent CreateJsonContent(JsonObjectNode value)
    {
        if (value == null) return null;
        var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream);
        value.WriteTo(writer);
        writer.Flush();
        stream.Position = 0;
        var content = new StreamContent(stream);
        content.Headers.ContentType = JsonValues.GetJsonMediaTypeHeaderValue();
        return content;
    }

    /// <summary>
    /// Creates an HTTP content from a JSON of the specific object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <param name="options">An optional serialization options.</param>
    /// <returns>The HTTP content a JSON of the specific object.</returns>
    public static StreamContent CreateJsonContent(object value, JsonSerializerOptions options = null)
    {
        if (value == null) return null;
        var stream = new MemoryStream();
        JsonValues.WriteTo(stream, value, options);
        var content = new StreamContent(stream);
        content.Headers.ContentType = JsonValues.GetJsonMediaTypeHeaderValue();
        return content;
    }

    /// <summary>
    /// Creates an HTTP content from a JSON of the specific object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <param name="options">An optional serialization options.</param>
    /// <returns>The HTTP content a JSON of the specific object.</returns>
    public static StreamContent CreateJsonContent(object value, DataContractJsonSerializerSettings options)
    {
        if (value == null) return null;
        var stream = new MemoryStream();
        JsonValues.WriteTo(stream, value, options);
        var content = new StreamContent(stream);
        content.Headers.ContentType = JsonValues.GetJsonMediaTypeHeaderValue();
        return content;
    }

    /// <summary>
    /// Adds a file to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="file">The file info instance.</param>
    /// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
    /// <param name="mediaType">The MIME of the file.</param>
    /// <return>The HTTP content to add.</return>
    public static StreamContent Add(this MultipartFormDataContent content, string name, FileInfo file, string fileName = null, string mediaType = null)
    {
        if (content == null || file == null) return null;
        var c = new StreamContent(file.OpenRead());
        if (!string.IsNullOrWhiteSpace(mediaType))
        {
            c.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        }
        else if (mediaType == null)
        {
            var mime = WebFormat.GetMime(file);
            if (!string.IsNullOrWhiteSpace(mime)) c.Headers.ContentType = new MediaTypeHeaderValue(mime);
        }

        content.Add(c, name, fileName ?? file.Name);
        return c;
    }

    /// <summary>
    /// Adds a file to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
    /// <param name="mediaType">The MIME of the file.</param>
    /// <return>The HTTP content to add.</return>
    public static StreamContent Add(this MultipartFormDataContent content, string name, Stream stream, string fileName = null, string mediaType = null)
    {
        if (content == null || stream == null || !stream.CanRead) return null;
        var c = new StreamContent(stream);
        if (!string.IsNullOrWhiteSpace(mediaType)) c.Headers.ContentType = new MediaTypeHeaderValue(mediaType);
        content.Add(c, name, fileName);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="value">The property value.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <param name="mediaType">The MIME of the file.</param>
    /// <return>The HTTP content to add.</return>
    public static StringContent Add(this MultipartFormDataContent content, string name, string value, Encoding encoding = null, string mediaType = null)
    {
        if (content == null) return null;
        encoding ??= Encoding.UTF8;
        var c = string.IsNullOrWhiteSpace(mediaType) ? new StringContent(value, encoding) : new StringContent(value, encoding, mediaType);
        content.Add(c, name);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="value">The property value.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <return>The HTTP content to add.</return>
    public static StringContent Add(this MultipartFormDataContent content, string name, int value, Encoding encoding = null)
    {
        if (content == null) return null;
        var c = new StringContent(value.ToString(), encoding ?? Encoding.UTF8);
        content.Add(c, name);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="value">The property value.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <return>The HTTP content to add.</return>
    public static StringContent Add(this MultipartFormDataContent content, string name, long value, Encoding encoding = null)
    {
        if (content == null) return null;
        var c = new StringContent(value.ToString(), encoding ?? Encoding.UTF8);
        content.Add(c, name);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="value">The property value.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <return>The HTTP content to add.</return>
    public static StringContent Add(this MultipartFormDataContent content, string name, double value, Encoding encoding = null)
    {
        if (content == null) return null;
        var c = new StringContent(value.ToString(), encoding ?? Encoding.UTF8);
        content.Add(c, name);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="name">The name for the HTTP content to add.</param>
    /// <param name="value">The property value.</param>
    /// <param name="dateFormat">A standard or custom date and time format string.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <return>The HTTP content to add.</return>
    /// <exception cref="FormatException">
    /// The length of format is 1, and it is not one of the format specifier characters defined for System.Globalization.DateTimeFormatInfo.
    /// -or- format does not contain a valid custom format pattern.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The date and time is outside the range of dates supported by the calendar used by the current culture.
    /// </exception>
    public static StringContent Add(this MultipartFormDataContent content, string name, DateTime value, string dateFormat = null, Encoding encoding = null)
    {
        if (content == null) return null;
        var c = new StringContent(string.IsNullOrWhiteSpace(dateFormat) ? WebFormat.ParseDate(value).ToString() : value.ToString(dateFormat), encoding ?? Encoding.UTF8);
        content.Add(c, name);
        return c;
    }

    /// <summary>
    /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
    /// </summary>
    /// <param name="content">The HTTP content of multipart form data.</param>
    /// <param name="collections">The collection to add.</param>
    /// <param name="encoding">The character encoding to use.</param>
    /// <return>The HTTP content to add.</return>
    public static void Add(this MultipartFormDataContent content, IDictionary<string, string> collections, Encoding encoding = null)
    {
        if (content == null || collections == null) return;
        foreach (var item in collections)
        {
            content.Add(new StringContent(item.Value, encoding ?? Encoding.UTF8), item.Key);
        }
    }

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
    /// <param name="completionOption">When the operation should complete.</param>
    /// <param name="cancellationToken">The optional cancellation token to cancel operation.</param>
    /// <returns>The retry result.</returns>
    public static async Task<Tasks.RetryResult<HttpResponseMessage>> SendAsync(this HttpClient httpClient, HttpRequestMessage request, Tasks.IRetryPolicy retryPolicy, Func<Exception, Exception> needThrow, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
    {
        if (httpClient == null) throw ObjectConvert.ArgumentNull(nameof(httpClient));
        return await Tasks.RetryExtensions.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
        {
            return await httpClient.SendAsync(request, completionOption, cancellation);
        }, needThrow, cancellationToken);
    }

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="request">The HTTP request message to send.</param>
    /// <param name="retryPolicy">The retry policy.</param>
    /// <param name="needThrow">A handler to check if need throw the exception without retry.</param>
    /// <param name="cancellationToken">The optional cancellation token to cancel operation.</param>
    /// <returns>The retry result.</returns>
    public static async Task<Tasks.RetryResult<HttpResponseMessage>> SendAsync(this HttpClient httpClient, HttpRequestMessage request, Tasks.IRetryPolicy retryPolicy, Func<Exception, Exception> needThrow, CancellationToken cancellationToken = default)
    {
        if (httpClient == null) throw ObjectConvert.ArgumentNull(nameof(httpClient));
        return await Tasks.RetryExtensions.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
        {
            return await httpClient.SendAsync(request, cancellation);
        }, needThrow, cancellationToken);
    }

    /// <summary>
    /// Sets the preferred color scheme to HTTP request headers.
    /// </summary>
    /// <param name="headers">The headers to fill the property.</param>
    /// <param name="value">The preferred color scheme to add, e.g. light and dark.</param>
    public static void SetTheme(HttpRequestHeaders headers, string value)
    {
        if (headers == null) return;
        headers.Remove("Sec-CH-Prefers-Color-Scheme");
        if (!string.IsNullOrWhiteSpace(value)) headers.Add("Sec-CH-Prefers-Color-Scheme", JsonStringNode.ToJson(value));
    }

    /// <summary>
    /// Sets the preferred color scheme as light to HTTP request headers.
    /// </summary>
    /// <param name="headers">The headers to fill the property.</param>
    public static void SetLightTheme(HttpRequestHeaders headers)
        => SetTheme(headers, "light");

    /// <summary>
    /// Sets the preferred color scheme as dark to HTTP request headers.
    /// </summary>
    /// <param name="headers">The headers to fill the property.</param>
    public static void SetDarkTheme(HttpRequestHeaders headers)
        => SetTheme(headers, "dark");

    /// <summary>
    /// Sets the OS platform name to HTTP request headers.
    /// </summary>
    /// <param name="headers">The headers to fill the property.</param>
    /// <param name="fullInfo">true if also append its version and CPU arch; otherwise, false.</param>
    /// <returns>The platform name.</returns>
    public static string SetPlatform(HttpRequestHeaders headers, bool fullInfo = false)
    {
        var client = new HttpClient();
        var name = GetPlatformName();
        if (string.IsNullOrWhiteSpace(name) || name == "Unknown" || headers == null) return name;
        headers.Add("Sec-CH-UA-Platform", string.Concat('"', name, '"'));
        if (!fullInfo) return name;
        try
        {
            headers.Remove("Sec-CH-UA-Platform-Version");
            headers.Add("Sec-CH-UA-Platform-Version", string.Concat('"', Environment.OSVersion.Version, '"'));
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (ExternalException)
        {
        }

#if NET48_OR_GREATER || NETCOREAPP
        try
        {
            var bitness = Environment.Is64BitProcess ? "64" : "32";
            var arch = RuntimeInformation.ProcessArchitecture.ToString();
            headers.Remove("Sec-CH-UA-Bitness");
            headers.Add("Sec-CH-UA-Bitness", bitness);
            headers.Remove("Sec-CH-UA-Arch");
            headers.Add("Sec-CH-UA-Arch", string.Concat('"', arch, '"'));
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (ExternalException)
        {
        }
#endif

        return name;
    }

    internal static HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri requestUri)
    {
        var m = new HttpRequestMessage(method, requestUri);
        m.Headers.Date = DateTimeOffset.Now;
        return m;
    }

    internal static HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri)
    {
        var m = new HttpRequestMessage(method, requestUri);
        m.Headers.Date = DateTimeOffset.Now;
        return m;
    }

    internal static HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri requestUri, HttpContent content)
    {
        var m = new HttpRequestMessage(method, requestUri)
        {
            Content = content
        };
        m.Headers.Date = DateTimeOffset.Now;
        return m;
    }

    internal static HttpRequestMessage CreateRequestMessage(HttpMethod method, string requestUri, HttpContent content)
    {
        var m = new HttpRequestMessage(method, requestUri)
        {
            Content = content
        };
        m.Headers.Date = DateTimeOffset.Now;
        return m;
    }

    /// <summary>
    /// Gets the operating system name of the host.
    /// </summary>
    /// <returns>The OS name used in HTTP request header Sec-CH-UA-Platform.</returns>
    private static string GetPlatformName()
    {
        try
        {
#if NETFRAMEWORK
            return Environment.OSVersion.Platform switch
            {
                PlatformID.Win32NT or PlatformID.Win32Windows or PlatformID.Win32S => "Windows",
                PlatformID.MacOSX => "macOS",
                PlatformID.WinCE => "Windows CE",
                PlatformID.Xbox => "Xbox",
                _ => "Unknown"
            };
#else
            if (OperatingSystem.IsWindows()) return "Windows";
            if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst()) return "macOS";
            if (OperatingSystem.IsIOS()) return "iOS";
            if (OperatingSystem.IsAndroid()) return "Android";
            if (OperatingSystem.IsTvOS()) return "tvOS";
            if (OperatingSystem.IsWatchOS()) return "watchOS";
            if (OperatingSystem.IsLinux()) return "Linux";

            //if (OperatingSystem.IsBrowser()) return "Chromium OS";    // Need read its host.
#endif
        }
        catch (InvalidOperationException)
        {
        }
        catch (NullReferenceException)
        {
        }
        catch (NotSupportedException)
        {
        }
        catch (NotImplementedException)
        {
        }
        catch (ArgumentException)
        {
        }
        catch (ExternalException)
        {
        }

        return "Unknown";
    }

    private static IEnumerable<JsonObjectNode> ToJsonObjectNodes(string mime, Stream stream)
    {
        if (mime == JsonValues.JsonlMIME)
        {
            var reader = CharsReader.ReadLines(stream, Encoding.UTF8);
            foreach (var line in reader)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return JsonObjectNode.Parse(line);
            }
        }
        else if (mime == WebFormat.ServerSentEventsMIME)
        {
            var col = ServerSentEventInfo.Parse(stream, Encoding.UTF8);
            foreach (var line in col)
            {
                yield return line.TryGetJsonData();
            }
        }
        else
        {
            var arr = JsonArrayNode.Parse(stream);
            foreach (var json in arr.ToJsonObjectNodes())
            {
                yield return json;
            }
        }
    }

    private static async IAsyncEnumerable<JsonObjectNode> ToJsonObjectNodesAsync(string mime, Stream stream)
    {
        if (mime == JsonValues.JsonlMIME)
        {
            var reader = CharsReader.ReadLinesAsync(stream, Encoding.UTF8);
            await foreach (var line in reader)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return JsonObjectNode.Parse(line);
            }
        }
        else if (mime == WebFormat.ServerSentEventsMIME)
        {
            var col = ServerSentEventInfo.ParseAsync(stream, Encoding.UTF8);
            await foreach (var line in col)
            {
                yield return line.TryGetJsonData();
            }
        }
        else
        {
            var arr = await JsonArrayNode.ParseAsync(stream);
            foreach (var json in arr.ToJsonObjectNodes())
            {
                yield return json;
            }
        }
    }
}
