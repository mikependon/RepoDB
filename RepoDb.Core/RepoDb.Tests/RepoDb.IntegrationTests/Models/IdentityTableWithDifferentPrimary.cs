using System;
using System.Globalization;
using RepoDb.Attributes;
using RepoDb.Entity;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class IdentityTableWithDifferentPrimary : BaseEntity<IdentityTableWithDifferentPrimary>
    {
        public IdentityTableWithDifferentPrimary()
        {
            Map(p => ColumnDateTime).Convert<string>(x => DateTime.Parse(x, CultureInfo.InvariantCulture));
            Map(p => ColumnDateTime).Convert<DateTime, string>(x => x.ToString(CultureInfo.InvariantCulture));
        }

        public long Id { get; set; }
        [Primary]
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public string ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
