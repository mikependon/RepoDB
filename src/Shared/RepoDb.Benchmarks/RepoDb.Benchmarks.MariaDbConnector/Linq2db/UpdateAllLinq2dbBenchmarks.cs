#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using DataModels;
using LinqToDB;

namespace RepoDb.Benchmarks.MariaDbConnector.Linq2db
{
    public class UpdateAllLinq2dbBenchmarks : Linq2dbBaseBenchmarks
    {
        private readonly List<Person> persons = new();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        protected override void Bootstrap()
        {
            using var db = GetDb();

            foreach (var person in (from p in db.People select p).Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow + TimeSpan.FromDays(365);
                persons.Add(person);
            }
        }

        [Benchmark]
        public void UpdateAll()
        {
            using var db = GetDb();

            foreach (var person in persons)
            {
                db.Update(person);
            }
        }
    }
}
