#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Db2.EFCore.Models;
using RepoDb.Benchmarks.Db2.Setup;

namespace RepoDb.Benchmarks.Db2.EFCore
{
    [Description("EFCore")]
    public class EFCoreBaseBenchmarks : Db2Benchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.FirstOrDefault();
            context.Persons.AsNoTracking().FirstOrDefault();
            context.Persons.FromSqlRaw("select * from PERSON").FirstOrDefault();
        }
    }
}
