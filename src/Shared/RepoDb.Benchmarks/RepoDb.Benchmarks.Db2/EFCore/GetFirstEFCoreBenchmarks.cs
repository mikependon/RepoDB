#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Db2.EFCore.Models;
using RepoDb.Benchmarks.Db2.Models;
using RepoDb.Benchmarks.Db2.Setup;

namespace RepoDb.Benchmarks.Db2.EFCore
{
    public class GetFirstEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        [Params(1)]
        public int Rows { get; set; }

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

            return context.Persons.FromSqlRaw("select * from PERSON where ID = {0}", CurrentId).First();
        }
    }
}
