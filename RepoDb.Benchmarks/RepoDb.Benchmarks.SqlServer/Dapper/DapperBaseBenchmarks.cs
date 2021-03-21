using System.ComponentModel;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.SqlServer.Configurations;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    [Description(OrmNameConstants.Dapper)]
    public class DapperBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var connection = GetConnection();

            connection.QueryFirstOrDefault<Person>("select * from Person");
        }
    }
}