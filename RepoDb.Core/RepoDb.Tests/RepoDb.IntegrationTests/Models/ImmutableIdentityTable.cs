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

        public long Id { get; private set; }
        public Guid RowGuid { get; private set; }
        public bool? ColumnBit { get; private set; }
        public DateTime? ColumnDateTime { get; private set; }
        public DateTime? ColumnDateTime2 { get; private set; }
        public decimal? ColumnDecimal { get; private set; }
        public double? ColumnFloat { get; private set; }
        public int? ColumnInt { get; private set; }
        public string ColumnNVarChar { get; private set; }

        // Backdoor Method

        public void SetColumnBit(bool value)
        {
            ColumnBit = value;
        }

        public void SetColumnDateTime2(DateTime value)
        {
            ColumnDateTime2 = value;
        }
    }
}
