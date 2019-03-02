using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("CompleteTable")]
    public class BigIntMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnBigInt")]
        public long? ColumnBigIntMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class BinaryMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnBinary")]
        public byte[] ColumnBinaryMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class BitMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnBit")]
        public bool? ColumnBitMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class CharMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnChar")]
        public string ColumnCharMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DateMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDate")]
        public DateTime? ColumnDateMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTimeMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDateTime")]
        public DateTime? ColumnDateTimeMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTime2MapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDateTime2")]
        public DateTime? ColumnDateTime2Mapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTimeOffsetMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDateTimeOffset")]
        public DateTimeOffset? ColumnDateTimeOffsetMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class DecimalMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnDecimal")]
        public decimal? ColumnDecimalMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class FloatMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnFloat")]
        public float? ColumnFloatMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class GeographyMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnGeography")]
        public object ColumnGeographyMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class GeometryMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnGeometry")]
        public object ColumnGeometryMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class HierarchyIdMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnHierarchyId")]
        public object ColumnHierarchyIdMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class ImageMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnImage")]
        public byte[] ColumnImageMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class IntMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnInt")]
        public int? ColumnIntMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class MoneyMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnMoney")]
        public decimal? ColumnMoneyMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class NCharMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnNChar")]
        public string ColumnNCharMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class NTextMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnNText")]
        public string ColumnNTextMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class NumericMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnNumeric")]
        public decimal? ColumnNumericMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class NVarCharMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnNVarChar")]
        public string ColumnNVarCharMapped { get; set; }
    }

    [Map("CompleteTable")]
    public class RealMapClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        [Map("ColumnReal")]
        public Single? ColumnRealMapped { get; set; }
    }
}
