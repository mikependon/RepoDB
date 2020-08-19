using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    [Description("EFCore 3.1.7")]
    public class EFCoreBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup() => BaseSetup();

        [Benchmark]
        public async Task<Person> FirstAsync()
        {
            await using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return await context.Persons.FirstAsync(x => x.Id == CurrentId);
        }

        [Benchmark]
        public Person First()
        {
            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.First(x => x.Id == CurrentId);
        }
    }
}