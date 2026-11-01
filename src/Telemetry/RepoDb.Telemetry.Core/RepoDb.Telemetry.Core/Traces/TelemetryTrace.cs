#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using Serilog;

namespace RepoDb.Telemetry.Core
{
    /// <summary>
    /// A class that is used to capture the telemetry of the library and send it to the insights solution.
    /// This is the default telemetry capturing of the library.
    /// </summary>
    public abstract class TelemetryTrace : ITrace
    {
        #region Private Variables

        private IDictionary<Guid, Tuple<DateTime, TimeSpan, object>> _afterTraceLogs;
        // The assembly that hosts/consumes the library (i.e. the client application), falling back to the
        // immediate caller if an entry assembly cannot be resolved (e.g. some test hosts or plugin scenarios).
        private static readonly Assembly _assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        private IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>> _beforeTraceLogs;
        // The machine/host name on which the application is running. Resolved once and cached, since it
        // does not change during the lifetime of the process.
        private static readonly string _client = GetClientMachineName();
        private int _isPublishing;
        private IPublisherRepository _publisherRepository;
#if NET9_0_OR_GREATER
        private static readonly System.Threading.Lock _syncLock = new System.Threading.Lock();
#else
        private static readonly object _syncLock = new object();
#endif
        private readonly Timer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryTrace"/> class.
        /// </summary>
        /// <param name="option">The telemetry option.</param>
        /// <param name="errorCallback">The callback to invoke when an error occurs.</param>
        /// <param name="logger">The logger to use for logging.</param>
        protected TelemetryTrace(TelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            Option = option;
            ErrorCallback = errorCallback;
            Logger = logger;
            _beforeTraceLogs = new Dictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>();
            _afterTraceLogs = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
            _timer = new Timer(callback: Callback);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the option object that contains the information about the telemetry emitting.
        /// </summary>
        public TelemetryOption Option { get; }

        /// <summary>
        /// Gets the callback function to trigger when any error occurs.
        /// </summary>
        public Action<Exception> ErrorCallback { get; }

        /// <summary>
        /// Gets the instance of logger to use to where to log the internal events.
        /// </summary>
        public ILogger Logger { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the telemetry trace to capture the telemetry data and send it to the insights solution.
        /// </summary>
        public void Start()
        {
            _timer.Change(Option.Frequency,
                Option.Frequency);
        }

        /// <summary>
        /// The method that is called before the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="CancellableTraceLog"/> class to log.</param>
        public void BeforeExecution(CancellableTraceLog log)
        {
            AddCancellableTraceLog(log);
        }

        /// <summary>
        /// The method that is called before the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="CancellableTraceLog"/> class to log.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            AddCancellableTraceLog(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The method that is called after the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="ResultTraceLog{TResult}"/> class to log.</param>
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            AddResultTraceLog(log);
        }

        /// <summary>
        /// The method that is called after the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="ResultTraceLog{TResult}"/> class to log.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AddResultTraceLog(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the instance of <see cref="TelemetryPublisherRepository"/> that is used for publishing the telemetry data.
        /// </summary>
        /// <returns>Returns the instance of <see cref="TelemetryPublisherRepository"/> class.</returns>
        public abstract TelemetryPublisherRepository GetPublisherRepository();

        #endregion

        #region Protected Methods

        /// <summary>
        /// Publishes completed and cancelled operations.
        /// </summary>
        protected void PublishBufferedItems() => Callback(null);

        /// <summary>
        /// Creates the telemetry item for an operation before execution.
        /// </summary>
        /// <param name="log">The operation being captured.</param>
        /// <returns>The telemetry item to populate and publish.</returns>
        protected virtual TelemetryItem CreateTelemetryItem(CancellableTraceLog log) => new TelemetryItem();

        /// <summary>
        /// Captures result metadata without enumerating deferred results.
        /// </summary>
        /// <param name="item">The telemetry item being completed.</param>
        /// <param name="result">The operation result.</param>
        protected virtual void CompleteTelemetryItem(TelemetryItem item,
            object result)
        {
        }

        /// <summary>
        /// Completes the telemetry item with the outcome of the operation, whether it succeeded or failed. If it succeeded, the item is completed with its result.
        /// If it failed, the result is not a result of the operation, so the item is not completed with it: override the method to record the error.
        /// </summary>
        /// <param name="item">The telemetry item to be completed.</param>
        /// <param name="result">The result of the operation (the default value if it failed).</param>
        /// <param name="error">The exception that made the operation fail, or <c>null</c> if it succeeded.</param>
        /// <param name="level">The level of the log of the operation.</param>
        protected virtual void CompleteTelemetryItem(TelemetryItem item,
            object result,
            Exception error,
            TraceLevel level)
        {
            if (error == null)
            {
                CompleteTelemetryItem(item,
                    result);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Publishes ready operations with final timing and cancellation.
        /// </summary>
        private void PublishReadyOperations()
        {
            (var logs, var results) = Switch();
            var items = new List<TelemetryItem>();
            foreach (var entry in logs)
            {
                var item = entry.Value.Item2;
                item.IsCancelled = entry.Value.Item1.IsCancelled;
                item.IsThrowException = entry.Value.Item1.IsThrowException;
                item.Elapsed = results.TryGetValue(entry.Key,
                    out var result) ? result.Item2.TotalMilliseconds : (DateTime.UtcNow - item.StartTime).TotalMilliseconds;
                items.Add(item);
            }

            if (items.Count > 0)
            {
                EnsurePublisherRepository().PublishMany(items);
            }
        }

        /// <summary>
        /// Selects completed and cancelled command logs.
        /// </summary>
        /// <returns>The ready command logs.</returns>
        private IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>> GetReadyLogs()
        {
            var logs = new Dictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>();
            foreach (var entry in _beforeTraceLogs)
            {
                if (_afterTraceLogs.ContainsKey(entry.Key) || entry.Value.Item1.IsCancelled)
                {
                    logs.Add(entry.Key,
                        entry.Value);
                }
            }

            return logs;
        }

        /// <summary>
        /// Removes ready logs and returns their outcomes.
        /// </summary>
        /// <param name="keys">The ready session identifiers.</param>
        /// <returns>The ready outcomes.</returns>
        private IDictionary<Guid, Tuple<DateTime, TimeSpan, object>> RemoveReadyResults(IEnumerable<Guid> keys)
        {
            var results = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
            foreach (var key in keys)
            {
                if (_afterTraceLogs.TryGetValue(key,
                    out var outcome))
                {
                    results.Add(key,
                        outcome);
                }

                _beforeTraceLogs.Remove(key);
                _afterTraceLogs.Remove(key);
            }

            return results;
        }

        /// <summary>
        /// Recovers the command log when needed.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="log">The result log.</param>
        /// <returns>Whether the command log is buffered.</returns>
        private bool EnsureBeforeExecutionLog<TResult>(ResultTraceLog<TResult> log)
        {
            if (!_beforeTraceLogs.ContainsKey(log.SessionId) && log.BeforeExecutionLog != null)
            {
                AddCancellableTraceLog(log.BeforeExecutionLog);
            }

            return _beforeTraceLogs.ContainsKey(log.SessionId);
        }

        /// <summary>
        /// Copies common event fields.
        /// </summary>
        /// <param name="item">The event to populate.</param>
        /// <param name="log">The command log.</param>
        private void PopulateCommonFields(TelemetryItem item,
            CancellableTraceLog log)
        {
            item.Application = Option.Application;
            item.Group = Option.Group;
            item.SessionId = log.SessionId;
            item.Operation = log.Key;
            item.StartTime = log.StartTime;
            item.Statement = log.Statement;
            item.Client = log.MachineName ?? _client;
            item.IpAddress = log.IpAddress;
            item.Source = _assembly?.GetName().Name;
            item.Version = _assembly?.GetName().Version?.ToString();
        }

        /// <summary>
        /// Publishes events during a timer tick.
        /// </summary>
        /// <param name="state">The timer state.</param>
        private void Callback(object state)
        {
            if (Interlocked.CompareExchange(ref _isPublishing,
                1,
                0) != 0)
            {
                return;
            }

            try
            {
                PublishReadyOperations();
            }
            finally
            {
                Interlocked.Exchange(ref _isPublishing,
                    0);
            }
        }

        /// <summary>
        /// Resolves the configured publisher.
        /// </summary>
        /// <returns>The configured publisher.</returns>
        private IPublisherRepository EnsurePublisherRepository()
        {
            if (_publisherRepository == null)
            {
                _publisherRepository = GetPublisherRepository();
            }

            return _publisherRepository;
        }

        /// <summary>
        /// Extracts completed and cancelled operations.
        /// </summary>
        /// <returns>The buffered publishing state.</returns>
        private Tuple<IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>, IDictionary<Guid, Tuple<DateTime, TimeSpan, object>>> Switch()
        {
            lock (_syncLock)
            {
                var logs = GetReadyLogs();
                var results = RemoveReadyResults(logs.Keys);
                return Tuple.Create(logs,
                    results);
            }
        }

        /// <summary>
        /// Add Cancellable Trace Log.
        /// </summary>

        /// <param name="log">The log.</param>
        private void AddCancellableTraceLog(CancellableTraceLog log)
        {
            lock (_syncLock)
            {
                if (!_beforeTraceLogs.ContainsKey(log.SessionId))
                {
                    var item = CreateTelemetryItem(log);
                    PopulateCommonFields(item,
                        log);
                    _beforeTraceLogs[log.SessionId] = Tuple.Create(log,
                        item);
                }
            }
        }

        /// <summary>
        /// Add Result Trace Log.
        /// </summary>

        /// <param name="log">The log.</param>
        private void AddResultTraceLog<TResult>(ResultTraceLog<TResult> log)
        {
            lock (_syncLock)
            {
                if (_afterTraceLogs.ContainsKey(log.SessionId) || !EnsureBeforeExecutionLog(log))
                {
                    return;
                }

                _afterTraceLogs[log.SessionId] = Tuple.Create<DateTime, TimeSpan, object>(DateTime.UtcNow,
                    log.ExecutionTime,
                    log.Result);
                CompleteTelemetryItem(_beforeTraceLogs[log.SessionId].Item2,
                    log.Result,
                    log.Error,
                    log.Level);
            }
        }

        /// <summary>
        /// Resolves the host/machine name the application is running on. Falls back through several
        /// APIs so it works reliably on Windows, Linux, and macOS (including containers where one of
        /// the APIs below may be unavailable or restricted).
        /// </summary>
        /// <returns>The best-effort machine/host name, or "Unknown" if none could be resolved.</returns>
        private static string GetClientMachineName()
        {
            try
            {
                // Works cross-platform on .NET Core/.NET 5+ (backed by gethostname() on Linux/macOS).
                return Environment.MachineName;
            }
            catch
            {
                try
                {
                    // Cross-platform fallback (e.g. sandboxed/containerized environments that restrict
                    // Environment.MachineName).
                    return Dns.GetHostName();
                }
                catch
                {
                    // Last resort: common environment variables set by the OS (HOSTNAME on Linux/macOS,
                    // COMPUTERNAME on Windows).
                    return Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.GetEnvironmentVariable("COMPUTERNAME") ?? "Unknown";
                }
            }
        }

        #endregion
    }
}
