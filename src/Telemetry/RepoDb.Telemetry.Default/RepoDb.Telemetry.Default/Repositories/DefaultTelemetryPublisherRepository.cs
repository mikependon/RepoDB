#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using RepoDb.Telemetry.Core;
using Serilog;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is used to publish the telemetry data to the insights solution.
    /// </summary>
    public class DefaultTelemetryPublisherRepository : TelemetryPublisherRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryPublisherRepository"/> class.
        /// </summary>
        /// <param name="host">The host to where to publish the telemetry data.</param>
        /// <param name="apiKey">The API key to be used for authentication. Leave this to empty if not provided in the collector API.</param>"
        /// <param name="errorCallback">The callback function to call in any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        /// <param name="certificateValidationCallback">An optional callback used to validate the server certificate presented by the collector API when publishing over HTTPS. Leave this to null to use the default .NET certificate validation.</param>
        public DefaultTelemetryPublisherRepository(
            string host = "http://localhost:5000",
            string apiKey = null,
            Action<Exception> errorCallback = null,
            ILogger logger = null,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certificateValidationCallback = null)
            : base(host, apiKey, errorCallback, logger, certificateValidationCallback)
        { }

        /// <summary>
        /// Gets the request URI where to publish the telemetry data. The URI will be appended after the <see cref="Host"/> to compose the target endpoint.
        /// </summary>
        /// <returns>The URI to where to publish the telemetry data.</returns>
        public override string GetRequestUri() => $"v1/telemetry/default";
    }
}
