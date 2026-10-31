#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Diagnostics;
using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Exposes the protected completion of an extended telemetry item, so it can be tested without going through a whole operation.
    /// </summary>
    internal sealed class ObservableExtendedTelemetryTrace : ExtendedTelemetryTrace
    {
        #region Constructors

        /// <summary>
        /// Creates a trace of an application.
        /// </summary>
        /// <param name="application">The name of the application.</param>
        public ObservableExtendedTelemetryTrace(string application = "Test")
            : base(new ExtendedTelemetryOption(application))
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Completes the item with the outcome of an operation.
        /// </summary>
        /// <param name="item">The item to be completed.</param>
        /// <param name="result">The result of the operation.</param>
        /// <param name="error">The exception that made the operation fail, or <c>null</c>.</param>
        /// <param name="level">The level of the log of the operation.</param>
        public void Complete(TelemetryItem item,
            object result,
            Exception error,
            TraceLevel level)
        {
            CompleteTelemetryItem(item,
                result,
                error,
                level);
        }

        #endregion
    }
}
