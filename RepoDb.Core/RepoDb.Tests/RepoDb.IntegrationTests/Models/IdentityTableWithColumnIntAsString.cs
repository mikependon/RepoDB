using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    /// <summary>
    /// Mapped to the same table as <see cref="IdentityTable"/>, except the <see cref="ColumnInt"/> column
    /// (a SQL <c>int</c>) is bound to a <see cref="string"/> property instead of <see cref="int"/>?.
    /// This is used to exercise the parameter-value conversion that <see cref="ConversionType.Automatic"/>
    /// performs before the value is sent to the database.
    /// </summary>
    [Map("[sc].[IdentityTable]")]
    public class IdentityTableWithColumnIntAsString
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public string ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
