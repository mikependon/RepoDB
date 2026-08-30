#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    public class BulkOperationIdentityTable
    {
        public long Id { get; set; }
        public char? ColumnChar { get; set; }
        public long? ColumnBigInt { get; set; }
        public bool? ColumnBit { get; set; }
        public bool? ColumnBoolean { get; set; }
        public DateTime? ColumnDate { get; set; }
        public int? ColumnInteger { get; set; }
        public decimal ColumnMoney { get; set; }
        public decimal? ColumnNumeric { get; set; }
        public float? ColumnReal { get; set; }
        public int? ColumnSerial { get; set; }
        public short? ColumnSmallInt { get; set; }
        public short? ColumnSmallSerial { get; set; }
        public string ColumnText { get; set; }
        public DateTime? ColumnTimeWithTimeZone { get; set; }
        public DateTime? ColumnTimeWithoutTimeZone { get; set; }
        public DateTimeOffset? ColumnTimestampWithTimeZone { get; set; }
        public DateTime? ColumnTimestampWithoutTimeZone { get; set; }
    }
}
