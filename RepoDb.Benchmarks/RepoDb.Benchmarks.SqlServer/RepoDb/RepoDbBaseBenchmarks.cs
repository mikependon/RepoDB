using System;
using System.ComponentModel;
using System.Data;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.SqlServer.Configurations;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    [Description(OrmNameConstants.RepoDB)]
    public class RepoDbBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            SqlServerBootstrap.Initialize();
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