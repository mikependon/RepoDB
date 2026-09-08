#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.SqlServer.EFCore.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.EFCore
{
    [Description("EFCore")]
    public class EFCoreBaseBenchmarks : SqlServerBenchmark
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
