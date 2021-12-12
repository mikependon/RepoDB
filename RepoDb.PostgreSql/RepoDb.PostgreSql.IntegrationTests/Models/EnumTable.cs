using RepoDb.PostgreSql.IntegrationTests.Enumerations;

namespace RepoDb.PostgreSql.IntegrationTests.Models
{
    public class EnumTable
    {
        public long Id { get; set; }
        public Hands? ColumnEnumHand { get; set; }
    }
}
