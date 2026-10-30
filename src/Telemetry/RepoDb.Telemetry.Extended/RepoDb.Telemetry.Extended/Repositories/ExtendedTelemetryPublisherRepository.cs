#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using RepoDb.Telemetry.Core;
using Serilog;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Publishes extended telemetry using the shared compression and authentication protocol.
    /// </summary>
    public class ExtendedTelemetryPublisherRepository : TelemetryPublisherRepository
    {
        #region Constructors

        /// <summary>
        /// Creates a publisher for the extended collector endpoint.
        /// </summary>
        /// <param name="host">The collector host.</param>
        /// <param name="apiKey">The optional authentication key.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <param name="logger">The internal diagnostic logger.</param>
        /// <param name="certificateValidationCallback">The optional server certificate validator.</param>
        public ExtendedTelemetryPublisherRepository(string host = "http://localhost:5000",
            string apiKey = null,
            Action<Exception> errorCallback = null,
            ILogger logger = null,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certificateValidationCallback = null)
            : base(host,
                apiKey,
                errorCallback,
                logger,
                certificateValidationCallback)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the relative extended telemetry endpoint.
        /// </summary>
        /// <returns>The extended telemetry route.</returns>
        public override string GetRequestUri() => "v1/telemetry/extended";

        #endregion

        #region Protected Methods

        /// <summary>
        /// Serializes all extended fields using generated metadata.
        /// </summary>
        /// <param name="telemetryItems">The extended events to publish.</param>
        /// <returns>The complete JSON event array.</returns>
        protected override string SerializeItems(IEnumerable<TelemetryItem> telemetryItems)
            => JsonSerializer.Serialize(telemetryItems.Cast<ExtendedTelemetryItem>(),
                ExtendedTelemetryJsonSerializerContext.Default.IEnumerableExtendedTelemetryItem);

        #endregion
    }
}

