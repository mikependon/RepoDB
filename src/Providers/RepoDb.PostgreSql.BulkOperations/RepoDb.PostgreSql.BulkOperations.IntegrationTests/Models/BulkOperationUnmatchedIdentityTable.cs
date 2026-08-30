#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using System;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    /*
     * Useful for NpgsqlBulkInsertMapItem mappings
     */

    [Map("[dbo].[BulkOperationIdentityTable]")]
    public class BulkOperationUnmatchedIdentityTable
    {
        [Identity]
        public long IdMapped { get; set; }

        public long? ColumnBigIntMapped { get; set; }

        public bool? ColumnBooleanMapped { get; set; }

        public int? ColumnIntegerMapped { get; set; }

        public decimal? ColumnNumericMapped { get; set; }

        public float? ColumnRealMapped { get; set; }

        public short? ColumnSmallIntMapped { get; set; }

        public string ColumnTextMapped { get; set; }
    }
}
