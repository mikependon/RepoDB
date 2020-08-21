using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class ImmutableIdentityTable
    {
        public ImmutableIdentityTable(long id,
            Guid rowGuid,
            bool? columnBit,
            DateTime? columnDateTime,
            DateTime? columnDateTime2,
            decimal? columnDecimal,
            double? columnFloat,
            int? columnInt,
            string columnNVarChar)
        {
            Id = id;
            RowGuid = rowGuid;
            ColumnBit = columnBit;
            ColumnDateTime = columnDateTime;
            ColumnDateTime2 = columnDateTime2;
            ColumnDecimal = columnDecimal;
            ColumnFloat = columnFloat;
            ColumnInt = columnInt;
            ColumnNVarChar = columnNVarChar;
        }

        // Properties

        public long Id { get; }
        public Guid RowGuid { get; }
        public bool? ColumnBit { get; }
        public DateTime? ColumnDateTime { get; }
        public DateTime? ColumnDateTime2 { get; }
        public decimal? ColumnDecimal { get; }
        public double? ColumnFloat { get; }
        public int? ColumnInt { get; }
        public string ColumnNVarChar { get; }
    }
}
