#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Attributes;
using DataModels;

namespace RepoDb.Benchmarks.Db2.Linq2db
{
    public class GetFirstLinq2dbBenchmarks : Linq2dbBaseBenchmarks
    {
        [Params(1)]
        public int Rows { get; set; }

        [Benchmark]
        public Person Find()
        {
            using var db = GetDb();

            return db.People.Find(CurrentId);
        }
    }
}
