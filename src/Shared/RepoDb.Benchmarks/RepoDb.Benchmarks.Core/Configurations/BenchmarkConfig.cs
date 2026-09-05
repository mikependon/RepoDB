#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Order;

namespace RepoDb.Benchmarks.Core.Configurations
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig(string provider)
        {
            AddLogger(ConsoleLogger.Default);
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);

            AddColumn(new OrmColumn(provider));
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
