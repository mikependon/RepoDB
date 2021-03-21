using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper;
using Dapper.Contrib.Extensions;
using RepoDb.Benchmarks.PostgreSql.Models;

namespace RepoDb.Benchmarks.PostgreSql.Dapper
{
    public class GetAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Benchmark]
        public void GetAll()
        {
            using var connection = GetConnection();
            connection.Open();

            connection.GetAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void QueryAll()
        {
            using var connection = GetConnection();

            connection.Query<Person>(@"select * from ""Person""", buffered: true).Consume(consumer);
        }
    }
}