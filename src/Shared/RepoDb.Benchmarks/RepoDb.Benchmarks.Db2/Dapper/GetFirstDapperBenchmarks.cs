#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.Db2.Models;

namespace RepoDb.Benchmarks.Db2.Dapper
{
    public class GetFirstDapperBenchmarks : DapperBaseBenchmarks
    {
        [Params(1)]
        public int Rows { get; set; }

        [Benchmark]
        public Person QueryFirst()
        {
            using var connection = GetConnection();

            var param = new
            {
                Id = CurrentId
            };

            return connection.QueryFirst<Person>("select * from PERSON where ID = :Id", param);
        }

        [Benchmark]
        public Person QueryLinqFirst()
        {
            using var connection = GetConnection();

            var param = new
            {
                Id = CurrentId
            };

            return connection.Query<Person>("select * from PERSON where ID = :Id", param, buffered: true).First();
        }
    }
}
