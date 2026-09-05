#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.Db2.Models;

namespace RepoDb.Benchmarks.Db2.Dapper
{
    public class UpdateAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly List<Person> persons = new ();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            foreach (var person in connection.GetAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void ExecuteUpdateAll()
        {
            using var connection = GetConnection();

            connection.Execute(@"update PERSON
                                set CREATEDDATEUTC = :CreatedDateUtc, NAME = :Name, AGE = :Age
                                where ID = :Id", persons);
        }
    }
}
