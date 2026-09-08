#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.Dapper
{
    public class GetAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Params(ElementsCount)]
        public int Rows { get; set; }

        [Benchmark]
        public void GetAll()
        {
            using var connection = GetConnection();
            connection.Open();

            connection.GetAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void QueryAll()
        {
            using var connection = GetConnection();

            connection.Query<Person>(@"select * from ""Person""", buffered: true).Consume(consumer);
        }
    }
}