using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.PostgreSql.Configurations;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.Dapper
{
    [Description(OrmNameConstants.Dapper)]
    public class DapperBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            connection.QueryFirstOrDefault<Person>(@"select * from ""Person""");
        }
    }
}