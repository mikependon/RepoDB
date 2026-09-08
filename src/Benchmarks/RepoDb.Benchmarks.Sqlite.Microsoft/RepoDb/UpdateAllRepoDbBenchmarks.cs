#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.Sqlite.Microsoft.RepoDb
{
    public class UpdateAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly List<Person> persons = new();

        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        protected override void Bootstrap()
        {
            using var connection = GetConnection().EnsureOpen();

            foreach (var person in connection.QueryAll<Person>().Take(Rows))
            {
                person.CreatedDateUtc = DateTime.UtcNow;
                persons.Add(person);
            }
        }

        [Benchmark]
        public void UpdateAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.UpdateAll(persons);
        }
    }
}
