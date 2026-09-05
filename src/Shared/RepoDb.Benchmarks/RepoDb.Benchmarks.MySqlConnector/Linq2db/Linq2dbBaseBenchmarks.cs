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
using LinqToDB.DataProvider.MySql;
using RepoDb.Benchmarks.MySqlConnector.Setup;

namespace RepoDb.Benchmarks.MySqlConnector.Linq2db
{
    [Description("Linq2db")]
    public class Linq2dbBaseBenchmarks : MySqlConnectorBenchmark
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
            options = options.UseMySql(DatabaseHelper.ConnectionString, MySqlVersion.AutoDetect, MySqlProvider.MySqlConnector);

            return new RepoDbDB(options);
        }
    }
}
