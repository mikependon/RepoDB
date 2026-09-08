#region Copyright Attributions

// Copyright (c) 2026 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Data;

namespace RepoDb.Benchmarks.Core
{
    public abstract class BaseBenchmark
    {
        private const int BeforeAndAfterStepsCount = 2;

        protected const int ElementsCount =
            (
                Configurations.DefaultsConstants.DefaultIterationCount *
                Configurations.DefaultsConstants.DefaultUnrollFactor
            ) +
            Configurations.DefaultsConstants.DefaultWarmupCount +
            BeforeAndAfterStepsCount;

        protected int CurrentId;

        public abstract void Cleanup();

        public abstract void IterationSetup();

        protected abstract void BaseSetup();

        protected abstract void Bootstrap();

        protected abstract IDbConnection GetConnection();
    }
}
