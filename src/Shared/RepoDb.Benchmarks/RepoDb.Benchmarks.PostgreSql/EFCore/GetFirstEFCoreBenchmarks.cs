#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.PostgreSql.EFCore.Models;
using RepoDb.Benchmarks.PostgreSql.Models;
using RepoDb.Benchmarks.PostgreSql.Setup;

namespace RepoDb.Benchmarks.PostgreSql.EFCore
{
    public class GetFirstEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        [Benchmark]
        public Person First()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person NoTrackingFirst()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.AsNoTracking().First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person FromSqlRawFirst()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.FromSqlRaw(@"select * from ""Person"" where ""Id"" = {0}", CurrentId).First();
        }
    }
}