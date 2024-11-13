#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;
using SystemDiagnosticsTrace = System.Diagnostics.Trace;

namespace RepoDb.Trace
{
    /// <summary>
    /// Creates a tracer that writes output to <see cref="SystemDiagnosticsTrace"/>
    /// </summary>
    public sealed class DiagnosticsTracer : ITrace
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="log"></param>
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        public void BeforeExecution(CancellableTraceLog log)
        {
            SystemDiagnosticsTrace.WriteLine(log);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="log"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
        {
            SystemDiagnosticsTrace.WriteLine(log);

            return Task.CompletedTask;
        }
    }
}
