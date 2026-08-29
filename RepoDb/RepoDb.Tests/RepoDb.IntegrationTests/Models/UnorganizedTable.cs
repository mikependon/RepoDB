// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[Unorganized Table]")]
    public class UnorganizedTable
    {
        public long Id { get; set; }
        [Map("Session Id")]
        public Guid SessionId { get; set; }
        [Map("[Column Int]")]
        public int? ColumnInt { get; set; }
        [Map("Column/NVarChar")]
        public string ColumnNVarChar { get; set; }
        [Map("Column.DateTime")]
        public DateTime? ColumnDateTime2 { get; set; }
    }
}
