#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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