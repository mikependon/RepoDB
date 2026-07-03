using System;
using Serilog;

namespace RepoDb.Telemetry.Core.Options
{
    /// <summary>
    /// A class that is being used to define the necessary settings to capture the library telemetries.
    /// </summary>
    public class TelemetryOption
    {
        /// <summary>
        /// Gets or sets the name of the application that produces the telemetry.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the group to where the application will be categorized. This is optional and can be used to group the applications that produce the telemetry.
        /// </summary>
        public string Group { get; set; } = "Default";

        /// <summary>
        /// Gets or sets the endpoint to where to publish the telemetries.
        /// </summary>
        public string Endpoint { get; set; } = "http://localhost:5000/telemetry";

        /// <summary>
        /// Gets or sets the threshold of how often to publish the buffered telemetry.
        /// </summary>
        public TimeSpan Frequency { get; set; } = TimeSpan.FromSeconds(5);
    }
}
