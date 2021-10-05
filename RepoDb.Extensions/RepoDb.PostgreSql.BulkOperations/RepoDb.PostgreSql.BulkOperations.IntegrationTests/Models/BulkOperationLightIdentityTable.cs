using System;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    public class BulkOperationLightIdentityTable
    {
        public long Id { get; set; }
        public long? ColumnBigInt { get; set; }
        public bool? ColumnBoolean { get; set; }
        public int? ColumnInteger { get; set; }
        public decimal? ColumnNumeric { get; set; }
        public float? ColumnReal { get; set; }
        public short? ColumnSmallInt { get; set; }
        public string ColumnText { get; set; }
    }
}
