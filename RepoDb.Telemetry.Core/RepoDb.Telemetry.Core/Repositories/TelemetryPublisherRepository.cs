using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using Serilog;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// A class that is used to publish the telemetry data to the insights solution.
    /// </summary>
    public abstract class TelemetryPublisherRepository : IPublisherRepository
    {
        #region Privates

        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiKey;
        private readonly Action<Exception> _errorCallback;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryPublisherRepository"/> class.
        /// </summary>
        /// <param name="host">The host to where to publish the telemetry data.</param>
        /// <param name="apiKey">The API key to be used for authentication. Leave this to empty if not provided in the collector API.</param>"
        /// <param name="errorCallback">The callback function to call in the case of any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        public TelemetryPublisherRepository(
            string host = "http://localhost:5000",
            string apiKey = null,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            Host = host;
            _apiKey = apiKey;
            _errorCallback = errorCallback;
            _logger = logger;
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Gets the request URI where to publish the telemetry data. The URI will be appended after the <see cref="Host"/> to compose the target endpoint.
        /// </summary>
        /// <returns>The URI to where to publish the telemetry data.</returns>
        public abstract string GetRequestUri();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the host of the teleemtry collector API.
        /// </summary>
        public string Host { get; private set;  }

        #endregion

        #region Methods

        /// <summary>
        /// Publishes the telemetry data to the insights solution.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        public void Publish(
            TelemetryItem telemetryItem) =>
                PublishMany(new[] { telemetryItem });

        /// <summary>
        /// Publishes the telemetry data to the insights solution.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public async void PublishAsync(
            TelemetryItem telemetryItem,
            CancellationToken cancellationToken = default) =>
                PublishManyAsync(new[] { telemetryItem }, cancellationToken);

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        public void PublishMany(
            IEnumerable<TelemetryItem> telemetryItems)
        {
            try
            {
                var compressed = ToCompressedJsonBytes(telemetryItems);
                using (var content = CreateCompressedContent(compressed))
                using (var request = CreatePublishRequest(content))
                {
                    _logger?.Debug("Publishing telemetry data to {Host}.", Host);
                    var result = _httpClient
                        .SendAsync(request)
                        .GetAwaiter()
                        .GetResult();
                    result.EnsureSuccessStatusCode();
                    _logger?.Information("{Count} telemetry data has been published.", telemetryItems.Count());
                }
            }
            catch (Exception ex)
            {
                var error = new InvalidOperationException("Failed to publish the telemetry data.", ex);
                _logger?.Error(error, "Failed to publish the telemetry data.");
                _errorCallback?.Invoke(error);
            }
        }

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution in an asynchronous way.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public async void PublishManyAsync(
            IEnumerable<TelemetryItem> telemetryItems,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var compressed = ToCompressedJsonBytes(telemetryItems);
                using (var content = CreateCompressedContent(compressed))
                using (var request = CreatePublishRequest(content))
                {
                    _logger?.Debug("Publishing telemetry data to {Host}.", Host);
                    var result = await _httpClient
                        .SendAsync(request, cancellationToken);
                    result.EnsureSuccessStatusCode();
                    _logger?.Information("{Count} telemetry data has been published.", telemetryItems.Count());
                }
            }
            catch (Exception ex)
            {
                var error = new InvalidOperationException("Failed to publish the telemetry data.", ex);
                _logger?.Error(error, "Failed to publish the telemetry data.");
                _errorCallback?.Invoke(error);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Serializes the telemetry items to JSON and gzip-compresses the resulting bytes.
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <returns></returns>
        private byte[] ToCompressedJsonBytes(
            IEnumerable<TelemetryItem> telemetryItems)
        {
            var json = JsonSerializer.Serialize(telemetryItems);
            return Compress(json);
        }

        /// <summary>
        /// Wraps the gzip-compressed bytes in an HTTP content whose body is the Base64-encoded
        /// string of the compressed bytes, so the receiving API can decode, decompress, and
        /// parse it as JSON.
        /// </summary>
        /// <param name="compressed"></param>
        /// <returns></returns>
        private HttpContent CreateCompressedContent(
            byte[] compressed)
        {
            var base64 = Convert.ToBase64String(compressed);
            var content = new StringContent(base64, Encoding.UTF8, "text/plain");
            return content;
        }

        /// <summary>
        /// Builds the HTTP request used to publish the telemetry data, tagging it with the
        /// X-API-Key header when an API key has been configured.
        /// </summary>
        /// <param name="content">The content to publish.</param>
        /// <returns></returns>
        private HttpRequestMessage CreatePublishRequest(
            HttpContent content)
        {
            var requestUri = $"{Host}/{GetRequestUri()}/publish";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = content
            };
            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Add("X-API-Key", _apiKey);
            }
            return request;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] Compress(
            string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                _logger?.Debug($"Json data with length {value.Length} has been compressed to {ToBytesString(bytes.Length)}.");
                return output.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string ToBytesString(
            long bytes)
        {
            switch (bytes)
            {
                case < 1024:
                    return $"{bytes} B";
                case < 1024 * 1024:
                    return $"{(bytes / 1024)} KB";
                default:
                    return $"{(bytes / (1024 * 1024))} MB";
            }
        }

        #endregion
    }
}
