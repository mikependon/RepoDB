using System.Data;
using System.Data.SqlClient;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Dapper.Contrib.Extensions;
using Dapper;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    public class GetAllDapperBenchmarks : DapperBaseBenchmarks
    {
        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public void GetAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            connection.GetAll<Person>().Consume(consumer);
        }

        [Benchmark]
        public void QueryAll()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            connection.Query<Person>("select * from Person", buffered: true).Consume(consumer);
        }
    }
}