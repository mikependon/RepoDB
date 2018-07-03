using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RepoDb
{
    public class Program
    {
        public class Order : DataEntity
        {
            public int Id { get; set; }
            public long CustomerId { get; set; }
            public int Quantity { get; set; }
            public double Price { get; set; }
            public DateTime CreatedDate { get; set; }
        }

        public class Customer : DataEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<Order> Orders { get; set; }
            public DateTime CreateDdate { get; set; }
        }

        public static void Main(string[] args)
        {
            var repository = new DbRepository<SqlConnection>("ConnectionString");
            repository.Query<Customer>(new { Id = 10045 }, recursive: true);
        }
    }
}
