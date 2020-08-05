using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public class RepoDbBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();

            SqlServerBootstrap.Initialize();
            TypeMapper.Add(typeof(DateTime), DbType.DateTime2, true);
        }

        [Benchmark(Description = "RepoDb: FirstAsync")]
        public async Task<Person> FirstAsync()
        {
            IncreaseId();

            using IDbConnection connection = await new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpenAsync();
            var person = await connection.QueryAsync<Person>(x => x.Id == CurrentId);

            return person.First();
        }

        [Benchmark(Description = "RepoDb: First")]
        public Person First()
        {
            IncreaseId();

            using IDbConnection connection = new SqlConnection(DatabaseHelper.ConnectionString).EnsureOpen();

            return connection.Query<Person>(x => x.Id == CurrentId).First();
        }
    }
}