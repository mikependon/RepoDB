using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    public abstract class BaseInheritedIdentityTable
    {
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
    }

    public abstract class LayeredPropertiesIdentityTable : BaseInheritedIdentityTable
    {
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }

    [Map("[sc].[IdentityTable]")]
    public class InheritedIdentityTable : LayeredPropertiesIdentityTable
    { }
}
