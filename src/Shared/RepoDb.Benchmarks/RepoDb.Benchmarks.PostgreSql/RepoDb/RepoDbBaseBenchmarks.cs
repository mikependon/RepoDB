#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.ComponentModel;
using System.Data;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.PostgreSql.Configurations;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.RepoDb
{
    [Description(OrmNameConstants.RepoDB)]
    public class RepoDbBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            GlobalConfiguration.Setup().UsePostgreSql();
            TypeMapper.Add(typeof(DateTime), DbType.DateTime2, true);
            BaseSetup();
        }

        protected override void Bootstrap()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.Query<Person>(x => x.Id == CurrentId);
            connection.QueryAll<Person>();
        }
    }
}
