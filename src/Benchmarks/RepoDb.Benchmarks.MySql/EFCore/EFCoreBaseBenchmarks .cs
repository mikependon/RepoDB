#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.MySql.EFCore.Models;
using RepoDb.Benchmarks.MySql.Setup;

namespace RepoDb.Benchmarks.MySql.EFCore
{
    [Description("EFCore")]
    public class EFCoreBaseBenchmarks : MySqlBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.FirstOrDefault();
            context.Persons.AsNoTracking().FirstOrDefault();
            context.Persons.FromSqlRaw("select * from Person").FirstOrDefault();
        }
    }
}
