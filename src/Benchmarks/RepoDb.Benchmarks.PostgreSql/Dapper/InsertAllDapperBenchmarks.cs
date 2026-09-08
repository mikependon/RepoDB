#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.Dapper
{
    public class InsertAllDapperBenchmarks : DapperBaseBenchmarks
    {
        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        [Benchmark]
        public void InsertAll()
        {
            using var connection = GetConnection();

            foreach (var person in GetPersons(Rows))
            {
                connection.Insert(person);
            }
        }

        [Benchmark]
        public void ExecuteInsertAll()
        {
            using var connection = GetConnection();

            connection.Execute(@"insert into ""Person"" (""Name"", ""Age"", ""CreatedDateUtc"")
                                values (@Name, @Age, @CreatedDateUtc)", GetPersons(Rows));
        }

        private static IEnumerable<Person> GetPersons(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person
                {
                    Name = $"Person-{i}",
                    Age = i + 1,
                    CreatedDateUtc = DateTime.UtcNow
                };
            }
        }
    }
}
