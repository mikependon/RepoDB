using System;
using Serilog;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings to capture the library telemetries.
    /// </summary>
    public static partial class DefaultTelemetryGlobalConfiguration
    {
        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="host">The host to where to publish the telemetries.</param>
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="frequency">The threshold of how often to publish the buffered telemetry.</param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <param name="logger">An optional logger instance to log any internal errors.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string host,
            string applicationName,
            TimeSpan frequency,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            return globalConfiguration.UseDefaultTelemetry(new DefaultTelemetryOption(applicationName)
            {
                Host = host,
                Frequency = frequency
            },
            errorCallback, logger);
        }

        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="option">The option that defines the necessary settings to capture the library telemetries.</param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <param name="logger">An optional logger instance to log any internal errors.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            DefaultTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            DefaultTelemetryBootstrap.InitializeInternal(option, errorCallback, logger);
            return globalConfiguration;
        }
    }
}
