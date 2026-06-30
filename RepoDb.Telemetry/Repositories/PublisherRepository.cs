using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RepoDb.Telemetry.Default.Models;

namespace RepoDb.Telemetry.Default.Repositories
{
    /// <summary>
    ///
    /// </summary>
    internal class PublisherRepository
    {
        #region Privates

        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly string _host;

        #endregion

        #region Constructors

        /// <summary>
        ///
        /// </summary>
        /// <param name="host"></param>
        public PublisherRepository(
            string host)
        {
            _host = host;
        }

        #endregion

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="telemetryItem"></param>
        public void Publish(DefaultTelemetryItem telemetryItem) =>
            PublishMany(new[] { telemetryItem });

        /// <summary>
        ///
        /// </summary>
        /// <param name="telemetryItems"></param>
        public void PublishMany(IEnumerable<DefaultTelemetryItem> telemetryItems)
        {
            var json = JsonSerializer.Serialize(telemetryItems);
            var compressed = Compress(json);
            var base64 = Convert.ToBase64String(compressed);

            using (var content = new StringContent(base64, Encoding.UTF8, "application/json"))
            {
                _httpClient
                    .PostAsync($"{_host}/telemetry/publish", content)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        #endregion

        #region Helpers

        private static byte[] Compress(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                return output.ToArray();
            }
        }

        #endregion
    }
}
