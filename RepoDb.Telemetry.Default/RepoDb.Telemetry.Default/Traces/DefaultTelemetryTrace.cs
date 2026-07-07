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
        #region Privates

        private static readonly object _syncLock = new object();
        private static DefaultTelemetryTrace _instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="DefaultTelemetryTrace"/> object.
        /// </summary>
        /// <param name="option">The instance of <see cref="DefaultTelemetryOption"/> that has been passed from the <see cref="GlobalConfiguration"/>.</param>
        /// <param name="errorCallback">The callback function to call in the case of any exception.</param>
        /// <param name="logger">The logger instance to use when logging messages or events.</param>
        internal DefaultTelemetryTrace(
            DefaultTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
            : base(option, errorCallback, logger)
        { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton instance of <see cref="DefaultTelemetryTrace"/> object.
        /// </summary>
        public static DefaultTelemetryTrace Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException("The instance has not been created. Please call the 'Create' method first.");
                }
                return _instance;
            }
        }


        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="DefaultTelemetryTrace"/> object.
        /// </summary>
        /// <param name="option">The option that defines the necessary settings to capture the library telemetries.</param>
        /// <param name="errorCallback">An optional callback invoked with any exception that occurs internally (e.g. during telemetry publishing).</param>
        /// <param name="logger">An optional logger instance to log any internal errors.</param>
        internal static DefaultTelemetryTrace Create(
            DefaultTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    _instance = new DefaultTelemetryTrace(option, errorCallback, logger);
                }
            }
            return _instance;
        }

        #endregion
    }
}
