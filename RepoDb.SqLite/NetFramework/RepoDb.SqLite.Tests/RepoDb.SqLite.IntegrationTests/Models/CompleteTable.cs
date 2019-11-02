using RepoDb.Attributes;
using System;

namespace RepoDb.SqLite.IntegrationTests.Models
{
    public class CompleteTable
    {
        public long Id { get; set; }
        public long ColumnBigInt { get; set; }
        public byte[] ColumnBlob { get; set; }
        public bool ColumnBool { get; set; }
        public string ColumnChar { get; set; }
        public DateTime ColumnDate { get; set; }
        public DateTime ColumnDateTime { get; set; }
        public decimal ColumnDecimal { get; set; }
        public double ColumnDouble { get; set; }
        public long ColumnInteger { get; set; }
        public int ColumnInt { get; set; }
        public object ColumnNone { get; set; }
        public decimal ColumnNumeric { get; set; }
        public float ColumnReal { get; set; }
        public string ColumnString { get; set; }
        public string ColumnText { get; set; }
        public TimeSpan ColumnTime { get; set; }
        public string ColumnVarChar { get; set; }
    }
}
