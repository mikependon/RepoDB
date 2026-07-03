using System;
using System.Collections.Generic;
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

        private static readonly object _lock = new object();
        private IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>> _beforeTraceLogs;
        private IDictionary<Guid, Tuple<DateTime, TimeSpan, object>> _afterTraceLogs;
        private readonly TelemetryOption _option;
        private readonly Timer _timer;
        private readonly IPublisherRepository _publisherRepository;

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
            _publisherRepository = new TelemetryPublisherRepository(option.Host, errorCallback);
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
            (var beforeTraceLogs, var afterTraceLogs) = Switch();
            var items = new List<TelemetryItem>();
            foreach (var kvp in beforeTraceLogs)
            {
                (var log, var item) = kvp.Value;
                if (log.IsCancelled)
                {
                    item.IsCancelled = true;
                }
                if (afterTraceLogs.ContainsKey(kvp.Key))
                {
                    var afterLog = afterTraceLogs[kvp.Key];
                    item.Elapsed = afterLog.Item2;
                }
                else
                {
                    item.Elapsed = DateTime.UtcNow - log.StartTime;
                }
                // TODO: How about the result?
                items.Add(item);
            }
            _publisherRepository.PublishMany(items);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Tuple<IDictionary<Guid, Tuple<CancellableTraceLog, TelemetryItem>>, IDictionary<Guid, Tuple<DateTime, TimeSpan, object>>> Switch()
        {
            lock (_lock)
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
            lock (_lock)
            {
                if (!_beforeTraceLogs.ContainsKey(log.SessionId))
                {
                    var item = new TelemetryItem
                    {
                        ApplicationName = _option.ApplicationName,
                        Group = _option.Group,
                        SessionId = log.SessionId,
                        Key = log.Key,
                        StartTime = log.StartTime,
                        Statement = log.Statement
                    };
                    _beforeTraceLogs[log.SessionId] = Tuple.Create(log, item);
                }
            }
        }

        private void AddResultTraceLog<TResult>(
            ResultTraceLog<TResult> log)
        {
            lock (_lock)
            {
                if (!_afterTraceLogs.ContainsKey(log.SessionId))
                {
                    var value = Tuple.Create<DateTime, TimeSpan, object>(DateTime.UtcNow, log.ExecutionTime, log.Result);
                    _afterTraceLogs[log.SessionId] = value;
                }
            }
        }

        #endregion
    }
}
