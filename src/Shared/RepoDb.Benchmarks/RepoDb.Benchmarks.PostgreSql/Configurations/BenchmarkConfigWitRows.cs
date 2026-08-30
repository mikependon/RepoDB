#region Copyright Attributions

// Copyright (c) 2020 SergerGood and Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using BenchmarkDotNet.Columns;

namespace RepoDb.Benchmarks.PostgreSql.Configurations
{
    public class BenchmarkConfigWitRows : BenchmarkConfig
    {
        public BenchmarkConfigWitRows()
        {
            AddColumn(new ParamColumn("Rows"));
        }
    }
}