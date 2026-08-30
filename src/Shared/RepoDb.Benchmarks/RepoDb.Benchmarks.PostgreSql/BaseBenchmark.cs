#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;
using BenchmarkDotNet.Attributes;
using Npgsql;
using RepoDb.Benchmarks.PostgreSql.Configurations;
using RepoDb.Benchmarks.PostgreSql.Setup;

namespace RepoDb.Benchmarks.PostgreSql
{
    public abstract class BaseBenchmark
    {
        private const int BeforeAndAfterStepsCount = 2;

        protected const int ElementsCount = DefaultConstants.DefaultIterationCount * DefaultConstants.DefaultUnrollFactor
                                            + DefaultConstants.DefaultWarmupCount + BeforeAndAfterStepsCount;

        protected int CurrentId;

        [GlobalCleanup]
        public void Cleanup() => DatabaseHelper.Cleanup();

        [IterationSetup]
        public void IterationSetup() => CurrentId++;

        protected void BaseSetup()
        {
            DatabaseHelper.Initialize(ElementsCount);
            Bootstrap();
        }

        protected abstract void Bootstrap();

        protected static IDbConnection GetConnection() => new NpgsqlConnection(DatabaseHelper.ConnectionString);
    }
}