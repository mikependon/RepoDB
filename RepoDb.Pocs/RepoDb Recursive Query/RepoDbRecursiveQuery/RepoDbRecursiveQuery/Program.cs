using RepoDb;
using RepoDbRecursiveQuery.Models;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDbRecursiveQuery
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = @"Server=.;Database=DoFactory;Integrated Security=SSPI;";
            var now = DateTime.UtcNow;

            using (var repository = new DbRepository<SqlConnection>(connectionString, connectionPersistency: RepoDb.Enumerations.ConnectionPersistency.Instance))
            {

                // Get all the customers
                var customers = repository.Query<Customer>(recursive: true);

                //customers.ToList().ForEach(customer =>
                //{

                //    // Get the orders
                //    var orders = repository.Query<Order>(new { CustomerId = customer.Id });

                //    orders.ToList().ForEach(order =>
                //    {

                //        // Get all order items
                //        var orderItems = repository.Query<OrderItem>(new { OrderId = order.Id });

                //    });

                //});

            }

            Console.WriteLine($"Elapsed time is {(DateTime.UtcNow - now).TotalSeconds} seconds.");

            // Stuck here
            Console.ReadLine();
        }
    }
}