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
using LinqToDB.DataProvider.DB2;
using RepoDb.Benchmarks.Db2.Setup;

namespace RepoDb.Benchmarks.Db2.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : Db2Benchmark
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
            options = options.UseDB2(DatabaseHelper.ConnectionString, DB2Version.LUW);

            return new RepoDbDB(options);
        }
    }
}
