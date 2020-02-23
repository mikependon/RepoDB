using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.SqlServer.BulkOperations.IntegrationTests.Models
{
    [Map("[dbo].[BulkInsertIdentityTable]")]
    public class WithExtraFieldsBulkInsertIdentityTable
    {
        /* Normal Fields */
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
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
