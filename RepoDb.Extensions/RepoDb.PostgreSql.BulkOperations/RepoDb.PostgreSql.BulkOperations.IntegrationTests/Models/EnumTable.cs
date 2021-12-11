using RepoDb.PostgreSql.BulkOperations.IntegrationTests.Enumerations;

namespace RepoDb.PostgreSql.BulkOperations.IntegrationTests.Models
{
    public class EnumTable
    {
        public long Id { get; set; }
        public Hands? ColumnEnumText { get; set; }
        public Hands? ColumnEnumInt { get; set; }
        public Hands? ColumnEnumHand { get; set; }
    }
}
