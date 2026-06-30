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
        /// <param name="endpoint"></param>
        /// <param name="applicationName"></param>
        /// <param name="frequency"></param>
        /// <param name="errorCallback"></param>
        internal static void InitializeInternal(
            string endpoint,
            string applicationName,
            TimeSpan frequency,
            Action<Exception> errorCallback = null)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Initialize
            DefaultTelemetryTrace.Create(endpoint, applicationName, frequency, errorCallback);

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
