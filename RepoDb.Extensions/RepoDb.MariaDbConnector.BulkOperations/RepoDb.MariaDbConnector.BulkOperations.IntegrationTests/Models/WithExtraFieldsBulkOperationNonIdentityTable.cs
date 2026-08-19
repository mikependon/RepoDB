using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.MariaDbConnector.BulkOperations.IntegrationTests.Models
{
    [Map("BulkOperationNonIdentityTable")]
    public class WithExtraFieldsBulkOperationNonIdentityTable
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
        public IEnumerable<BulkOperationNonIdentityTable> NonIdentityTables { get; set; }
    }
}
