using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class ImmutableWithFewerCtorArgumentsIdentityTable
    {
        public ImmutableWithFewerCtorArgumentsIdentityTable(long id,
            Guid rowGuid,
            bool? columnBit,
            DateTime? columnDateTime,
            DateTime? columnDateTime2)
        {
            Id = id;
            RowGuid = rowGuid;
            ColumnBit = columnBit;
            ColumnDateTime = columnDateTime;
            ColumnDateTime2 = columnDateTime2;
        }

        // Properties

        public long Id { get; }
        public Guid RowGuid { get; }
        public bool? ColumnBit { get; }
        public DateTime? ColumnDateTime { get; }
        public DateTime? ColumnDateTime2 { get; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
