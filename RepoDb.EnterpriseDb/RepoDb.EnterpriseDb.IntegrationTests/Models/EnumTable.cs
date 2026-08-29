using RepoDb.EnterpriseDb.IntegrationTests.Enumerations;

namespace RepoDb.EnterpriseDb.IntegrationTests.Models
{
    public class EnumTable
    {
        public long Id { get; set; }
        public Hands? ColumnEnumHand { get; set; }
    }
}
