#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class GetAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Params(ElementsCount)]
        public int Rows { get; set; }

        [Benchmark]
        public void QueryAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.QueryAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void ExecuteQueryAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.ExecuteQuery<Person>("select * from Person").Consume(consumer);
        }
    }
}