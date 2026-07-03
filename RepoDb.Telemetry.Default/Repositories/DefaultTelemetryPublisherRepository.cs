using System;
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
        /// <param name="errorCallback">The callback function to call in any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        public DefaultTelemetryPublisherRepository(
            string host = "http://localhost:5000",
            Action<Exception> errorCallback = null,
            ILogger logger = null)
            : base(host, errorCallback, logger)
        { }
    }
}
