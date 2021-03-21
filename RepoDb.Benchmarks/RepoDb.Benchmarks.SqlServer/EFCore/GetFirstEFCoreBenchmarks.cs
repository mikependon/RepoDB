using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.SqlServer.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.EFCore
{
    public class GetFirstEFCoreBenchmarks : EFCoreBaseBenchmarks
    {
        [Benchmark]
        public Person First()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person NoTrackingFirst()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.AsNoTracking().First(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person FromSqlRawFirst()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.FromSqlRaw("select * from Person where Id = {0}", CurrentId).First();
        }
    }
}