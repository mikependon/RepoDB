using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    public class CompleteTable
    {
        [Primary]
        public Guid SessionId { get; set; }
        public long? ColumnBigInt { get; set; }
        public byte[] ColumnBinary { get; set; }
        public bool? ColumnBit { get; set; }
        public string ColumnChar { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public DateTimeOffset? ColumnDateTimeOffset { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public float? ColumnFloat { get; set; }
        public object ColumnGeography { get; set; }
        public object ColumnGeometry { get; set; }
        public object ColumnHierarchyId { get; set; }
        public byte[] ColumnImage { get; set; }
        public int? ColumnInt { get; set; }
        public decimal? ColumnMoney { get; set; }
        public string ColumnNChar { get; set; }
        public string ColumnNText { get; set; }
        public decimal? ColumnNumeric { get; set; }
        public string ColumnNVarChar { get; set; }
        public Single? ColumnReal { get; set; }
    }

    [Map("CompleteTable")]
    public class BigIntClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public long? ColumnBigInt { get; set; }
    }

    [Map("CompleteTable")]
    public class BinaryClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public byte[] ColumnBinary { get; set; }
    }

    [Map("CompleteTable")]
    public class BitClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public bool? ColumnBit { get; set; }
    }

    [Map("CompleteTable")]
    public class CharClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public string ColumnChar { get; set; }
    }

    [Map("CompleteTable")]
    public class DateClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public DateTime? ColumnDate { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTimeClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public DateTime? ColumnDateTime { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTime2Class
    {
        [Primary]
        public Guid SessionId { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
    }

    [Map("CompleteTable")]
    public class DateTimeOffsetClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public DateTimeOffset? ColumnDateTimeOffset { get; set; }
    }

    [Map("CompleteTable")]
    public class DecimalClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public decimal? ColumnDecimal { get; set; }
    }

    [Map("CompleteTable")]
    public class FloatClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public float? ColumnFloat { get; set; }
    }

    [Map("CompleteTable")]
    public class GeographyClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public object ColumnGeography { get; set; }
    }

    [Map("CompleteTable")]
    public class GeometryClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public object ColumnGeometry { get; set; }
    }

    [Map("CompleteTable")]
    public class HierarchyIdClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public object ColumnHierarchyId { get; set; }
    }

    [Map("CompleteTable")]
    public class ImageClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public byte[] ColumnImage { get; set; }
    }

    [Map("CompleteTable")]
    public class IntClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public int? ColumnInt { get; set; }
    }

    [Map("CompleteTable")]
    public class MoneyClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public decimal? ColumnMoney { get; set; }
    }

    [Map("CompleteTable")]
    public class NCharClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public string ColumnNChar { get; set; }
    }

    [Map("CompleteTable")]
    public class NTextClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public string ColumnNText { get; set; }
    }

    [Map("CompleteTable")]
    public class NumericClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public decimal? ColumnNumeric { get; set; }
    }

    [Map("CompleteTable")]
    public class NVarCharClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public string ColumnNVarChar { get; set; }
    }

    [Map("CompleteTable")]
    public class RealClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public Single? ColumnReal { get; set; }
    }
}
