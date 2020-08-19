using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace RepoDb.Benchmarks.Configurations
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
            AddColumn(StatisticColumn.Max);
            AddColumn(StatisticColumn.Min);

            AddColumnProvider(DefaultColumnProviders.Metrics);

            var job = Job.ShortRun
                .WithLaunchCount(Constant.DefaultLaunchCount)
                .WithWarmupCount(Constant.DefaultWarmupCount)
                .WithUnrollFactor(Constant.DefaultUnrollFactor)
                .WithIterationCount(Constant.DefaultIterationCount);

            AddJob(job);

            Orderer = new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest);
            Options |= ConfigOptions.JoinSummary | ConfigOptions.StopOnFirstError;
        }
    }
}