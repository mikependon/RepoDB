using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public class DapperBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
        }

        [Benchmark(Description = "Dapper 2.0.35: FirstAsync")]
        public async Task<Person> FirstAsync()
        {
            IncreaseId();

            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Person>("select * from Person where Id = @Id",
                new {Id = CurrentId});
        }

        [Benchmark(Description = "Dapper 2.0.35: First")]
        public Person First()
        {
            IncreaseId();

            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString);
            connection.Open();

            return connection.QueryFirstOrDefault<Person>("select * from Person where Id = @Id", new {Id = CurrentId});
        }
    }
}