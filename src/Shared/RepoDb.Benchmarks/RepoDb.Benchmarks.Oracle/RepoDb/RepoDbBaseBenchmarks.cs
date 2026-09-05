#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.ComponentModel;
using System.Data;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Oracle.Models;

namespace RepoDb.Benchmarks.Oracle.RepoDb
{
    [Description("RepoDB")]
    public class RepoDbBaseBenchmarks : OracleBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            GlobalConfiguration.Setup().UseOracle();
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
