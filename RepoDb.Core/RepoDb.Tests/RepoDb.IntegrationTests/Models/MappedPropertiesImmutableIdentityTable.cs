using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class MappedPropertiesImmutableIdentityTable
    {
        public MappedPropertiesImmutableIdentityTable(long id,
            Guid rowGuidMapped,
            bool? columnBitMapped,
            DateTime? columnDateTimeMapped,
            DateTime? columnDateTime2Mapped,
            decimal? columnDecimalMapped,
            double? columnFloatMapped,
            int? columnIntMapped,
            string columnNVarCharMapped)
        {
            Id = id;
            RowGuidMapped = rowGuidMapped;
            ColumnBitMapped = columnBitMapped;
            ColumnDateTimeMapped = columnDateTimeMapped;
            ColumnDateTime2Mapped = columnDateTime2Mapped;
            ColumnDecimalMapped = columnDecimalMapped;
            ColumnFloatMapped = columnFloatMapped;
            ColumnIntMapped = columnIntMapped;
            ColumnNVarCharMapped = columnNVarCharMapped;
        }

        // Properties

        public long Id { get; }
        [Map("RowGuid")]
        public Guid RowGuidMapped { get; }
        [Map("ColumnBit")]
        public bool? ColumnBitMapped { get; }
        [Map("ColumnDateTime")]
        public DateTime? ColumnDateTimeMapped { get; }
        [Map("ColumnDateTime2")]
        public DateTime? ColumnDateTime2Mapped { get; }
        [Map("ColumnDecimal")]
        public decimal? ColumnDecimalMapped { get; }
        [Map("ColumnFloat")]
        public double? ColumnFloatMapped { get; }
        [Map("ColumnInt")]
        public int? ColumnIntMapped { get; }
        [Map("ColumnNVarChar")]
        public string ColumnNVarCharMapped { get; }
    }
}
