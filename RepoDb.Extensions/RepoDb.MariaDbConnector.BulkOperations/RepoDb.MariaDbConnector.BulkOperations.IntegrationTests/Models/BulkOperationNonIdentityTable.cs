using System;

namespace RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// Same shape and columns as <see cref="BulkOperationIdentityTable"/>, except <see cref="Id"/> is a
    /// plain (non-identity) primary key - the caller's value is stored as-is, rather than being
    /// overridden by an MariaDb-generated identity/sequence value. Used by tests that need to know a
    /// row's <see cref="Id"/> ahead of time (e.g. to build a separate object - anonymous, expando, etc. -
    /// that must match an already-inserted row by primary key), since an IDENTITY column's
    /// server-generated value can't be predicted or reused for that purpose.
    /// </summary>
    public class BulkOperationNonIdentityTable
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
