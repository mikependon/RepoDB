using System;
using RepoDb.Attributes.Parameter.ClickHouse;

namespace RepoDb.ClickHouse.IntegrationTests.Models
{
    public class CompleteTable
    {
        public Int64? Id { get; set; }
        public String ColumnVarchar { get; set; }
        public Int32? ColumnInt { get; set; }
        public Decimal? ColumnDecimal2 { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public String ColumnBlob { get; set; }
        public String ColumnBlobAsArray { get; set; }
        public String ColumnBinary { get; set; }
        public String ColumnLongBlob { get; set; }
        public String ColumnMediumBlob { get; set; }
        public String ColumnTinyBlob { get; set; }
        public String ColumnVarBinary { get; set; }
        public DateTime? ColumnDate { get; set; }
        [ClickHouseType("Nullable(DateTime64(3))")]
        public DateTime? ColumnDateTime2 { get; set; }
        public String ColumnTime { get; set; }
        [ClickHouseType("Nullable(DateTime64(3))")]
        public DateTime? ColumnTimeStamp { get; set; }
        public Int16? ColumnYear { get; set; }
        public String ColumnGeometry { get; set; }
        public String ColumnLineString { get; set; }
        public String ColumnMultiLineString { get; set; }
        public String ColumnMultiPoint { get; set; }
        public String ColumnMultiPolygon { get; set; }
        public String ColumnPoint { get; set; }
        public String ColumnPolygon { get; set; }
        public Int64? ColumnBigint { get; set; }
        public Decimal? ColumnDecimal { get; set; }
        public Double? ColumnDouble { get; set; }
        public Single? ColumnFloat { get; set; }
        public Int32? ColumnInt2 { get; set; }
        public Int32? ColumnMediumInt { get; set; }
        public Double? ColumnReal { get; set; }
        public Int16? ColumnSmallInt { get; set; }
        public SByte? ColumnTinyInt { get; set; }
        public String ColumnChar { get; set; }
        public String ColumnJson { get; set; }
        public String ColumnNChar { get; set; }
        public String ColumnNVarChar { get; set; }
        public String ColumnLongText { get; set; }
        public String ColumnMediumText { get; set; }
        public String ColumnText { get; set; }
        public String ColumnTinyText { get; set; }
        public UInt64? ColumnBit { get; set; }
    }
}
