#region Copyright Attributions

// Copyright (c) 2021 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.PostgreSql.EFCore.Models;
using RepoDb.Benchmarks.PostgreSql.Models;
using RepoDb.Benchmarks.PostgreSql.Setup;

namespace RepoDb.Benchmarks.PostgreSql.EFCore
{
    public class InsertAllEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        [Params(10, 100, 1000)]
        public int Rows { get; set; }

        [Benchmark]
        public void InsertAll()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.AddRange(GetPersons(Rows));
            context.SaveChanges();
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
