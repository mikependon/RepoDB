using System;

namespace RepoDb.MariaDb.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// MariaDb has no native BOOLEAN column usable for parameter binding (MariaDbType has no Boolean
    /// member either), so the SqlServer suite's BIT column
    /// is represented here as a NUMBER(1,0)-backed <see cref="byte"/>? instead of bool?. RowGuid is
    /// stored as RAW(16) and round-tripped via <c>MariaDbGuidToByteArrayPropertyHandler</c> (registered
    /// per-property in Setup/Database.cs), the same pattern used by RepoDb.MariaDb.IntegrationTests.
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
