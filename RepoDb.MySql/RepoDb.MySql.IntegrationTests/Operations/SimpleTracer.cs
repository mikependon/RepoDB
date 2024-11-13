using System;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.MySql.IntegrationTests.Operations
{
    internal class SimpleTracer : ITrace
    {
        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
            //throw new System.NotImplementedException();
        }

        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void BeforeExecution(CancellableTraceLog log)
        {
            Console.WriteLine(log);
        }

        public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
        {
            Console.WriteLine(log);
            return Task.CompletedTask;
        }
    }
}
