#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.Sqlite.Microsoft.RepoDb
{
    // No RepoDb.Sqlite.Microsoft.BulkOperations package exists - SQLite has no server-side bulk-copy
    // protocol to wrap, so unlike the other providers in this suite, only the plain InsertAll benchmark
    // is exercised here.
    public class InsertAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        [Benchmark]
        public void InsertAll()
        {
            using var connection = GetConnection().EnsureOpen();

            var persons = GetPersons(Rows);
            connection.InsertAll(persons);
        }

        private static IEnumerable<Person> GetPersons(int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Person
                {
                    Name = $"Person-{i}",
                    Age = i + 1,
                    CreatedDateUtc= DateTime.UtcNow
                };
            }
        }
    }
}
