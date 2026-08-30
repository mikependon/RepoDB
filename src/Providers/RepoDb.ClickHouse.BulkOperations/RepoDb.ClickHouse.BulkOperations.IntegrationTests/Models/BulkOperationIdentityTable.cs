#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.ClickHouse.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// ClickHouse has no dedicated boolean column type usable the way SQL Server's BIT is, so the
    /// SqlServer suite's BIT column is represented here as a <c>Nullable(UInt8)</c>-backed
    /// <see cref="byte"/>? instead of bool?. RowGuid is stored as ClickHouse's native <c>UUID</c> column
    /// type, which binds directly to/from <see cref="Guid"/> - no property handler required (see
    /// Setup/Database.cs).
    /// </summary>
    public class BulkOperationIdentityTable
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public byte? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
