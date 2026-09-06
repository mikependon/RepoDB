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
using LinqToDB.DataProvider.ClickHouse;
using RepoDb.Benchmarks.ClickHouse.Setup;

namespace RepoDb.Benchmarks.ClickHouse.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : ClickHouseBenchmark
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
            options = options.UseClickHouse(DatabaseHelper.ConnectionString, ClickHouseProvider.ClickHouseDriver);

            return new RepoDbDB(options);
        }
    }
}
