using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("CompleteTable")]
    public class NumbersClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public long? ColumnBigInt { get; set; }
        public bool? ColumnBit { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public float? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public decimal? ColumnMoney { get; set; }
        public decimal? ColumnNumeric { get; set; }
        public Single? ColumnReal { get; set; }
        public short? ColumnSmallInt { get; set; }
        public decimal? ColumnSmallMoney { get; set; }
    }

    [Map("CompleteTable")]
    public class BytesClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public byte[] ColumnBinary { get; set; }
        public byte[] ColumnImage { get; set; }
        public byte[] ColumnVarBinary { get; set; }
        public byte? ColumnTinyInt { get; set; }
    }

    [Map("CompleteTable")]
    public class StringsClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public string ColumnChar { get; set; }
        public string ColumnNChar { get; set; }
        public string ColumnNText { get; set; }
        public string ColumnNVarChar { get; set; }
        public string ColumnText { get; set; }
        public string ColumnVarChar { get; set; }
    }

    [Map("CompleteTable")]
    public class DatesClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public DateTimeOffset? ColumnDateTimeOffset { get; set; }
        public DateTime? ColumnSmallDateTime { get; set; }
        public TimeSpan? ColumnTime { get; set; }
    }

    [Map("CompleteTable")]
    public class SpatialsClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public object ColumnGeography { get; set; }
        public object ColumnGeometry { get; set; }
    }

    [Map("CompleteTable")]
    public class OthersClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        public Guid? ColumnUniqueIdentifier { get; set; }
        public string ColumnXml { get; set; }
        public object ColumnSqlVariant { get; set; }
        public object ColumnHierarchyId { get; set; }
    }

    [Map("CompleteTable")]
    public class TimestampClass
    {
        [Primary]
        public Guid SessionId { get; set; }
        /* Link: https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2005/ms182776(v=sql.90) */
        public byte[] ColumnTimeStamp { get; set; } // Cannot explicitly insert
    }
}
