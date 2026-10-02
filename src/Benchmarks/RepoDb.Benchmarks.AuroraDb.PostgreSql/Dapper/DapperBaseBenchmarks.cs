#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.AuroraDb.PostgreSql.Models;

namespace RepoDb.Benchmarks.AuroraDb.PostgreSql.Dapper
{
    [Description("Dapper")]
    public class DapperBaseBenchmarks : AuroraDbPostgreSqlBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            connection.QueryFirstOrDefault<Person>(@"select * from ""Person""");
        }
    }
}
