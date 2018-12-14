using System;
using System.Collections.Generic;
using System.IO;
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
    /// HTTP web client extension.
    /// </summary>
    public static class HttpClientExtension
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
            return CopyToAsync(httpContent, destination, IO.StreamExtension.DefaultBufferSize, progress, cancellationToken);
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
                await IO.StreamExtension.CopyToAsync(downloadingStream, destination, bufferSize, progress, cancellationToken);
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
            return WriteFileAsync(httpContent, fileName, IO.StreamExtension.DefaultBufferSize, progress, cancellationToken);
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
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeJsonAsync<T>(this HttpContent httpContent)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serialize the HTTP XML content into an object as the specific type.
        /// </summary>
        /// <typeparam name="T">The type of the result expected.</typeparam>
        /// <param name="httpContent">The http response content.</param>
        /// <returns>The result serialized.</returns>
        /// <exception cref="ArgumentNullException">The argument is null.</exception>
        public static async Task<T> SerializeXmlAsync<T>(this HttpContent httpContent)
        {
            if (httpContent == null) throw new ArgumentNullException(nameof(httpContent));
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var serializer = new DataContractSerializer(typeof(T)); 
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
        /// <param name="httpContent">The http response content.</param>
        /// <param name="serializer">The serializer to read the object from the stream downloaded.</param>
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
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="retryCheck">A function to check whether the exception thrown should raise retry logic.</param>
        /// <param name="completionOption">When the operation should complete.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel operation.</param>
        /// <returns>The retry result.</returns>
        public static async Task<Tasks.RetryResult<HttpResponseMessage>> SendAsync(this HttpClient httpClient, HttpRequestMessage request, Tasks.IRetryPolicy retryPolicy, Func<Exception, bool> retryCheck, HttpCompletionOption completionOption, CancellationToken cancellationToken = default)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            return await Tasks.RetryExtension.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
            {
                return await httpClient.SendAsync(request, completionOption, cancellation);
            }, retryCheck, cancellationToken);
        }

        /// <summary>
        /// Sends an HTTP request as an asynchronous operation.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="request">The HTTP request message to send.</param>
        /// <param name="retryPolicy">The retry policy.</param>
        /// <param name="retryCheck">A function to check whether the exception thrown should raise retry logic.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel operation.</param>
        /// <returns>The retry result.</returns>
        public static async Task<Tasks.RetryResult<HttpResponseMessage>> SendAsync(this HttpClient httpClient, HttpRequestMessage request, Tasks.IRetryPolicy retryPolicy, Func<Exception, bool> retryCheck, CancellationToken cancellationToken = default)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));
            return await Tasks.RetryExtension.ProcessAsync(retryPolicy, async (CancellationToken cancellation) =>
            {
                return await httpClient.SendAsync(request, cancellation);
            }, retryCheck, cancellationToken);
        }
    }

    /// <summary>
    /// JSON format serialization HTTP client.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonHttpClient<T>
    {
        private readonly HttpRequestMessage request = new HttpRequestMessage();

        /// <summary>
        /// Gets or sets the retry policy.
        /// </summary>
        public Tasks.IRetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// Gets or sets the JSON serializer.
        /// </summary>
        public Func<string, T> Serializer { get; set; }

        /// <summary>
        /// Gets or sets the HTTP client.
        /// </summary>
        public HttpClient Client { get; set; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public HttpMethod Method
        {
            get => request.Method;
            set => request.Method = value;
        }

        /// <summary>
        /// Gets the HTTP request headers.
        /// </summary>
        public HttpRequestHeaders Headers => request.Headers;

        /// <summary>
        /// Gets or sets the request URI.
        /// </summary>
        public Uri Uri
        {
            get => request.RequestUri;
            set => request.RequestUri = value;
        }

        /// <summary>
        /// Gets or sets the HTTP version.
        /// </summary>
        public Version Version
        {
            get => request.Version;
            set => request.Version = value;
        }

        /// <summary>
        /// Gets or sets the request content.
        /// </summary>
        public HttpContent RequestContent
        {
            get => request.Content;
            set => request.Content = value;
        }

        /// <summary>
        /// Sends request.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        /// <returns>A result serialized.</returns>
        public async Task<T> Process(CancellationToken cancellationToken = default)
        {
            var client = Client ?? new HttpClient();
            var result = await Tasks.RetryExtension.ProcessAsync(RetryPolicy, async (CancellationToken cancellation) =>
            {
                var resp = await client.SendAsync(request, cancellationToken);
                var obj = Serializer != null
                    ? await HttpClientExtension.SerializeAsync(resp.Content, Serializer)
                    : await HttpClientExtension.SerializeJsonAsync<T>(resp.Content);
                return obj;
            }, cancellationToken);
            return result.Result;
        }
    }
}
