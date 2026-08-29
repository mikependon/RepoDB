using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.MariaDb.BulkOperations.IntegrationTests.Models
{
    [Map("BulkOperationIdentityTable")]
    public class WithExtraFieldsBulkOperationIdentityTable
    {
        /* Normal Fields */
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public byte? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
        /* Extra Fields */
        public string ExtraField { get; set; }
        public IEnumerable<BulkOperationIdentityTable> IdentityTables { get; set; }
    }
}
