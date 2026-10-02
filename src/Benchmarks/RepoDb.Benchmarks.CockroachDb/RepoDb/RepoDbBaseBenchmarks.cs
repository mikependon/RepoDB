#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.ComponentModel;
using System.Data;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.CockroachDb.Models;

namespace RepoDb.Benchmarks.CockroachDb.RepoDb
{
    [Description("RepoDB")]
    public class RepoDbBaseBenchmarks : CockroachDbBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            GlobalConfiguration.Setup().UseCockroachDb();
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
