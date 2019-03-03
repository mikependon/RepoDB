using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{

    [Map("CompleteTable")]
    public class NumbersMappedClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnBigInt")]
        public long? ColumnBigIntMapped { get; set; }
        [Map("ColumnBit")]
        public bool? ColumnBitMapped { get; set; }
        [Map("ColumnDecimal")]
        public decimal? ColumnDecimalMapped { get; set; }
        [Map("ColumnFloat")]
        public float? ColumnFloatMapped { get; set; }
        [Map("ColumnInt")]
        public int? ColumnIntMapped { get; set; }
        [Map("ColumnMoney")]
        public decimal? ColumnMoneyMapped { get; set; }
        [Map("ColumnNumeric")]
        public decimal? ColumnNumericMapped { get; set; }
        [Map("ColumnReal")]
        public Single? ColumnRealMapped { get; set; }
        [Map("ColumnSmallInt")]
        public short? ColumnSmallIntMapped { get; set; }
        [Map("ColumnSmallMoney")]
        public decimal? ColumnSmallMoneyMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class BytesMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnBinary")]
        public byte[] ColumnBinaryMapped { get; set; }
        [Map("ColumnImage")]
        public byte[] ColumnImageMapped { get; set; }
        [Map("ColumnVarBinary")]
        public byte[] ColumnVarBinaryMapped { get; set; }
        [Map("ColumnTinyInt")]
        public byte? ColumnTinyIntMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class StringsMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnChar")]
        public string ColumnCharMapped { get; set; }
        [Map("ColumnNChar")]
        public string ColumnNCharMapped { get; set; }
        [Map("ColumnNText")]
        public string ColumnNTextMapped { get; set; }
        [Map("ColumnNVarChar")]
        public string ColumnNVarCharMapped { get; set; }
        [Map("ColumnText")]
        public string ColumnTextMapped { get; set; }
        [Map("ColumnVarChar")]
        public string ColumnVarCharMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DatesMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDate")]
        public DateTime? ColumnDateMapped { get; set; }
        [Map("ColumnDateTime")]
        public DateTime? ColumnDateTimeMapped { get; set; }
        [Map("ColumnDateTime2")]
        public DateTime? ColumnDateTime2Mapped { get; set; }
        [Map("ColumnDateTimeOffset")]
        public DateTimeOffset? ColumnDateTimeOffsetMapped { get; set; }
        [Map("ColumnSmallDateTime")]
        public DateTime? ColumnSmallDateTimeMapped { get; set; }
        [Map("ColumnTime")]
        public TimeSpan? ColumnTimeMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class SpatialsMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnGeography")]
        public object ColumnGeographyMapped { get; set; }
        [Map("ColumnGeometry")]
        public object ColumnGeometryMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class OthersMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnUniqueIdentifier")]
        public Guid? ColumnUniqueIdentifierMapped { get; set; }
        [Map("ColumnXml")]
        public string ColumnXmlMapped { get; set; }
        [Map("ColumnSqlVariant")]
        public object ColumnSqlVariantMapped { get; set; }
        [Map("ColumnHierarchyId")]
        public object ColumnHierarchyIdMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class TimestampMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        /* Link: https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2005/ms182776(v=sql.90) */
        [Map("ColumnTimeStamp")]
        public byte[] ColumnTimeStampMapped { get; set; } // Cannot explicitly insert
    }
}
