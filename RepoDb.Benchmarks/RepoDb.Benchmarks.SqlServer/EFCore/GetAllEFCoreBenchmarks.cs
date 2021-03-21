using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.SqlServer.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.EFCore
{
    public class GetAllEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        private readonly Consumer consumer = new ();

        [Benchmark]
        public void NoTrackingGetAll()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.AsNoTracking().Consume(consumer);
        }

        [Benchmark]
        public void FromSqlRawGetAll()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.FromSqlRaw("select * from Person").Consume(consumer);
        }
    }
}