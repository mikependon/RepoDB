using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace RepoDb.Benchmarks.PostgreSql.Linq2db
{
    public class GetAllLinq2dbBenchmarks : Linq2dbBaseBenchmarks
    {
        private readonly Consumer consumer = new();

        [Benchmark]
        public void SelectAll()
        {
            using var db = GetDb();

            var persons = from p in db.People select p;

            persons.Consume(consumer);
        }
    }
}