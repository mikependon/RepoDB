using System;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// Firebird has native <c>BOOLEAN</c> support, unlike DB2/MySQL - <c>ColumnBit</c> is a plain
    /// <see cref="bool"/>?, no property handler needed. <c>RowGuid</c> is stored as
    /// <c>CHAR(16) CHARACTER SET OCTETS</c> and mapped as a plain <see cref="byte"/>[] - Firebird has no
    /// native GUID type either, and a fixed-width binary column round-trips a byte array directly with no
    /// handler required.
    /// </summary>
    public class BulkOperationIdentityTable
    {
        public long Id { get; set; }
        public byte[] RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
