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
            PostgreSqlBootstrap.Initialize();
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