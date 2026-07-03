using System;

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
        /// Gets or sets the threshold of how often to publish the buffered telemetry.
        /// </summary>
        public TimeSpan Frequency { get; set; } = TimeSpan.FromSeconds(5);
    }
}
