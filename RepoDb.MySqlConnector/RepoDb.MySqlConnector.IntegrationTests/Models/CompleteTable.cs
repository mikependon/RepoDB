using System;

namespace RepoDb.MySqlConnector.IntegrationTests.Models
{
    public class CompleteTable
    {
        public Int64? Id { get; set; }
        public String ColumnVarchar { get; set; }
        public Int32? ColumnInt { get; set; }
        public Decimal? ColumnDecimal2 { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public Byte[] ColumnBlob { get; set; }
        public Byte[] ColumnBlobAsArray { get; set; }
        public Byte[] ColumnBinary { get; set; }
        public Byte[] ColumnLongBlob { get; set; }
        public Byte[] ColumnMediumBlob { get; set; }
        public Byte[] ColumnTinyBlob { get; set; }
        public Byte[] ColumnVarBinary { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public TimeSpan? ColumnTime { get; set; }
        public DateTime? ColumnTimeStamp { get; set; }
        public Int16? ColumnYear { get; set; }
        public Byte[] ColumnGeometry { get; set; }
        public Byte[] ColumnLineString { get; set; }
        public Byte[] ColumnMultiLineString { get; set; }
        public Byte[] ColumnMultiPoint { get; set; }
        public Byte[] ColumnMultiPolygon { get; set; }
        public Byte[] ColumnPoint { get; set; }
        public Byte[] ColumnPolygon { get; set; }
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
