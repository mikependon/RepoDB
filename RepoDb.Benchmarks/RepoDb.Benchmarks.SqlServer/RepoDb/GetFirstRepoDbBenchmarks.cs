using System.Linq;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.SqlServer.Models;

namespace RepoDb.Benchmarks.SqlServer.RepoDb
{
    public class GetFirstRepoDbBenchmarks : RepoDbBaseBenchmarks
    {
        [Benchmark]
        public Person QueryLinqFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            return connection.Query<Person>(x => x.Id == CurrentId).First();
        }

        [Benchmark]
        public Person QueryDynamicFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            return connection.Query<Person>(new {Id = CurrentId}).First();
        }

        [Benchmark]
        public Person QueryObjectsFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            QueryField[] where =
            {
                new (nameof(Person.Id), CurrentId)
            };

            return connection.Query<Person>(where).First();
        }

        [Benchmark]
        public Person ExecuteQueryFirst()
        {
            using var connection = GetConnection().EnsureOpen();

            var param = new
            {
                Id = CurrentId
            };

            return connection.ExecuteQuery<Person>("select * from Person where Id = @Id", param).First();
        }
    }
}