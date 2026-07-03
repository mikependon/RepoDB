using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
    public class TelemetryPublisherRepository : IPublisherRepository
    {
        #region Privates

        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _host;
        private readonly Action<Exception> _errorCallback;
        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryPublisherRepository"/> class.
        /// </summary>
        /// <param name="host">The host to where to publish the telemetry data.</param>
        /// <param name="errorCallback">The callback function to call in the case of any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        public TelemetryPublisherRepository(
            string host = "http://localhost:5000",
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            _host = host;
            _errorCallback = errorCallback;
            _logger = logger;
        }

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
                var base64 = ToBase64JsonString(telemetryItems);
                using (var content = new StringContent(base64, Encoding.UTF8, "application/json"))
                {
                    _logger.Debug("Publishing telemetry data to {Host}.", _host);
                    _httpClient
                        .PostAsync ($"{_host}/telemetry/publish", content)
                        .GetAwaiter()
                        .GetResult();
                    _logger.Debug("{Count} data has been published.", telemetryItems.Count());
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
                var base64 = ToBase64JsonString(telemetryItems);
                using (var content = new StringContent(base64, Encoding.UTF8, "application/json"))
                {
                    _logger.Debug("Publishing telemetry data to {Host}.", _host);
                    await _httpClient
                        .PostAsync($"{_host}/telemetry/publish", content, cancellationToken);
                    _logger.Debug("{Count} data has been published.", telemetryItems.Count());
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
        /// 
        /// </summary>
        /// <param name="telemetryItems"></param>
        /// <returns></returns>
        private string ToBase64JsonString(
            IEnumerable<TelemetryItem> telemetryItems)
        {
            var json = JsonSerializer.Serialize(telemetryItems);
            var compressed = Compress(json);
            return Convert.ToBase64String(compressed);
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
                _logger.Debug("Json data with length {Length} has been compressed to {Bytes}.", value.Length, ToBytesString(output.Length));
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
