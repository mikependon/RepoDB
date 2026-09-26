#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.CockroachDb.IntegrationTests.Models
{
    public class CompleteTable
    {
        public System.Int64 Id { get; set; }
        public System.Nullable<System.Char> ColumnChar { get; set; }
        public System.Nullable<System.Int64> ColumnBigInt { get; set; }
        public System.Array ColumnBigIntAsArray { get; set; }
        public System.Nullable<System.Int64> ColumnBigSerial { get; set; }
        public System.Nullable<System.Boolean> ColumnBoolean { get; set; }
        public System.String ColumnCharacter { get; set; }
        public System.String ColumnCharacterVarying { get; set; }
        public System.Nullable<System.DateOnly> ColumnDate { get; set; }
        public System.Array ColumnDateAsArray { get; set; }
        public System.Nullable<System.Int32> ColumnInteger { get; set; }
        public System.Array ColumnIntegerAsArray { get; set; }
        public System.Nullable<System.TimeSpan> ColumnInterval { get; set; }
        public System.Array ColumnIntervalAsArray { get; set; }
        public System.Nullable<System.Decimal> ColumnNumeric { get; set; }
        public System.String ColumnName { get; set; }
        public System.Nullable<System.Single> ColumnReal { get; set; }
        public System.Nullable<System.Int16> ColumnSmallInt { get; set; }
        public System.String ColumnText { get; set; }
        public System.Nullable<System.DateTimeOffset> ColumnTimestampWithTimeZone { get; set; }
        public System.Nullable<System.DateTime> ColumnTimestampWithoutTimeZone { get; set; }
    }
}
