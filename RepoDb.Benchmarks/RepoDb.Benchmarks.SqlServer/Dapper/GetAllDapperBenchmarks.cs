using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    public class GetAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Benchmark]
        public void GetAll()
        {
            using var connection = GetConnection();

            connection.GetAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void QueryAll()
        {
            using var connection = GetConnection();

            connection.Query<Person>("select * from Person", buffered: true).Consume(consumer);
        }
    }
}