using RepoDb.Attributes;
using System;

namespace RepoDb.Firebird.BulkOperations.IntegrationTests.Models
{
    [Map("BulkOperationNonIdentityTable")]
    public class BulkOperationMappedNonIdentityTable
    {
        [Map("Id")]
        public long IdMapped { get; set; }
        [Map("RowGuid")]
        public byte[] RowGuidMapped { get; set; }
        [Map("ColumnBit")]
        public bool? ColumnBitMapped { get; set; }
        [Map("ColumnDateTime")]
        public DateTime? ColumnDateTimeMapped { get; set; }
        [Map("ColumnDateTime2")]
        public DateTime? ColumnDateTime2Mapped { get; set; }
        [Map("ColumnDecimal")]
        public decimal? ColumnDecimalMapped { get; set; }
        [Map("ColumnFloat")]
        public double? ColumnFloatMapped { get; set; }
        [Map("ColumnInt")]
        public int? ColumnIntMapped { get; set; }
        [Map("ColumnNVarChar")]
        public string ColumnNVarCharMapped { get; set; }
    }
}
