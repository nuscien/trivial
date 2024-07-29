using System;
using System.Collections.Generic;
using System.IO;
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
    /// Loads the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
    /// </summary>
    /// <param name="httpContent">The http response content.</param>
    /// <param name="destination">The destination stream to write.</param>
    /// <param name="progress">The progress to report. The value is the length of the stream.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work if it has not yet started.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">The argument is null.</exception>
    public static Task CopyToAsync(this HttpContent httpContent, Stream destination, IProgress<long> progress = null, CancellationToken cancellationToken = default)
    {
        return CopyToAsync(httpContent, destination, IO.StreamCopy.DefaultBufferSize, progress, cancellationToken);
    }

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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
        if (destination == null) throw new ArgumentNullException(nameof(destination));
#if NET6_0_OR_GREATER
        var downloadingStream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        var downloadingStream = await httpContent.ReadAsStreamAsync();
#endif
        await IO.StreamCopy.CopyToAsync(downloadingStream, destination, bufferSize, progress, cancellationToken);
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
        => WriteFileAsync(httpContent, fileName, IO.StreamCopy.DefaultBufferSize, progress, cancellationToken);

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
        => TryWriteFileAsync(httpContent, fileName, IO.StreamCopy.DefaultBufferSize, progress, cancellationToken);

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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
        var type = typeof(T);
#if NET6_0_OR_GREATER
        if (type == typeof(string)) return (T)(object)await httpContent.ReadAsStringAsync(cancellationToken);
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        if (type == typeof(string)) return (T)(object)await httpContent.ReadAsStringAsync();
        var stream = await httpContent.ReadAsStreamAsync();
#endif
        IO.StreamCopy.TrySeek(stream, origin);
        if (type == typeof(JsonObjectNode)) return (T)(object)await JsonObjectNode.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(JsonArrayNode)) return (T)(object)await JsonArrayNode.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(JsonDocument)) return (T)(object)await JsonDocument.ParseAsync(stream, default, cancellationToken);
        if (type == typeof(System.Text.Json.Nodes.JsonNode) || type.IsSubclassOf(typeof(System.Text.Json.Nodes.JsonNode))) return (T)(object)System.Text.Json.Nodes.JsonNode.Parse(stream);
        if (type == typeof(IEnumerable<ServerSentEventInfo>)) return (T)(object)ServerSentEventInfo.Parse(stream);
        if (type == typeof(Stream)) return (T)(object)stream;
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
#if NET6_0_OR_GREATER
        var stream = await httpContent.ReadAsStreamAsync(cancellationToken);
#else
        var stream = await httpContent.ReadAsStreamAsync();
#endif
        IO.StreamCopy.TrySeek(stream, origin);
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
        if (deserializer == null) throw new ArgumentNullException(nameof(deserializer), "serializer should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
        if (deserializer == null) throw new ArgumentNullException(nameof(deserializer), "serializer should not be null.");
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse), "webResponse should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
        if (deserializer == null) throw new ArgumentNullException(nameof(deserializer), "serializer should not be null.");
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
        if (httpContent == null) throw new ArgumentNullException(nameof(httpContent), "httpContent should not be null.");
        if (deserializer == null) throw new ArgumentNullException(nameof(deserializer), "serializer should not be null.");
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse), "webResponse should not be null.");
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse), "webResponse should not be null.");
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
        if (serializer == null) throw new ArgumentNullException(nameof(serializer));
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
        if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
        if (deserializer == null) throw new ArgumentNullException(nameof(deserializer));
        using var stream = webResponse.GetResponseStream();
        using var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
        var str = await reader.ReadToEndAsync();
        return deserializer(str);
    }

    /// <summary>
    /// Creates an HTTP content from a JSON of the specific object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <param name="options">An optional serialization options.</param>
    /// <returns>The HTTP content a JSON of the specific object.</returns>
    public static StringContent CreateJsonContent(object value, JsonSerializerOptions options = null)
    {
        if (value == null) return null;
        var json = StringExtensions.ToJson(value, options);
        if (json == null) return null;
        return new StringContent(json, Encoding.UTF8, WebFormat.JsonMIME);
    }

    /// <summary>
    /// Creates an HTTP content from a JSON of the specific object.
    /// </summary>
    /// <param name="value">The object.</param>
    /// <param name="options">An optional serialization options.</param>
    /// <returns>The HTTP content a JSON of the specific object.</returns>
    public static StringContent CreateJsonContent(object value, DataContractJsonSerializerSettings options)
    {
        if (value == null) return null;
        var json = StringExtensions.ToJson(value, options);
        if (json == null) return null;
        return new StringContent(json, Encoding.UTF8, WebFormat.JsonMIME);
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
        if (encoding == null) encoding = Encoding.UTF8;
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
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
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
        if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
        return await Tasks.RetryExtensions.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
        {
            return await httpClient.SendAsync(request, cancellation);
        }, needThrow, cancellationToken);
    }
}
