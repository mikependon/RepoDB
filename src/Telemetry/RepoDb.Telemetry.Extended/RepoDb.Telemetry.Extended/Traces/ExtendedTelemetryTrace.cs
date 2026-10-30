#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using RepoDb.Telemetry.Core;
using Serilog;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Captures extended operation metadata and publishes it to the extended collector.
    /// </summary>
    public class ExtendedTelemetryTrace : TelemetryTrace
    {
        #region Private Variables

#if NET9_0_OR_GREATER
        private static readonly System.Threading.Lock _syncLock = new System.Threading.Lock();
#else
        private static readonly object _syncLock = new object();
#endif
        private static volatile ExtendedTelemetryTrace _instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an independently configured extended trace.
        /// </summary>
        /// <param name="option">The capture and publishing settings.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <param name="logger">The internal diagnostic logger.</param>
        public ExtendedTelemetryTrace(ExtendedTelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
            : base(option ?? throw new ArgumentNullException(nameof(option)),
                errorCallback,
                logger)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the trace initialized through global configuration.
        /// </summary>
        public static ExtendedTelemetryTrace Instance
            => _instance ?? throw new InvalidOperationException("Call UseExtendedTelemetry first.");

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the publisher using this trace's connection settings.
        /// </summary>
        /// <returns>The extended telemetry publisher.</returns>
        public override TelemetryPublisherRepository GetPublisherRepository()
            => new ExtendedTelemetryPublisherRepository(Option.Host,
                Option.ApiKey,
                ErrorCallback,
                Logger,
                Option.CertificateValidationCallback);

        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes and registers the global trace once.
        /// </summary>
        /// <param name="option">The capture and publishing settings.</param>
        /// <param name="errorCallback">The publishing error callback.</param>
        /// <param name="logger">The internal diagnostic logger.</param>
        /// <returns>The registered singleton trace.</returns>
        internal static ExtendedTelemetryTrace Create(ExtendedTelemetryOption option,
            Action<Exception> errorCallback,
            ILogger logger)
        {
            lock (_syncLock)
            {
                if (_instance == null)
                {
                    var trace = new ExtendedTelemetryTrace(option,
                        errorCallback,
                        logger);
                    GlobalTraceRegistration.Register(trace);
                    trace.Start();
                    _instance = trace;
                }
                return _instance;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Captures correlation identifiers and enriches the event on the calling thread.
        /// </summary>
        /// <param name="log">The command trace log.</param>
        /// <returns>The enriched event.</returns>
        protected override TelemetryItem CreateTelemetryItem(CancellableTraceLog log)
        {
            var activity = Activity.Current;
            var item = new ExtendedTelemetryItem
            {
                Operation = log.Key,
                ParameterCount = log.Parameters?.Count() ?? 0,
                SpanId = activity?.IdFormat == ActivityIdFormat.W3C ? activity.SpanId.ToString() : null,
                ThreadId = Environment.CurrentManagedThreadId,
                TraceId = activity?.IdFormat == ActivityIdFormat.W3C ? activity.TraceId.ToString() : null
            };
            ((ExtendedTelemetryOption)Option).Enrich?.Invoke(item,
                log);
            return item;
        }

        /// <summary>
        /// Records known row counts without enumerating or consuming operation results.
        /// </summary>
        /// <param name="item">The event being completed.</param>
        /// <param name="result">The operation result.</param>
        protected override void CompleteTelemetryItem(TelemetryItem item,
            object result)
        {
            var extended = (ExtendedTelemetryItem)item;
            if (result is ICollection collection)
            {
                extended.RowsReturned = collection.Count;
            }
            else if (IsWriteOperation(item.Operation) && (result is int || result is long))
            {
                extended.RowsAffected = Convert.ToInt64(result,
                    System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Identifies operations whose numeric result represents affected rows.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <returns>Whether the operation returns an affected row count.</returns>
        private static bool IsWriteOperation(string operation)
            => operation is "Delete" or "DeleteAll" or "InsertAll" or
                "Update" or "UpdateAll" or "MergeAll";

        #endregion
    }
}

