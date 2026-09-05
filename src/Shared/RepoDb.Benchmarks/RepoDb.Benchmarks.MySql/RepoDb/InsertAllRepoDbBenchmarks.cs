#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MySql.Data.MySqlClient;
using RepoDb.Benchmarks.Core.Models;

namespace RepoDb.Benchmarks.MySql.RepoDb
{
    public class InsertAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        [Benchmark]
        public void BulkInsertAll()
        {
            using var connection = GetConnection().EnsureOpen() as MySqlConnection;

            var persons = GetPersons(Rows);
            connection.BulkInsert(persons);
        }

        [Benchmark]
        public void InsertAll()
        {
            using var connection = GetConnection().EnsureOpen();

            var persons = GetPersons(Rows);
            connection.InsertAll(persons);
        }

        [Benchmark]
        public async Task BulkInsertAllAsync()
        {
            await using var connection = await GetConnection().EnsureOpenAsync() as MySqlConnection;

            var persons = GetPersons(Rows);
            await connection.BulkInsertAsync(persons);
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
