using System;

namespace RepoDb.Telemetry
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
        /// <param name="endpoint">The endpoint to where to publish the telemetries.</param>
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="frequencyInSeconds">The threshold of how often to publish the buffered telemetry.</param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string endpoint,
            string applicationName,
            int frequencyInSeconds = 5,
            Action<Exception> errorCallback = null)
        {
            DefaultTelemetryBootstrap.InitializeInternal(endpoint, applicationName, TimeSpan.FromSeconds(frequencyInSeconds), errorCallback);
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="endpoint">The endpoint to where to publish the telemetries.</param>
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="frequency">The threshold of how often to publish the buffered telemetry.</param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string endpoint,
            string applicationName,
            TimeSpan frequency,
            Action<Exception> errorCallback = null)
        {
            DefaultTelemetryBootstrap.InitializeInternal(endpoint, applicationName, frequency, errorCallback);
            return globalConfiguration;
        }
    }
}
