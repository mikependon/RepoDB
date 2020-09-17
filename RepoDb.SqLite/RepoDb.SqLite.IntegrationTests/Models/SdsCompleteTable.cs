using System;

namespace RepoDb.SqLite.IntegrationTests.Models
{
    public class SdsCompleteTable
    {
        public Int64 Id { get; set; }
        public Int64? ColumnBigInt { get; set; }
        public Byte[] ColumnBlob { get; set; }
        public Boolean? ColumnBoolean { get; set; }
        public String ColumnChar { get; set; }
        public DateTime? ColumnDate { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public Decimal? ColumnDecimal { get; set; }
        public Double? ColumnDouble { get; set; }
        public Int64? ColumnInteger { get; set; }
        public Int32? ColumnInt { get; set; }
        public Object ColumnNone { get; set; }
        public Decimal? ColumnNumeric { get; set; }
        public Double? ColumnReal { get; set; }
        public String ColumnString { get; set; }
        public String ColumnText { get; set; }
        public DateTime? ColumnTime { get; set; }
        public String ColumnVarChar { get; set; }
    }
}
