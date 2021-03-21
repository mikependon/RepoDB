using System.Data;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.SqlClient;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public abstract class BaseBenchmark
    {
        private const int BeforeAndAfterStepsCount = 2;

        protected const int ElementsCount = Configurations.DefaultsConstants.DefaultIterationCount * Configurations.DefaultsConstants.DefaultUnrollFactor
                                            + Configurations.DefaultsConstants.DefaultWarmupCount
                                            + BeforeAndAfterStepsCount;

        protected int CurrentId;

        [GlobalCleanup]
        public void Cleanup() => DatabaseHelper.Cleanup();

        [IterationSetup]
        public void IterationSetup() => CurrentId++;
        
        protected void BaseSetup()
        {
            DatabaseHelper.Initialize(ElementsCount);
            Bootstrap();
        }

        protected abstract void Bootstrap();

        protected static IDbConnection GetConnection() => new SqlConnection(DatabaseHelper.ConnectionString);
    }
}