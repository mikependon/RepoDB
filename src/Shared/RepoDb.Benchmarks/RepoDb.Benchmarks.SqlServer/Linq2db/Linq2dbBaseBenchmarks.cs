#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using DataModels;
using LinqToDB;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : SqlServerBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var db = GetDb();

            db.People.Select(x => x.Id == CurrentId).ToList();
        }

        protected static RepoDbDB GetDb()
        {
            var options = new DataOptions();
            options = options.UseSqlServer(DatabaseHelper.ConnectionString);

            return new RepoDbDB(options);
        }
    }
}
