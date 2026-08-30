#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Benchmarks.SqlServer.Configurations
{
    public class DefaultsConstants
    {
        /// <summary>
        /// How many times we should launch process with target benchmark.
        /// </summary>
        public const int DefaultLaunchCount = 1;

        /// <summary>
        /// How many warmup iterations should be performed.
        /// </summary>
        public const int DefaultWarmupCount = 2;

        /// <summary>
        /// How many times the benchmark method will be invoked per one iteration of a generated loop.
        /// </summary>
        public const int DefaultUnrollFactor = 500;

        /// <summary>
        /// How many target iterations should be performed.
        /// </summary>
        public const int DefaultIterationCount = 10;
    }
}