using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public abstract class BaseBenchmark
    {
        protected int CurrentId;

        protected void BaseSetup()
        {
            DatabaseHelper.Initialize(5000);
        }

        protected void IncreaseId()
        {
            CurrentId++;

            if (CurrentId > 5000)
            {
                CurrentId = 1;
            }
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            DatabaseHelper.Cleanup();
        }
    }
}