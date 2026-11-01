#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// A class that is used to publish the telemetry data to the insights solution.
    /// </summary>
    public abstract class TelemetryPublisherRepository : IPublisherRepository
    {
        #region Private Variables

        private readonly string _apiKey;
        private static readonly HttpClient _defaultHttpClient = new HttpClient();
        private readonly Action<Exception> _errorCallback;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryPublisherRepository"/> class.
        /// </summary>
        /// <param name="host">The host of the telemetry collector API.</param>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <param name="errorCallback">The callback to invoke when an error occurs.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <param name="certificateValidationCallback">The callback to validate SSL certificates.</param>
        protected TelemetryPublisherRepository(string host = "http://localhost:5000",
            string apiKey = null,
            Action<Exception> errorCallback = null,
            ILogger logger = null,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certificateValidationCallback = null)
        {
            Host = host;
            _apiKey = apiKey;
            _errorCallback = errorCallback;
            _logger = logger;
#pragma warning disable MA0039 // Do not write your own certificate validation method

            _httpClient = certificateValidationCallback == null ? _defaultHttpClient : new HttpClient(new HttpClientHandler {
                ServerCertificateCustomValidationCallback = certificateValidationCallback
            });
#pragma warning restore MA0039 // Do not write your own certificate validation method

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the host of the teleemtry collector API.
        /// </summary>
        public string Host { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the request URI where to publish the telemetry data. The URI will be appended after the <see cref="Host"/> to compose the target endpoint.
        /// </summary>
        /// <returns>The URI to where to publish the telemetry data.</returns>
        public abstract string GetRequestUri();

        /// <summary>
        /// Publishes the telemetry data to the insights solution.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        public void Publish(TelemetryItem telemetryItem) => PublishMany(new[] { telemetryItem });

        /// <summary>
        /// Publishes the telemetry data to the insights solution.
        /// </summary>
        /// <param name="telemetryItem">The telemetry data to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public async Task PublishAsync(TelemetryItem telemetryItem,
            CancellationToken cancellationToken = default) => await PublishManyAsync(new[] { telemetryItem },
            cancellationToken).ConfigureAwait(false);

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        public void PublishMany(IEnumerable<TelemetryItem> telemetryItems)
        {
            PublishManyAsync(telemetryItems).GetAwaiter().GetResult();
        }

        /// <summary>
        /// A method that is used to publish multiple telemetry items to the insights solution in an asynchronous way.
        /// </summary>
        /// <param name="telemetryItems">The telemetry items to publish.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public async Task PublishManyAsync(IEnumerable<TelemetryItem> telemetryItems,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await SendManyAsync(telemetryItems,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception error)
            {
                HandlePublishingError(error);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Serializes the items using the metadata appropriate to the telemetry package.
        /// </summary>
        /// <param name="telemetryItems">The items to serialize.</param>
        /// <returns>The JSON payload before compression.</returns>
        protected virtual string SerializeItems(IEnumerable<TelemetryItem> telemetryItems) => JsonSerializer.Serialize(telemetryItems,
            TelemetryJsonSerializerContext.Default.IEnumerableTelemetryItem);

        #endregion

        #region Private Methods

        /// <summary>
        /// Compresses and sends a telemetry batch.
        /// </summary>
        /// <param name="telemetryItems">The events to publish.</param>
        /// <param name="cancellationToken">The request cancellation token.</param>
        /// <returns>The publishing task.</returns>
        private async Task SendManyAsync(IEnumerable<TelemetryItem> telemetryItems,
            CancellationToken cancellationToken)
        {
            using var content = CreateCompressedContent(ToCompressedJsonBytes(telemetryItems));
            using var request = CreatePublishRequest(content);
            _logger?.Debug("Publishing telemetry data to {Host}.",
                Host);
            using var response = await _httpClient.SendAsync(request,
                cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            _logger?.Information("{Count} telemetry data has been published.",
                telemetryItems.Count());
        }

        /// <summary>
        /// Reports a publishing failure.
        /// </summary>
        /// <param name="error">The publishing exception.</param>
        private void HandlePublishingError(Exception error)
        {
            var failure = new InvalidOperationException("Failed to publish the telemetry data.",
                error);
            _logger?.Error(failure,
                "Failed to publish the telemetry data.");
            _errorCallback?.Invoke(failure);
        }

        /// <summary>
        /// Serializes the telemetry items to JSON and gzip-compresses the resulting bytes.
        /// </summary>
        /// <param name="telemetryItems">The events to serialize.</param>
        /// <returns>The gzip-compressed JSON bytes.</returns>
        private byte[] ToCompressedJsonBytes(IEnumerable<TelemetryItem> telemetryItems)
        {
            var json = SerializeItems(telemetryItems);
            return Compress(json);
        }

        /// <summary>
        /// Encodes compressed bytes as an HTTP body.
        /// </summary>
        /// <param name="compressed">The gzip-compressed bytes.</param>
        /// <returns>The Base64 request content.</returns>
        private HttpContent CreateCompressedContent(byte[] compressed)
        {
            var base64 = Convert.ToBase64String(compressed);
            var content = new StringContent(base64,
                Encoding.UTF8,
                "text/plain");
            return content;
        }

        /// <summary>
        /// Creates an authenticated publishing request.
        /// </summary>
        /// <param name="content">The content to publish.</param>
        /// <returns>The publishing request.</returns>
        private HttpRequestMessage CreatePublishRequest(HttpContent content)
        {
            var requestUri = $"{Host}/{GetRequestUri()}/publish";
            var request = new HttpRequestMessage(HttpMethod.Post,
                requestUri)
            {
                Content = content
            };
            if (!string.IsNullOrEmpty(_apiKey))
            {
                request.Headers.Add("X-API-Key",
                    _apiKey);
            }

            return request;
        }

        /// <summary>
        /// Gzip-compresses UTF-8 JSON.
        /// </summary>
        /// <param name="value">The JSON text.</param>
        /// <returns>The compressed bytes.</returns>
        private byte[] Compress(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output,
                    CompressionLevel.Optimal))
                {
                    gzip.Write(bytes,
                        0,
                        bytes.Length);
                }

                var compressed = output.ToArray();
                _logger?.Debug("JSON data of {OriginalSize} has been gzip-compressed to {CompressedSize}.",
                    ToBytesString(bytes.Length),
                    ToBytesString(compressed.Length));
                return compressed;
            }
        }

        /// <summary>
        /// Formats a payload size.
        /// </summary>
        /// <param name="bytes">The byte count.</param>
        /// <returns>The readable payload size.</returns>
        private string ToBytesString(long bytes)
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
