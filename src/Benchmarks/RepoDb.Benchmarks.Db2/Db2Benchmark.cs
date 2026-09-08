#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using BenchmarkDotNet.Attributes;
using IBM.Data.Db2;
using RepoDb.Benchmarks.Core;
using RepoDb.Benchmarks.Db2.Setup;

namespace RepoDb.Benchmarks.Db2
{
    public abstract class Db2Benchmark : BaseBenchmark
    {
        [GlobalCleanup]
        public override void Cleanup() => DatabaseHelper.Cleanup();

        [IterationSetup]
        public override void IterationSetup() => CurrentId++;

        protected override void BaseSetup()
        {
            DatabaseHelper.Initialize(ElementsCount);
            Bootstrap();
        }

        protected override IDbConnection GetConnection() => new DB2Connection(DatabaseHelper.ConnectionString);
    }
}
