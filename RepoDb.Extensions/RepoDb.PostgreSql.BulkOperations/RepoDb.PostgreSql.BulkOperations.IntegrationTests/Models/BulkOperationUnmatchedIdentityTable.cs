using RepoDb.Attributes;
using System;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    /*
     * Useful for NpgsqlBulkInsertMapItem mappings
     */

    [Map("[dbo].[BulkOperationIdentityTable]")]
    public class BulkOperationUnmatchedIdentityTable
    {
        [Identity]
        public long IdMapped { get; set; }

        public long? ColumnBigIntMapped { get; set; }

        public bool? ColumnBooleanMapped { get; set; }

        public int? ColumnIntegerMapped { get; set; }

        public decimal? ColumnNumericMapped { get; set; }

        public float? ColumnRealMapped { get; set; }

        public short? ColumnSmallIntMapped { get; set; }

        public string ColumnTextMapped { get; set; }
    }
}
