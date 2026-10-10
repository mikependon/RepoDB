#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.Data.PostgreSql.IntegrationTests.Setup
{
    /// <summary>
    /// A trace that records the keys of the operations that were traced.
    /// </summary>
    public class RecordingTrace : ITrace
    {
        public List<string> Keys { get; } = new List<string>();

        public void BeforeExecution(CancellableTraceLog log) =>
            Keys.Add(log.Key);

        public Task BeforeExecutionAsync(CancellableTraceLog log,
            CancellationToken cancellationToken = default)
        {
            Keys.Add(log.Key);
            return Task.CompletedTask;
        }

        public void AfterExecution<TResult>(ResultTraceLog<TResult> log)
        {
        }

        public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log,
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
