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
using LinqToDB.DataProvider.SQLite;
using RepoDb.Benchmarks.Sqlite.Microsoft.Setup;

namespace RepoDb.Benchmarks.Sqlite.Microsoft.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : SqliteBenchmark
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
            options = options.UseSQLite(DatabaseHelper.ConnectionString, SQLiteProvider.Microsoft);

            return new RepoDbDB(options);
        }
    }
}
