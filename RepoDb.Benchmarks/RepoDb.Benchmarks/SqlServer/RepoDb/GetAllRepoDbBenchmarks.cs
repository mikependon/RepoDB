using System.Data;
using System.Data.SqlClient;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class GetAllRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public void QueryAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            connection.QueryAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void ExecuteQueryAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            connection.ExecuteQuery<Person>("select * from Person").Consume(consumer);
        }
    }
}