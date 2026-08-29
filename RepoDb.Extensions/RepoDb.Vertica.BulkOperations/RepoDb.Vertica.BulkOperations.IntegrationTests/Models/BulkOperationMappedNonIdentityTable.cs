#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using System;

namespace RepoDb.Vertica.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// 
    /// </summary>
    [Map("BulkOperationNonIdentityTable")]
    public class BulkOperationMappedNonIdentityTable
    {
        [Map("Id")]
        public long IdMapped { get; set; }
        [Map("RowGuid")]
        public Guid RowGuidMapped { get; set; }
        [Map("ColumnBit")]
        public bool? ColumnBitMapped { get; set; }
        [Map("ColumnDateTime")]
        public DateTime? ColumnDateTimeMapped { get; set; }
        [Map("ColumnDateTime2")]
        public DateTime? ColumnDateTime2Mapped { get; set; }
        [Map("ColumnDecimal")]
        public decimal? ColumnDecimalMapped { get; set; }
        [Map("ColumnFloat")]
        public double? ColumnFloatMapped { get; set; }
        [Map("ColumnInt")]
        public int? ColumnIntMapped { get; set; }
        [Map("ColumnNVarChar")]
        public string ColumnNVarCharMapped { get; set; }
    }
}
