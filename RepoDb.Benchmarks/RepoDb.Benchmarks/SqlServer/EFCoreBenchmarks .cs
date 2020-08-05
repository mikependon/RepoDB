using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using RepoDb.Benchmarks.Models;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public class EFCoreBenchmarks : BaseBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
        }

        [Benchmark(Description = "EFCore 3.1.6: FirstAsync")]
        public async Task<Person> FirstAsync()
        {
            IncreaseId();

            await using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return await context.Persons.FirstAsync(x => x.Id == CurrentId);
        }

        [Benchmark(Description = "EFCore 3.1.6: First")]
        public Person First()
        {
            IncreaseId();

            using var context = new EFCoreContext(DatabaseHelper.ConnectionString);

            return context.Persons.First(x => x.Id == CurrentId);
        }
    }
}