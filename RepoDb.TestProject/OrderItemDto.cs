using RepoDb;
using RepoDb.Attributes;

namespace RepoDb.TestProject
{
    [Map("OrderItem")]
    public class OrderItemDto : DataEntity
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
    }
}
