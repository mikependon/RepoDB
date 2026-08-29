#region Copyright Attributions

// Copyright (c) 2021 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    public class EnumTable
    {
        public long Id { get; set; }
        public Hands? ColumnEnumText { get; set; }
        public Hands? ColumnEnumInt { get; set; }
        public Hands? ColumnEnumHand { get; set; }
    }
}
