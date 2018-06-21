using RepoDb.Attributes;

namespace RepoDb.TestProject.Models
{

    [Map("Supplier")]
    public class SupplierDto : DataEntity
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}
