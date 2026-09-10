#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// A class that is being used to define the necessary settings to capture the library telemetries.
    /// </summary>
    public class TelemetryOption
    {
        /// <summary>
        /// Creates a new instance of <see cref="TelemetryOption"/> object.
        /// </summary>
        /// <param name="application">The name of the application that produces the telemetry.</param>
        public TelemetryOption(
            string application)
        {
            Application = application;
        }

        /// <summary>
        /// Gets the name of the application that produces the telemetry.
        /// </summary>
        public string Application { get; }

        /// <summary>
        /// Gets or sets the group to where the application will be categorized. This is optional and can be used to group the applications that produce the telemetry.
        /// </summary>
        public string Group { get; set; } = "Default";

        /// <summary>
        /// Gets or sets the host to where to publish the telemetries.
        /// </summary>
        public string Host { get; set; } = "http://localhost:5000";

        /// <summary>
        /// The API key to be used for authentication. Leave this to empty if not provided in the collector API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the threshold of how often to publish the buffered telemetry.
        /// </summary>
        public TimeSpan Frequency { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Gets or sets the callback used to validate the server certificate presented by the collector API
        /// when publishing over HTTPS. Leave this to null to use the default .NET certificate validation.
        /// This is useful when the collector API is deployed with a self-signed or otherwise untrusted certificate.
        /// </summary>
        public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> CertificateValidationCallback { get; set; }
    }
}
