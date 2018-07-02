using RepoDb;
using RepoDb.Attributes;

namespace RepoDb.TestProject.Models
{
    [Map("Product")]
    public class ProductDto : DataEntity
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int? SupplierId { get; set; }
        public string Package { get; set; }
        public bool? IsDiscontinued { get; set; }
        public decimal? UnitPrice { get; set; }
    }
}
