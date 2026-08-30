#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;

namespace RepoDb.MySqlConnector.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// MySqlConnector has no native BOOLEAN column usable for parameter binding (MySqlConnectorDbType.Boolean is
    /// documented as "Not Available in ODP.NET, Managed Driver"), so the SqlServer suite's BIT column
    /// is represented here as a NUMBER(1,0)-backed <see cref="byte"/>? instead of bool?. RowGuid is
    /// stored as RAW(16) and round-tripped via <c>MySqlConnectorGuidToByteArrayPropertyHandler</c> (registered
    /// per-property in Setup/Database.cs), the same pattern used by RepoDb.MySqlConnector.IntegrationTests.
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
