using System.ComponentModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.SqlServer.Configurations;
using RepoDb.Benchmarks.SqlServer.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer.EFCore
{
    [Description(OrmNameConstants.EFCore)]
    public class EFCoreBaseBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        protected override void Bootstrap()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            context.Persons.FirstOrDefault();
            context.Persons.AsNoTracking().FirstOrDefault();
            context.Persons.FromSqlRaw("select * from Person").FirstOrDefault();
        }
    }
}