using System;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// Non-identity counterpart of <see cref="BulkOperationIdentityTable"/> - <c>Id</c> is a plain
    /// <c>BIGINT</c> primary key (no <c>GENERATED ... AS IDENTITY</c>), so the caller's value is always
    /// stored as-is. Useful for tests that need to know a row's <c>Id</c> ahead of time.
    /// </summary>
    public class BulkOperationNonIdentityTable
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
