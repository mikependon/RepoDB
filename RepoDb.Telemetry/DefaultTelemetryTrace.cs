using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using RepoDb.Telemetry.Default.Models;

namespace RepoDb.Telemetry
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultTelemetryTrace : ITrace
    {
        #region Privates

        private static readonly object _instanceLock = new object();
        private static readonly object _dataLock = new object();
        private IDictionary<Guid, Tuple<CancellableTraceLog, DefaultTelemetryItem>> _beforeTraceLogs;
        private IDictionary<Guid, Tuple<DateTime, TimeSpan, object>> _afterTraceLogs;
        private string _applicationName;
        private readonly Timer _timer;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        private DefaultTelemetryTrace(
            string applicationName,
            TimeSpan frequency)
        {
            _applicationName = applicationName;
            _beforeTraceLogs = new Dictionary<Guid, Tuple<CancellableTraceLog, DefaultTelemetryItem>>();
            _afterTraceLogs = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
            _timer = new Timer(
                callback: Callback,
                state: null,
                dueTime: frequency,
                period: frequency);
        }

        #endregion

        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static void Create(
            string applicationName,
            TimeSpan frequency)
        {
            lock (_instanceLock)
            {
                if (Instance == null)
                {
                    new DefaultTelemetryTrace(applicationName, frequency);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DefaultTelemetryTrace Instance { get; private set; }

        #endregion

        #region Implementations

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public void BeforeExecution(
            CancellableTraceLog log)
        {
            AddCancellableTraceLog(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task BeforeExecutionAsync(
            CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            AddCancellableTraceLog(log);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="log"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AfterExecution<TResult>(
            ResultTraceLog<TResult> log)
        {
            AddResultTraceLog(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task AfterExecutionAsync<TResult>(
            ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default)
        {
            AddResultTraceLog(log);
            return Task.CompletedTask;
        }

        #endregion

        #region Methods

        private void Callback(object state)
        {
            (var beforeTraceLogs, var afterTraceLogs) = Switch();
            var items = new List<DefaultTelemetryItem>();
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
        }

        private Tuple<IDictionary<Guid, Tuple<CancellableTraceLog, DefaultTelemetryItem>>, IDictionary<Guid, Tuple<DateTime, TimeSpan, object>>> Switch()
        {
            lock (_dataLock)
            {
                var beforeTraceLogs = _beforeTraceLogs;
                var afterTraceLogs = _afterTraceLogs;
                _beforeTraceLogs = new Dictionary<Guid, Tuple<CancellableTraceLog, DefaultTelemetryItem>>();
                _afterTraceLogs = new Dictionary<Guid, Tuple<DateTime, TimeSpan, object>>();
                return Tuple.Create(beforeTraceLogs, afterTraceLogs);
            }
        }

        #endregion

        #region Helpers

        private void AddCancellableTraceLog(
            CancellableTraceLog log)
        {
            lock (_dataLock)
            {
                if (!_beforeTraceLogs.ContainsKey(log.SessionId))
                {
                    var item = new DefaultTelemetryItem
                    {
                        ApplicationName = _applicationName,
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
            lock (_dataLock)
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
