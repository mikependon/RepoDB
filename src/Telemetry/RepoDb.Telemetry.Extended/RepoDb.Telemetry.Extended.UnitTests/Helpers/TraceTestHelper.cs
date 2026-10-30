#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using RepoDb.Telemetry.Core;

namespace RepoDb.Telemetry.Extended.UnitTests
{
    /// <summary>
    /// Inspects buffered events and triggers publishing without timer delays.
    /// </summary>
    internal static class TraceTestHelper
    {
        #region Public Methods

        /// <summary>
        /// Gets the event buffered for the specified command.
        /// </summary>
        /// <param name="trace">The trace under test.</param>
        /// <param name="log">The original command.</param>
        /// <returns>The buffered extended event.</returns>
        public static ExtendedTelemetryItem GetItem(ExtendedTelemetryTrace trace,
            CancellableTraceLog log)
        {
            var field = typeof(TelemetryTrace).GetField("_beforeTraceLogs",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var items = (IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>)field.GetValue(trace);
            return (ExtendedTelemetryItem)items[log.SessionId].Item2;
        }

        /// <summary>
        /// Executes one publishing cycle synchronously.
        /// </summary>
        /// <param name="trace">The trace to flush.</param>
        public static void Flush(ExtendedTelemetryTrace trace)
        {
            typeof(TelemetryTrace).GetMethod("Callback",
                BindingFlags.Instance | BindingFlags.NonPublic).Invoke(trace,
                    new object[] { null });
        }

        #endregion
    }
}

