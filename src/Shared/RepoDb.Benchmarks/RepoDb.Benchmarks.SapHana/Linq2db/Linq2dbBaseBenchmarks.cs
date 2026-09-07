#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using DataModels;
using LinqToDB;
using LinqToDB.DataProvider.SapHana;
using RepoDb.Benchmarks.SapHana.Setup;

namespace RepoDb.Benchmarks.SapHana.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : SapHanaBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var db = GetDb();

            db.People.Select(x => x.Id == CurrentId).ToList();
        }

        // linq2db has no fluent DataOptions.UseSapHana() helper (unlike MySql/ClickHouse) - the
        // provider is resolved through SapHanaTools instead. SapHanaProvider.Unmanaged is the native
        // Sap.Data.Hana(.Core) client, the same one RepoDb.SapHana itself is built on.
        protected static RepoDbDB GetDb()
        {
            var dataProvider = SapHanaTools.GetDataProvider(SapHanaProvider.Unmanaged, DatabaseHelper.ConnectionString);
            var options = new DataOptions().UseConnectionString(dataProvider, DatabaseHelper.ConnectionString);

            return new RepoDbDB(options);
        }
    }
}
