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

        // SapHanaProvider.Unmanaged is the native Sap.Data.Hana(.Core) client, the same one
        // RepoDb.SapHana itself is built on (as opposed to the ODBC alternative).
        protected static RepoDbDB GetDb()
        {
            var options = new DataOptions();
            options = options.UseSapHana(DatabaseHelper.ConnectionString, SapHanaProvider.Unmanaged);

            return new RepoDbDB(options);
        }
    }
}
