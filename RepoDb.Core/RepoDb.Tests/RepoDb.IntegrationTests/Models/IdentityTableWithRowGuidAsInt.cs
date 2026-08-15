using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    /// <summary>
    /// Mapped to the same table as <see cref="IdentityTable"/>, except the <see cref="RowGuid"/> column
    /// (a SQL <c>uniqueidentifier</c>) is bound to an <see cref="int"/> property. This is a conversion
    /// that is expected to fail regardless of <see cref="ConversionType"/>.
    /// </summary>
    [Map("[sc].[IdentityTable]")]
    public class IdentityTableWithRowGuidAsInt
    {
        public long Id { get; set; }
        public int RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
