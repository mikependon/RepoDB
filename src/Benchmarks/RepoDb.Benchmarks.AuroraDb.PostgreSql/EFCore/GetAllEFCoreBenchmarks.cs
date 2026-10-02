#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.AuroraDb.PostgreSql.EFCore.Models;
using RepoDb.Benchmarks.AuroraDb.PostgreSql.Setup;

namespace RepoDb.Benchmarks.AuroraDb.PostgreSql.EFCore
{
    public class GetAllEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        private readonly Consumer consumer = new ();

        [Params(ElementsCount)]
        public int Rows { get; set; }

        [Benchmark]
        public void NoTrackingGetAll()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.AsNoTracking().Consume(consumer);
        }

        [Benchmark]
        public void FromSqlRawGetAll()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.FromSqlRaw(@"select * from ""Person""").Consume(consumer);
        }
    }
}
