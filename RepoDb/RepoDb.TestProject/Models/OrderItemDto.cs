using RepoDb;
using RepoDb.Attributes;
using System.Collections.Generic;

namespace RepoDb.TestProject.Models
{
    [Map("OrderItem")]
    public class OrderItemDto : DataEntity
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public decimal? UnitPrice { get; set; }
        public int? Quantity { get; set; }
        [Foreign("ProductId", "Id")]
        public IEnumerable<ProductDto> Products { get; set; }
    }
}
