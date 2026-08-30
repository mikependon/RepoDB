#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Models
{
    public class MdsNonIdentityCompleteTable
    {
        public Guid Id { get; set; }
        public Int64? ColumnBigInt { get; set; }
        public Byte[] ColumnBlob { get; set; }
        public String ColumnBoolean { get; set; }
        public String ColumnChar { get; set; }
        public String ColumnDate { get; set; }
        public String ColumnDateTime { get; set; }
        public Decimal? ColumnDecimal { get; set; }
        public Double? ColumnDouble { get; set; }
        public Int64? ColumnInteger { get; set; }
        public Int64? ColumnInt { get; set; }
        public String ColumnNone { get; set; }
        public Int64? ColumnNumeric { get; set; }
        public Double? ColumnReal { get; set; }
        public String ColumnString { get; set; }
        public String ColumnText { get; set; }
        public String ColumnTime { get; set; }
        public String ColumnVarChar { get; set; }
    }
}
