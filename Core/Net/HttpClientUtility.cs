using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Net
{
    /// <summary>
    /// HTTP web client extension and helper.
    /// </summary>
    public static class HttpClientUtility
    {
        /// <summary>
        /// Default block size.
        /// </summary>
        public const int DefaultBlockSize = 4194304; // 4M

        /// <summary>
        /// Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
        /// </summary>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="destination">The destination stream to write.</param>
        /// <param name="progress">The progress to report. The value is the length of the stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static Task CopyToAsync(this HttpContent httpContent, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            return CopyToAsync(httpContent, destination, IO.StreamUtility.DefaultBufferSize, progress, cancellationToken);
        }

        /// <summary>
        /// Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
        /// </summary>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="destination">The destination stream to write.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
        /// <param name="progress">The progress to report. The value is the length of the stream.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task CopyToAsync(this HttpContent httpContent, Stream destination, int bufferSize, IProgress<long> progress, CancellationToken cancellationToken = default)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            using (var downloadingStream = await httpContent.ReadAsStreamAsync())
            {
                await IO.StreamUtility.CopyToAsync(downloadingStream, destination, bufferSize, progress, cancellationToken);
            }
        }

        /// <summary>
        /// Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
        /// </summary>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="progress">The progress to report, from 0 to 1.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        /// <exception cref="ArgumentException">The argument is invalid.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="IOException">An I/O error.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
        public static Task WriteFileAsync(this HttpContent httpContent, string fileName, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            return WriteFileAsync(httpContent, fileName, IO.StreamUtility.DefaultBufferSize, progress, cancellationToken);
        }

        /// <summary>
        /// Serialize the HTTP content into a stream of bytes and copies it to the stream object provided as the stream parameter.
        /// </summary>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="bufferSize">The size, in bytes, of the buffer. This value must be greater than zero.</param>
        /// <param name="progress">The progress to report, from 0 to 1.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The file info instance to write.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        /// <exception cref="ArgumentException">The argument is invalid.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        /// <exception cref="IOException">An I/O error.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="NotSupportedException">The path of the file refers to a non-file device, such as "con:", "com1:", "lpt1:".</exception>
        public static async Task<FileInfo> WriteFileAsync(this HttpContent httpContent, string fileName, int bufferSize, IProgress<double> progress, CancellationToken cancellationToken = default)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                var relativeProgress = progress != null && httpContent.Headers.ContentLength.HasValue ? new Progress<long>(totalBytes => progress.Report((double)totalBytes / httpContent.Headers.ContentLength.Value)) : null;
                await CopyToAsync(httpContent, fileStream, bufferSize, relativeProgress, cancellationToken);
            }

            progress.Report(1);
            return new FileInfo(fileName);
        }

        /// <summary>
        /// Serialize the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeJsonAsync<T>(this HttpContent httpContent, DataContractJsonSerializerSettings settings = null)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var serializer = settings != null ? new DataContractJsonSerializer(typeof(T), settings) : new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP XML content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeXmlAsync<T>(this HttpContent httpContent, DataContractSerializerSettings settings = null)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var serializer = settings != null ? new DataContractSerializer(typeof(T), settings) : new DataContractSerializer(typeof(T)); 
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP content into an object by the specific serializer.
        /// </summary>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="serializer">The serializer to read the object from the stream downloaded.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<object> SerializeAsync(this HttpContent httpContent, XmlObjectSerializer serializer)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                return serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP content into an object by the specific serializer.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <param name="serializer">The JSON serializer.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeAsync<T>(this HttpContent httpContent, Func<string, T> serializer)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            var str = await httpContent.ReadAsStringAsync();
            return serializer(str);
        }

        /// <summary>
        /// Serialize the HTTP JSON content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static T SerializeJson<T>(this WebResponse webResponse, DataContractJsonSerializerSettings settings = null)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
            using (var stream = webResponse.GetResponseStream())
            {
                var serializer = settings != null ? new DataContractJsonSerializer(typeof(T), settings) : new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP XML content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="settings">The options for serialization.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static T SerializeXml<T>(this WebResponse webResponse, DataContractSerializerSettings settings = null)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
            using (var stream = webResponse.GetResponseStream())
            {
                var serializer = settings != null ? new DataContractSerializer(typeof(T), settings) : new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP content into an object by the specific serializer.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <param name="serializer">The serializer to read the object from the stream downloaded.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static object Serialize(this WebResponse webResponse, XmlObjectSerializer serializer)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            using (var stream = webResponse.GetResponseStream())
            {
                return serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP content into an object by the specific serializer.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="webResponse">The web response.</param>
        /// <param name="serializer">The JSON serializer.</param>
        /// <param name="encoding">The character encoding to use.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeAsync<T>(this WebResponse webResponse, Func<string, T> serializer, Encoding encoding = null)
        {
            if (webResponse == null) throw new ArgumentNullException(nameof(webResponse));
            if (serializer == null) throw new ArgumentNullException(nameof(serializer));
            using (var stream = webResponse.GetResponseStream())
            {
                var reader = new StreamReader(stream, encoding ?? Encoding.UTF8);
                var str = await reader.ReadToEndAsync();
                return serializer(str);
            }
        }

        /// <summary>
        /// Creates a stream content from a JSON of the specific object.
        /// </summary>
        /// <param name="value">The graph.</param>
        /// <returns>The HTTP content based on a stream from a JSON of the specific object.</returns>
        public static StreamContent CreateJsonStreamContent(object value)
        {
            if (value == null) return null;
            var serializer = new DataContractJsonSerializer(value.GetType());
            var stream = new MemoryStream();
            serializer.WriteObject(stream, value);
            var content = new StreamContent(stream);
            return content;
        }

        /// <summary>
        /// Adds a file to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="file">The file info instance.</param>
        /// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
        public static void Add(this MultipartFormDataContent content, string name, FileInfo file, string fileName = null)
        {
            if (content == null || file == null) return;
            var stream = new StreamContent(file.OpenRead());
            content.Add(stream, name, fileName ?? file.Name);
        }

        /// <summary>
        /// Adds a file to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="fileName">The file name for the HTTP content to add to the collection.</param>
        public static void Add(this MultipartFormDataContent content, string name, Stream stream, string fileName = null)
        {
            if (content == null || stream == null || !stream.CanRead) return;
            content.Add(new StreamContent(stream), name, fileName);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="value">The property value.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static void Add(this MultipartFormDataContent content, string name, string value, Encoding encoding = null)
        {
            if (content == null) return;
            content.Add(new StringContent(value, encoding ?? Encoding.UTF8), name);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="value">The property value.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static void Add(this MultipartFormDataContent content, string name, int value, Encoding encoding = null)
        {
            if (content == null) return;
            content.Add(new StringContent(value.ToString(), encoding ?? Encoding.UTF8), name);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="value">The property value.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static void Add(this MultipartFormDataContent content, string name, long value, Encoding encoding = null)
        {
            if (content == null) return;
            content.Add(new StringContent(value.ToString(), encoding ?? Encoding.UTF8), name);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="value">The property value.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static void Add(this MultipartFormDataContent content, string name, double value, Encoding encoding = null)
        {
            if (content == null) return;
            content.Add(new StringContent(value.ToString(), encoding ?? Encoding.UTF8), name);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="name">The name for the HTTP content to add.</param>
        /// <param name="value">The property value.</param>
        /// <param name="encoding">The character encoding to use.</param>
        public static void Add(this MultipartFormDataContent content, string name, DateTime value, Encoding encoding = null)
        {
            if (content == null) return;
            content.Add(new StringContent(Web.WebUtility.ParseDate(value).ToString(), encoding ?? Encoding.UTF8), name);
        }

        /// <summary>
        /// Adds a string to a collection of System.Net.Http.HttpContent objects that get serialized to multipart/form-data MIME type.
        /// </summary>
        /// <param name="content">The HTTP content of multipart form data.</param>
        /// <param name="collections">The collection to add.</param>
        /// <param name="encoding">The character encoding to use.</param>
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
            return await Tasks.RetryExtension.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
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
            return await Tasks.RetryExtension.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
            {
                return await httpClient.SendAsync(request, cancellation);
            }, needThrow, cancellationToken);
        }
    }
}
