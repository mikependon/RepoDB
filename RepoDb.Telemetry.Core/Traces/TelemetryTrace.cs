using System;
using System.Collections.Generic;
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
    public class TelemetryTrace : ITrace
    {
        #region Privates

        private static readonly object _syncLock = new object();
        private static bool _isPublishing = false;
        private IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>> _beforeTraceLogs;
        private IDictionary<Guid, Tuple<DateTime, TimeSpan, object>> _afterTraceLogs;
        private readonly TelemetryOption _option;
        private readonly Timer _timer;
        private readonly IPublisherRepository _publisherRepository;

        // The assembly that hosts/consumes the library (i.e. the client application), falling back to the
        // immediate caller if an entry assembly cannot be resolved (e.g. some test hosts or plugin scenarios).
        private static readonly Assembly _callingAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();

        // The machine/host name on which the application is running. Resolved once and cached, since it
        // does not change during the lifetime of the process.
        private static readonly string _clientMachineName = GetClientMachineName();

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <param name="errorCallback"></param>
        /// <param name="logger"></param>
        public TelemetryTrace(
            TelemetryOption option,
            Action<Exception> errorCallback = null,
            ILogger logger = null)
        {
            _option = option;
            _beforeTraceLogs = new Dictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>();
            _afterTraceLogs = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
            _timer = new Timer(callback: Callback);
            _publisherRepository = new TelemetryPublisherRepository(option.Host, option.ApiKey, errorCallback);
        }

        #endregion

        #region Implementations

        /// <summary>
        /// Starts the telemetry trace to capture the telemetry data and send it to the insights solution.
        /// </summary>
        public void Start()
        {
            _timer.Change(_option.Frequency, _option.Frequency);
        }

        /// <summary>
        /// The method that is called before the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="CancellableTraceLog"/> class to log.</param>
        public void BeforeExecution(
            CancellableTraceLog log)
        {
            AddCancellableTraceLog(log);
        }

        /// <summary>
        /// The method that is called before the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="CancellableTraceLog"/> class to log.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public Task BeforeExecutionAsync(
            CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            AddCancellableTraceLog(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// The method that is called after the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="ResultTraceLog{TResult}"/> class to log.</param>
        public void AfterExecution<TResult>(
            ResultTraceLog<TResult> log)
        {
            AddResultTraceLog(log);
        }

        /// <summary>
        /// The method that is called after the execution of the command.
        /// </summary>
        /// <param name="log">The instance of <see cref="ResultTraceLog{TResult}"/> class to log.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
        public Task AfterExecutionAsync<TResult>(
            ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AddResultTraceLog(log);
            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void Callback(
            object state)
        {
            if (_isPublishing)
            {
                return;
            }
            _isPublishing = true;
            try
            {
                (var beforeTraceLogs, var afterTraceLogs) = Switch();
                var items = new List<TelemetryItem>();
                foreach (var kvp in beforeTraceLogs)
                {
                    (var log, var item) = kvp.Value;
                    item.IsCancelled = log.IsCancelled;
                    item.Elapsed = afterTraceLogs.ContainsKey(kvp.Key) ?
                        afterTraceLogs[kvp.Key].Item2.TotalSeconds :
                            (DateTime.UtcNow - log.StartTime).TotalSeconds;
                    items.Add(item);
                }
                if (items.Count > 0)
                {
                    _publisherRepository.PublishMany(items);
                }
            }
            finally
            {
                _isPublishing = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Tuple<IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>, IDictionary<Guid, Tuple<DateTime, TimeSpan, object>>> Switch()
        {
            lock (_syncLock)
            {
                var beforeTraceLogs = _beforeTraceLogs;
                var afterTraceLogs = _afterTraceLogs;
                _beforeTraceLogs = new Dictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>();
                _afterTraceLogs = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
                return Tuple.Create(beforeTraceLogs, afterTraceLogs);
            }
        }

        #endregion

        #region Helpers

        private void AddCancellableTraceLog(
            CancellableTraceLog log)
        {
            lock (_syncLock)
            {
                if (!_beforeTraceLogs.ContainsKey(log.SessionId))
                {
                    var item = new TelemetryItem
                    {
                        Application = _option.Application,
                        Group = _option.Group,
                        SessionId = log.SessionId,
                        Operation = log.Key,
                        StartTime = log.StartTime,
                        Statement = log.Statement,
                        Client = _clientMachineName,
                        Assembly = _callingAssembly?.FullName,
                        Version = _callingAssembly?.GetName().Version?.ToString()
                    };
                    _beforeTraceLogs[log.SessionId] = Tuple.Create(log, item);
                }
            }
        }

        private void AddResultTraceLog<TResult>(
            ResultTraceLog<TResult> log)
        {
            lock (_syncLock)
            {
                if (!_afterTraceLogs.ContainsKey(log.SessionId))
                {
                    var value = Tuple.Create<DateTime, TimeSpan, object>(DateTime.UtcNow, log.ExecutionTime, log.Result);
                    _afterTraceLogs[log.SessionId] = value;
                }
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
                    return Environment.GetEnvironmentVariable("HOSTNAME")
                        ?? Environment.GetEnvironmentVariable("COMPUTERNAME")
                        ?? "Unknown";
                }
            }
        }

        #endregion
    }
}
