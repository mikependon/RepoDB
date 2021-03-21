using System.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.Dapper
{
    public class GetFirstDapperBenchmarks : DapperBaseBenchmarks
    {
        [Benchmark]
        public Person QueryFirst()
        {
            using var connection = GetConnection();

            var param = new
            {
                Id = CurrentId
            };

            return connection.QueryFirst<Person>("select * from Person where Id = @Id", param);
        }

        [Benchmark]
        public Person QueryLinqFirst()
        {
            using var connection = GetConnection();

            var param = new
            {
                Id = CurrentId
            };

            return connection.Query<Person>("select * from Person where Id = @Id", param, buffered: true).First();
        }
    }
}