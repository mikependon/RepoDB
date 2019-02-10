using System;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[OrderDetail]")]
    public class OrderDetail
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

        private DateTime dateInsertedUtc;

        public DateTime GetDateInsertedUtc()
        {
            return dateInsertedUtc;
        }

        public void SetDateInsertedUtc(DateTime value)
        {
            dateInsertedUtc = value;
        }

        public string LastUserId { get; set; }
    }
}