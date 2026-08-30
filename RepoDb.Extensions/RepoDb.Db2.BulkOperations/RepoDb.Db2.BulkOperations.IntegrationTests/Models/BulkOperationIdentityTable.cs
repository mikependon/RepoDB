#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.Db2.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// Db2 has no native BOOLEAN column usable for parameter binding, so the SqlServer suite's BIT
    /// column is represented here as a SMALLINT-backed <see cref="byte"/>? instead of bool? -
    /// round-tripped via <c>Db2ByteToInt16PropertyHandler</c>. RowGuid is stored as
    /// <c>CHAR(16) FOR BIT DATA</c> and round-tripped via <c>Db2GuidToByteArrayPropertyHandler</c>.
    /// Both handlers are registered per-property in Setup/Database.cs, the same pattern used by
    /// RepoDb.Db2.IntegrationTests.
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
