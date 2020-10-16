using BenchmarkDotNet.Running;
using RepoDb.Benchmarks.Configurations;

namespace RepoDb.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(typeof(BenchmarkConfig).Assembly);
            //switcher.RunAll(new BenchmarkConfig());

            //For single run.
            switcher.Run(args, new BenchmarkConfig());
        }
    }
}