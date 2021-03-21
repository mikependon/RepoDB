using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class GetAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Benchmark]
        public void QueryAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.QueryAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void ExecuteQueryAll()
        {
            using var connection = GetConnection().EnsureOpen();

            connection.ExecuteQuery<Person>("select * from Person").Consume(consumer);
        }
    }
}