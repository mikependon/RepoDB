using System;
using BenchmarkDotNet.Attributes;
using RepoDb.Benchmarks.SqlServer.Setup;

namespace RepoDb.Benchmarks.SqlServer
{
    public abstract class BaseBenchmark
    {
        private const int BeforeAndAfterStepsCount = 2;
        private const int ElementsCount = Configurations.Constant.DefaultIterationCount * Configurations.Constant.DefaultUnrollFactor + Configurations.Constant.DefaultWarmupCount + BeforeAndAfterStepsCount;

        protected int CurrentId;

        protected void BaseSetup() => DatabaseHelper.Initialize(ElementsCount);

        [GlobalCleanup]
        public void Cleanup() => DatabaseHelper.Cleanup();

        [IterationSetup]
        public void IterationSetup() => CurrentId++;
    }
}