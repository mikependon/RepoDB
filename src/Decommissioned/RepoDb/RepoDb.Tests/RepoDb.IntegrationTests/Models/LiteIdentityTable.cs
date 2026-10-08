#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class LiteIdentityTable
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public int? ColumnInt { get; set; }
    }
}
