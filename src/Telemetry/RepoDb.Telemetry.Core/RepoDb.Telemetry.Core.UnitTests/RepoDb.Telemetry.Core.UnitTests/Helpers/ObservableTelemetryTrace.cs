#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RepoDb.Telemetry.Core.UnitTests.Helpers
{
    /// <summary>
    /// A trace that remembers the telemetry items that it creates and how it is told that each operation was completed, so the base behavior can be tested.
    /// </summary>
    internal sealed class ObservableTelemetryTrace : TelemetryTrace
    {
        #region Constructors

        /// <summary>
        /// Creates a trace of an application.
        /// </summary>
        public ObservableTelemetryTrace()
            : base(new TelemetryOption("Test"))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the telemetry items that were created, in the order that they were created.
        /// </summary>
        public List<TelemetryItem> Items { get; } = new List<TelemetryItem>();

        /// <summary>
        /// Gets the outcomes that the trace was told, in the order that it was told.
        /// </summary>
        public List<Tuple<object, Exception, TraceLevel>> Outcomes { get; } = new List<Tuple<object, Exception, TraceLevel>>();

        /// <summary>
        /// Gets the results that the items were completed with.
        /// </summary>
        public List<object> Results { get; } = new List<object>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the repository that would publish the items.
        /// </summary>
        /// <returns>A repository that is not used.</returns>
        public override TelemetryPublisherRepository GetPublisherRepository() =>
            new ObservableTelemetryPublisherRepository();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the telemetry item of the operation, and remembers it.
        /// </summary>
        /// <param name="log">The log of the operation.</param>
        /// <returns>The telemetry item.</returns>
        protected override TelemetryItem CreateTelemetryItem(CancellableTraceLog log)
        {
            var item = base.CreateTelemetryItem(log);
            Items.Add(item);
            return item;
        }

        /// <summary>
        /// Remembers the result that the item was completed with.
        /// </summary>
        /// <param name="item">The telemetry item.</param>
        /// <param name="result">The result of the operation.</param>
        protected override void CompleteTelemetryItem(TelemetryItem item,
            object result)
        {
            Results.Add(result);
        }

        /// <summary>
        /// Remembers the outcome that the trace was told, and completes the item as the base does.
        /// </summary>
        /// <param name="item">The telemetry item.</param>
        /// <param name="result">The result of the operation.</param>
        /// <param name="error">The exception that made the operation fail, or <c>null</c>.</param>
        /// <param name="level">The level of the log of the operation.</param>
        protected override void CompleteTelemetryItem(TelemetryItem item,
            object result,
            Exception error,
            TraceLevel level)
        {
            Outcomes.Add(Tuple.Create(result,
                error,
                level));
            base.CompleteTelemetryItem(item,
                result,
                error,
                level);
        }

        #endregion
    }
}
