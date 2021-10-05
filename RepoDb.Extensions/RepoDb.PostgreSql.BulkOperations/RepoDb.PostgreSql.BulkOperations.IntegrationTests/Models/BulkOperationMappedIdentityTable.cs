using RepoDb.Attributes;
using System;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    [Map("[dbo].[BulkOperationIdentityTable]")]
    public class BulkOperationMappedIdentityTable
    {
        [Map("Id")]
        public long IdMapped { get; set; }

        [Map("ColumnBigInt")]
        public long? ColumnBigIntMapped { get; set; }

        [Map("ColumnBoolean")]
        public bool? ColumnBooleanMapped { get; set; }

        [Map("ColumnInteger")]
        public int? ColumnIntegerMapped { get; set; }

        [Map("ColumnNumeric")]
        public decimal? ColumnNumericMapped { get; set; }

        [Map("ColumnReal")]
        public float? ColumnRealMapped { get; set; }

        [Map("ColumnSmallInt")]
        public short? ColumnSmallIntMapped { get; set; }

        [Map("ColumnText")]
        public string ColumnTextMapped { get; set; }
    }
}
