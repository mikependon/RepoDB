using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    [Description("Dapper 2.0.35")]
    public class DapperBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        public override void Bootstrap()
        {
            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            connection.QueryFirstOrDefault<Person>("select * from Person");
        }
    }
}