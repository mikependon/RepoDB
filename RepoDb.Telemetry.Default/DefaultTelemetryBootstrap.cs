using System;
using Serilog;

namespace RepoDb.Telemetry.Default
{
    /// <summary>
    /// A class that is being used to initialize the necessary settings to capture the library telemetries.
    /// </summary>
    internal static class DefaultTelemetryBootstrap
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
        /// <param name="option"></param>
        public static void InitializeInternal(
            DefaultTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            // Skip if already initialized
            if (IsInitialized == true)
            {
                return;
            }

            // Initialize
            new DefaultTelemetryTrace(option).Start();

            // Set the flag
            IsInitialized = true;
        }

        #endregion
    }
}
