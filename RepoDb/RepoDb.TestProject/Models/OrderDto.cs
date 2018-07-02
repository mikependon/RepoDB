using RepoDb;
using System;
using RepoDb.Attributes;

namespace RepoDb.TestProject.Models
{
    [Map("[dbo].[Order]")]
    public class OrderDto : DataEntity
    {
        public int Id { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderNumber { get; set; }
        public int? CustomerId { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
