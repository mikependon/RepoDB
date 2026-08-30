#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.IntegrationTests.Models
{
    public class PropertyHandlerTable
    {
        public long Id { get; set; }
        public string ColumnNVarChar { get; set; }
        public int? ColumnInt { get; set; }
        public int ColumnIntNotNull { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public int ColumnDecimalNotNull { get; set; }
        public short? ColumnFloat { get; set; }
        public short ColumnFloatNotNull { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime ColumnDateTimeNotNull { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public DateTime ColumnDateTime2NotNull { get; set; }
    }
}
