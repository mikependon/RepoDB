#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.SqlServer.Configurations;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    [Description(OrmNameConstants.Dapper)]
    public class DapperBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            connection.QueryFirstOrDefault<Person>("select * from Person");
        }
    }
}