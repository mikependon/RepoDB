using RepoDb;

namespace RepoDbRecursiveQuery.Models
{
    public class OrderItem : DataEntity
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
    }
}
