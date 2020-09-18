using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    [Description("Dapper 2.0.35")]
    public class DapperBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        public override void Bootstrap()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            connection.QueryFirstOrDefault<Person>("select * from Person");
        }

        [Benchmark]
        public Person QueryFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var param = new
            {
                Id = CurrentId
            };

            return connection.QueryFirst<Person>("select * from Person where Id = @Id", param);
        }

        [Benchmark]
        public Person QueryLinqFirst()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            var param = new
            {
                Id = CurrentId
            };

            return connection.Query<Person>("select * from Person where Id = @Id", param, buffered: true).First();
        }
    }
}