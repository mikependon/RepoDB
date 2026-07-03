using System;
using RepoDb.Telemetry.Core.Repositories;
using Serilog;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is used to publish the telemetry data to the insights solution.
    /// </summary>
    public class DefaultTelemetryPublisherRepository : TelemetryPublisherRepository
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryPublisherRepository"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint to where to publish the telemetry data.</param>
        /// <param name="errorCallback">The callback function to call in any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        public DefaultTelemetryPublisherRepository(
            string endpoint = "http://localhost:5000/telemetry",
            Action<Exception> errorCallback = null,
            ILogger logger = null)
            : base(endpoint, errorCallback, logger) { }

        #endregion
    }
}
