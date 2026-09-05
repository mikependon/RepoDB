#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Db2.Models;

namespace RepoDb.Benchmarks.Db2.RepoDb
{
    public class GetFirstRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        [Params(1)]
        public int Rows { get; set; }

        [Benchmark]
        public Person QueryLinqFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            return connection.Query<Person>(x => x.Id == CurrentId).First();
        }

        [Benchmark]
        public Person QueryDynamicFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            return connection.Query<Person>(new {Id = CurrentId}).First();
        }

        [Benchmark]
        public Person QueryObjectsFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            QueryField[] where =
            {
                new (nameof(Person.Id), CurrentId)
            };

            return connection.Query<Person>(where).First();
        }

        [Benchmark]
        public Person ExecuteQueryFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            var param = new
            {
                Id = CurrentId
            };

            return connection.ExecuteQuery<Person>("select * from PERSON where ID = :Id", param).First();
        }
    }
}
