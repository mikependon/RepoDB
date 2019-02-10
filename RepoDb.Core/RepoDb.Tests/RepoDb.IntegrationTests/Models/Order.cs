using System;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Collections.Generic;

namespace RepoDb.IntegrationTests.Models
{
    [Map("[dbo].[Order]")]
    public class Order
    {
        [Identity(), Map("Id")]
        public int Id { get; set; }
        public Guid GlobalId { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public long CustomerId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Freight { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDue { get; set; }
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