#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SaveDefinitionAsyncTest : SaveDefinitionTestBase
    {
        [TestMethod]
        [DataRow("json")]
        [DataRow("xml")]
        [DataRow("yaml")]
        public async Task UsesAsyncCatalogTracing(string format)
        {
            var trace = new AsyncCatalogTrace();
            using var cancellation = new CancellationTokenSource();
            if (format == "json")
                await Connection.SaveDefinitionAsJsonAsync("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren, 23, "save-async", trace, cancellation.Token);
            else if (format == "xml")
                await Connection.SaveDefinitionAsXmlAsync("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren, 23, "save-async", trace, cancellation.Token);
            else
                await Connection.SaveDefinitionAsYamlAsync("Invoice", Filename, RelatedTableBehavior.ParentsAndChildren, 23, "save-async", trace, cancellation.Token);

            Assert.IsTrue(trace.Keys.Count > 0);
            Assert.IsTrue(trace.Keys.All(key => key == "save-async"));
            Assert.IsTrue(trace.Timeouts.All(timeout => timeout == 23));
            Assert.IsTrue(trace.Tokens.All(token => token == cancellation.Token));
            Assert.AreEqual(trace.Keys.Count, trace.AfterCount);
            Assert.IsNull(SchemaReadContext.Current);
        }

        private sealed class AsyncCatalogTrace : ITrace
        {
            internal List<string> Keys { get; } = new List<string>();
            internal List<int?> Timeouts { get; } = new List<int?>();
            internal List<CancellationToken> Tokens { get; } = new List<CancellationToken>();
            internal int AfterCount { get; private set; }

            public void BeforeExecution(CancellableTraceLog log) => throw new InvalidOperationException("A synchronous callback was used.");

            public void AfterExecution<TResult>(ResultTraceLog<TResult> log) => throw new InvalidOperationException("A synchronous callback was used.");

            public Task BeforeExecutionAsync(CancellableTraceLog log, CancellationToken cancellationToken = default)
            {
                Keys.Add(log.Key);
                Timeouts.Add(SchemaReadContext.Current?.CommandTimeout);
                Tokens.Add(cancellationToken);
                return Task.CompletedTask;
            }

            public Task AfterExecutionAsync<TResult>(ResultTraceLog<TResult> log, CancellationToken cancellationToken = default)
            {
                AfterCount++;
                Tokens.Add(cancellationToken);
                return Task.CompletedTask;
            }
        }
    }
}
