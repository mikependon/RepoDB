using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace RepoDb.Benchmarks.SqlServer.Configurations
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddLogger(ConsoleLogger.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);

            AddColumn(new ORMColum());
            AddColumn(TargetMethodColumn.Method);
            AddColumn(StatisticColumn.Mean);
            AddColumn(StatisticColumn.StdDev);
            AddColumn(StatisticColumn.Error);
            AddColumn(BaselineRatioColumn.RatioMean);
            AddColumn(StatisticColumn.Min);
            AddColumn(StatisticColumn.Max);

            AddColumnProvider(DefaultColumnProviders.Metrics);

            var job = Job.ShortRun
                .WithLaunchCount(DefaultsConstants.DefaultLaunchCount)
                .WithWarmupCount(DefaultsConstants.DefaultWarmupCount)
                .WithUnrollFactor(DefaultsConstants.DefaultUnrollFactor)
                .WithIterationCount(DefaultsConstants.DefaultIterationCount);

            AddJob(job);

            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
            Options |= ConfigOptions.JoinSummary | ConfigOptions.StopOnFirstError;
        }
    }
}