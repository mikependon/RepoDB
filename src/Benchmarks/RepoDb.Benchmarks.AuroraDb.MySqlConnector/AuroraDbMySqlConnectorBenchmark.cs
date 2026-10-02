#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Connector.AuroraDb.MySqlConnector;
using System.Data;
using BenchmarkDotNet.Attributes;
using MySqlConnector;
using RepoDb.Benchmarks.Core;
using RepoDb.Benchmarks.AuroraDb.MySqlConnector.Setup;

namespace RepoDb.Benchmarks.AuroraDb.MySqlConnector
{
    public abstract class AuroraDbMySqlConnectorBenchmark : BaseBenchmark
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

        protected override IDbConnection GetConnection() => new AuroraDbConnection(DatabaseHelper.ConnectionString);
    }
}
