#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    public class NonKeyedTable
    {
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
    }
}
