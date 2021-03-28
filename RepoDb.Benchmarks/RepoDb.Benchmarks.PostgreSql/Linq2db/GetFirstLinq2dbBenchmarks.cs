using BenchmarkDotNet.Attributes;
using DataModels;

namespace RepoDb.Benchmarks.PostgreSql.Linq2db
{
    public class GetFirstLinq2dbBenchmarks : Linq2dbBaseBenchmarks
    {
        [Benchmark]
        public Person Find()
        {
            using var db = GetDb();

            return db.People.Find(CurrentId);
        }
    }
}