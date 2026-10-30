#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Serilog;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Registers extended telemetry capture for RepoDB operations.
    /// </summary>
    public static class ExtendedTelemetryGlobalConfiguration
    {
        #region Public Methods

        /// <summary>
        /// Initializes the global extended trace using collector settings.
        /// Subsequent calls retain the original trace settings.
        /// </summary>
        /// <param name="globalConfiguration">The configuration being extended.</param>
        /// <param name="host">The collector host.</param>
        /// <param name="apiKey">The optional authentication key.</param>
        /// <param name="applicationName">The application producing telemetry.</param>
        /// <param name="groupName">The application group.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <param name="logger">The internal diagnostic logger.</param>
        /// <param name="certificateValidationCallback">The optional server certificate validator.</param>
        /// <returns>The configuration being extended.</returns>
        public static GlobalConfiguration UseExtendedTelemetry(this GlobalConfiguration globalConfiguration,
            string host,
            string apiKey,
            string applicationName,
            string groupName = "Default",
            Action<Exception> errorCallback = null,
            ILogger logger = null,
            Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> certificateValidationCallback = null)
            => globalConfiguration.UseExtendedTelemetry(new ExtendedTelemetryOption(applicationName)
            {
                ApiKey = apiKey,
                CertificateValidationCallback = certificateValidationCallback,
                Group = groupName,
                Host = host
            },
                errorCallback,
                logger);

        /// <summary>
        /// Initializes and registers the global extended trace once.
        /// Subsequent calls retain the original trace settings.
        /// </summary>
        /// <param name="globalConfiguration">The configuration being extended.</param>
        /// <param name="option">The capture and publishing settings.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <param name="logger">The internal diagnostic logger.</param>
        /// <returns>The configuration being extended.</returns>
        public static GlobalConfiguration UseExtendedTelemetry(this GlobalConfiguration globalConfiguration,
            ExtendedTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            if (globalConfiguration == null)
            {
                throw new ArgumentNullException(nameof(globalConfiguration));
            }
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }
            ExtendedTelemetryTrace.Create(option,
                errorCallback,
                logger);
            return globalConfiguration;
        }

        #endregion
    }
}

