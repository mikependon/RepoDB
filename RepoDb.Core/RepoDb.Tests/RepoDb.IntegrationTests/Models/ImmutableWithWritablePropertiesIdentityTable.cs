using RepoDb.Attributes;
using System;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[sc].[IdentityTable]")]
    public class ImmutableWithWritablePropertiesIdentityTable
    {
        public ImmutableWithWritablePropertiesIdentityTable(long id,
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

        public long Id { get; set; }
        public Guid RowGuid { get; set; }
        public bool? ColumnBit { get; set; }
        public DateTime? ColumnDateTime { get; set; }
        public DateTime? ColumnDateTime2 { get; set; }
        public decimal? ColumnDecimal { get; set; }
        public double? ColumnFloat { get; set; }
        public int? ColumnInt { get; set; }
        public string ColumnNVarChar { get; set; }
    }
}
