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
        /// Initializes all the necessary settings for capturing the telemetries.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="host">The host to where to publish the telemetries.</param>
        /// <param name="apiKey">The API key to be used for authentication. Leave this to empty if not provided in the collector API.</param>"
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="groupName">
        /// The name of the group to where the application will be placed.
        /// This is very useful on the visualization dashboard for organization the things.
        /// By default, all application is grouped to 'Default' group.
        /// </param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <param name="logger">An optional logger instance to log any internal errors.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string host,
            string apiKey,
            string applicationName,
            string groupName = "Default",
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            return globalConfiguration.UseDefaultTelemetry(new DefaultTelemetryOption(applicationName)
            {
                ApiKey = apiKey,
                Group = groupName,
                Host = host
            },
            errorCallback, logger);
        }

        /// <summary>
        /// Initializes all the necessary settings for capturing the telemetries.
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
            DefaultTelemetryTrace.Create(option, errorCallback, logger).Start();
            return globalConfiguration;
        }
    }
}
