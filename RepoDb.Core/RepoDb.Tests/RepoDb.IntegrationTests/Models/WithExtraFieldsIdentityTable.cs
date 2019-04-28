using RepoDb.Attributes;
using System;
using System.Collections.Generic;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[IdentityTable]")]
    public class WithExtraFieldsIdentityTable
    {
        /* Normal Fields */
        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public float? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
        /* Extra Fields */
        public string ExtraField { get; set; }
        public IEnumerable<IdentityTable> IdentitableTables { get; set; }
    }
}
