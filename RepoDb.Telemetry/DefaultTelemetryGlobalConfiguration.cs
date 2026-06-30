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
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="frequencyInSeconds">The threshold of how often to publish the buffered telemetry.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string applicationName,
            int frequencyInSeconds = 5)
        {
            DefaultTelemetryBootstrap.InitializeInternal(applicationName, TimeSpan.FromSeconds(5));
            return globalConfiguration;
        }

        /// <summary>
        /// Initializes all the necessary settings for SQL Server.
        /// </summary>
        /// <param name="globalConfiguration">The instance of the global configuration in used.</param>
        /// <param name="applicationName">The name of the application that produces the telemetry.</param>
        /// <param name="frequency">The threshold of how often to publish the buffered telemetry.</param>
        /// <returns>The used global configuration instance itself.</returns>
        public static GlobalConfiguration UseDefaultTelemetry(
            this GlobalConfiguration globalConfiguration,
            string applicationName,
            TimeSpan frequency)
        {
            DefaultTelemetryBootstrap.InitializeInternal(applicationName, frequency);
            return globalConfiguration;
        }
    }
}
