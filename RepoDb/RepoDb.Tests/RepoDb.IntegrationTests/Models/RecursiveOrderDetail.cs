using System;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[OrderDetail]")]
    public class RecursiveOrderDetail : DataEntity
    {
        [Identity]
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public int OrderId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        [Attributes.Ignore(Command.Update)]
        public DateTime DateInsertedUtc { get; set; }
        public string LastUserId { get; set; }

        /*
         * Recursive
         */

        // Parent Orders (Hack the Foreign)

        [Foreign("OrderId", "Id")]
        public IEnumerable<RecursiveOrder> Orders { get; set; }
    }
}