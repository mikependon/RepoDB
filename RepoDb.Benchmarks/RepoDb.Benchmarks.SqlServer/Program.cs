using BenchmarkDotNet.Running;
using RepoDb.Benchmarks.SqlServer.Configurations;

namespace RepoDb.Benchmarks.SqlServer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(typeof(BenchmarkConfig).Assembly);
            //switcher.RunAll(new BenchmarkConfigWitRows());

            //For single run.
            switcher.Run(args, new BenchmarkConfigWitRows());
        }
    }
}