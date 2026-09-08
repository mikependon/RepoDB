#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace RepoDb.Benchmarks.PostgreSql.Linq2db
{
    public class GetAllLinq2dbBenchmarks : Linq2dbBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Params(ElementsCount)]
        public int Rows { get; set; }

        [Benchmark]
        public void SelectAll()
        {
            using var db = GetDb();

            var persons = from p in db.People select p;

            persons.Consume(consumer);
        }
    }
}