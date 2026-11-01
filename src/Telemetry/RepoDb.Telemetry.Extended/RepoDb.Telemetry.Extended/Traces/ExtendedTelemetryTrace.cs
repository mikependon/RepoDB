#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using RepoDb.Telemetry.Core;
using Serilog;
using Serilog.Events;

namespace RepoDb.Telemetry.Extended
{
    /// <summary>
    /// Captures extended operation metadata and publishes it to the extended collector.
    /// </summary>
    public class ExtendedTelemetryTrace : TelemetryTrace
    {
        #region Private Variables

        private static volatile ExtendedTelemetryTrace _instance;
#if NET9_0_OR_GREATER
        private static readonly System.Threading.Lock _syncLock = new System.Threading.Lock();
#else
        private static readonly object _syncLock = new object();
#endif

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
        public static ExtendedTelemetryTrace Instance => _instance ?? throw new InvalidOperationException("Call UseExtendedTelemetry first.");

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the publisher using this trace's connection settings.
        /// </summary>
        /// <returns>The extended telemetry publisher.</returns>
        public override TelemetryPublisherRepository GetPublisherRepository() => new ExtendedTelemetryPublisherRepository(Option.Host,
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
            var parameters = log.Parameters?.ToArray();
            var item = CreateContextItem(log);
            item.ParameterCount = parameters?.Length ?? 0;
            item.Parameters = parameters?.Select(CreateParameter).ToList();
            ((ExtendedTelemetryOption)Option).Enrich?.Invoke(item,
                log);
            return item;
        }

        /// <summary>
        /// Records the final severity, error, and result metadata.
        /// </summary>
        /// <param name="item">The event being completed.</param>
        /// <param name="result">The operation result (the default value if the operation has failed).</param>
        /// <param name="error">The exception that made the operation fail, or <c>null</c> if it succeeded.</param>
        /// <param name="level">The level of the log of the operation.</param>
        protected override void CompleteTelemetryItem(TelemetryItem item,
            object result,
            Exception error,
            TraceLevel level)
        {
            var extended = (ExtendedTelemetryItem)item;
            extended.TraceLevel = level;
            if (error != null)
            {
                SetError(extended,
                    error);
                return;
            }

            if (level != TraceLevel.Info)
            {
                extended.Level = ToLogEventLevel(level);
            }

            base.CompleteTelemetryItem(item,
                result,
                error,
                level);
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
            extended.ResultType = result?.GetType().FullName;
            if (((ExtendedTelemetryOption)Option).CaptureResultValues)
            {
                extended.Result = SnapshotValue(result);
            }

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
        /// Copies the operation context.
        /// </summary>
        /// <param name="log">The command log.</param>
        /// <returns>The context event.</returns>
        private static ExtendedTelemetryItem CreateContextItem(CancellableTraceLog log)
        {
            var activity = Activity.Current;
            var isW3C = activity?.IdFormat == ActivityIdFormat.W3C;
            return new ExtendedTelemetryItem
            {
                ConnectionType = log.ConnectionType,
                Database = log.Database,
                Operation = log.Key,
                TraceLevel = log.Level,
                SpanId = log.SpanId ?? (isW3C ? activity.SpanId.ToString() : null),
                ThreadId = log.ThreadId != 0 ? log.ThreadId : Environment.CurrentManagedThreadId,
                TraceId = log.TraceId ?? (isW3C ? activity.TraceId.ToString() : null)
            };
        }

        /// <summary>
        /// Copies parameter metadata and its value.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>The parameter snapshot.</returns>
        private TelemetryParameter CreateParameter(IDbDataParameter parameter)
        {
            if (parameter == null)
            {
                return null;
            }

            return new TelemetryParameter
            {
                Name = parameter.ParameterName,
                DbType = parameter.DbType,
                Direction = parameter.Direction,
                IsNullable = parameter.IsNullable,
                Size = parameter.Size,
                Precision = parameter.Precision,
                Scale = parameter.Scale,
                SourceColumn = parameter.SourceColumn,
                SourceVersion = parameter.SourceVersion,
                ValueType = parameter.Value?.GetType().FullName,
                Value = ((ExtendedTelemetryOption)Option).CaptureParameterValues ? SnapshotValue(parameter.Value) : null
            };
        }

        /// <summary>
        /// Captures a value using generated metadata.
        /// </summary>
        /// <param name="value">The value to capture.</param>
        /// <returns>The captured JSON value.</returns>
        private JsonElement? SnapshotValue(object value) => TelemetryValueSnapshot.Capture(value,
            (ExtendedTelemetryOption)Option,
            Logger);

        /// <summary>
        /// Records the exception that made the operation fail. The rows are not recorded, as the operation did not complete.
        /// The message of the item is not replaced if the application has already given it one.
        /// </summary>
        /// <param name="item">The telemetry item of the operation.</param>
        /// <param name="error">The exception that made the operation fail.</param>
        private static void SetError(ExtendedTelemetryItem item,
            Exception error)
        {
            item.Level = LogEventLevel.Error;
            item.ErrorType = error.GetType().FullName;
            item.ErrorMessage = error.Message;
            item.ErrorStackTrace = error.StackTrace;
            item.Message = item.Message ?? error.Message;
        }

        /// <summary>
        /// Gets the level of the logging that is equivalent to the level of the trace.
        /// </summary>
        /// <param name="level">The level of the trace.</param>
        /// <returns>The equivalent level of the logging.</returns>
        private static LogEventLevel ToLogEventLevel(TraceLevel level)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    return LogEventLevel.Error;
                case TraceLevel.Warning:
                    return LogEventLevel.Warning;
                case TraceLevel.Verbose:
                    return LogEventLevel.Verbose;
                default:
                    return LogEventLevel.Information;
            }
        }

        /// <summary>
        /// Identifies operations whose numeric result represents affected rows.
        /// </summary>
        /// <param name="operation">The operation name.</param>
        /// <returns>Whether the operation returns an affected row count.</returns>
        private static bool IsWriteOperation(string operation) => operation is "Delete" or "DeleteAll" or "InsertAll" or "Update" or "UpdateAll" or "MergeAll";

        #endregion
    }
}
