using RepoDb;
using System;
using System.Collections.Generic;

namespace RepoDbRecursiveQuery.Models
{
    public class Order : DataEntity
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public decimal? TotalAmount { get; set; }
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }
}
