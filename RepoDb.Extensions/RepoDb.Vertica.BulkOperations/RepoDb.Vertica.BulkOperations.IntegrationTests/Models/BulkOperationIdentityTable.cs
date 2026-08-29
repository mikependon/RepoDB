using System;

namespace RepoDb.Vertica.BulkOperations.IntegrationTests.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class BulkOperationIdentityTable
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
