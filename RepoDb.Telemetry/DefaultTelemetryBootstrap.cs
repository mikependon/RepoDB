using System;

namespace RepoDb.Telemetry
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings to capture the library telemetries.
    /// </summary>
    public static class DefaultTelemetryBootstrap
    {
        #region Properties

        /// <summary>
        /// Gets the value that indicates whether the initialization is completed.
        /// </summary>
        public static bool IsInitialized { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        internal static void InitializeInternal(
            string applicationName,
            TimeSpan frequency)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Initialize
            DefaultTelemetryTrace.Create(applicationName, frequency);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
