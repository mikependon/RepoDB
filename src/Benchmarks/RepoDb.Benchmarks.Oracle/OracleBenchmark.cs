#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using BenchmarkDotNet.Attributes;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Benchmarks.Core;
using RepoDb.Benchmarks.Oracle.Setup;

namespace RepoDb.Benchmarks.Oracle
{
    public abstract class OracleBenchmark : BaseBenchmark
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

        protected override IDbConnection GetConnection() => new OracleConnection(DatabaseHelper.ConnectionString);
    }
}
