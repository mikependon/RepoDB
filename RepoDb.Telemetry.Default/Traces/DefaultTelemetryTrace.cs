using System;
using RepoDb.Telemetry.Core;
using Serilog;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is used to capture the telemetry of the library and send it to the insights solution.
    /// This is the default telemetry capturing of the library.
    /// </summary>
    public class DefaultTelemetryTrace : TelemetryTrace
    {
        /// <summary>
        /// Creates a new instance of <see cref="DefaultTelemetryTrace"/> object.
        /// </summary>
        /// <param name="option">The instance of <see cref="DefaultTelemetryOption"/> that has been passed from the <see cref="GlobalConfiguration"/>.</param>
        /// <param name="errorCallback">The callback function to call in the case of any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        public DefaultTelemetryTrace(
            DefaultTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
            : base(option, errorCallback, logger)
        { }
    }
}
