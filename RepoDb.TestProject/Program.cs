using System;
using Dapper;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.Enumerations;

namespace RepoDb.TestProject
{
    class Program
    {
        private static readonly string RepoDbConnectionString = @"Server=.;Database=RepoDb;Integrated Security=True;";
        private static readonly string InventoryConnectionString = "Initial Catalog=.;Database=Northwind;Integrated Security=True;";

        static void Main(string[] args)
        {
            //TestInNotInBetweenNotBetweenAnyAllOperation();
            //InventoryMain();
            //RepoDbMain();
            TestCrud();
        }

        public static void InventoryMain()
        {
            var repository = new DbRepository<SqlConnection>(InventoryConnectionString, null, null, new InventoryTrace());
            var customers = repository.Query<CustomerDto>();
            customers.ToList().ForEach(customer =>
            {
                var rows = repository.InlineUpdate<CustomerDto>(new { customer.FirstName }, new { customer.Id }, true);
                rows = Convert.ToInt32(repository.Insert(customer));
                rows = repository.Update(customer);
                rows = repository.Merge(customer, Field.Parse(new { customer.Id }));

                // Customer
                Console.WriteLine($"Customer: {customer.FirstName} {customer.LastName} from {customer.City}, {customer.Country}");
                // Orders
                var orders = repository.Query<OrderDto>(new { CustomerId = customer.Id });
                orders.ToList().ForEach(order =>
                {
                    Console.WriteLine($"   Order: {order.OrderNumber}, " +
                        $"Date: {order.OrderDate.GetValueOrDefault().ToString("u")}, Total Amount: {order.TotalAmount}");
                    // OrderItem
                    var orderItem = repository.Query<OrderItemDto>(new { OrderId = order.Id }).FirstOrDefault();
                    if (orderItem != null)
                    {
                        // Product
                        var product = repository.Query<ProductDto>(new { Id = orderItem.ProductId }).FirstOrDefault();
                        if (product != null)
                        {
                            Console.WriteLine($"      Product: {product.ProductName}, Price: {orderItem.UnitPrice}, Quantity: {orderItem.Quantity}, Total: {orderItem.UnitPrice * orderItem.Quantity} ");
                            // Supplier
                            var supplier = repository.Query<SupplierDto>(new { Id = product.SupplierId }).FirstOrDefault();
                            if (supplier != null)
                            {
                                Console.WriteLine($"      Supplier: {supplier.CompanyName}, City: {supplier.City}, Country: {supplier.Country}, Contact: {supplier.ContactName}, Phone: {supplier.Phone} ");
                            }
                        }
                    }
                });
                //Console.ReadLine();
            });
            Console.ReadLine();
        }

        public static void RepoDbMain()
        {
            //DataEntityMapper.For<Person>()
            //    .On(Command.Query, "[dbo].[Person]")
            //    .On(Command.Delete, "[dbo].[sp_delete_person]", CommandType.StoredProcedure)
            //    .On(Command.Insert, "[dbo].[Person]")
            //    .On(Command.Update, "[dbo].[sp_update_person]", CommandType.StoredProcedure)
            //    .On(Command.BulkInsert, "[dbo].[Person]");

            Console.WriteLine("Started");
            //TestBulkInsert();
            var rows = 1000000;
            TestDapper(rows);
            TestEntityFramework(rows);
            TestRepoDbQuery(rows);
            //TestRepoDbExecuteQuery(rows);
            //TestDapperLoop();
            //TestRepoDbQueryLoop();
            //TestInNotInBetweenNotBetweenAnyAllOperation();
            //TestInlineUpdate();
            //TestCrud();
            //TestBatchQuery();
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }

        private static void TestBulkInsert()
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var people = (IEnumerable<Person>)null;
            var rows = 500000;
            var now = DateTime.UtcNow;
            Console.WriteLine($"RepoDb.DbRepository.BulkInsert({rows})");
            people = repository.Query(top: rows);
            repository.BulkInsert(people);
            Console.WriteLine($"RepoDb: Bulk inserted {rows} rows for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestDapper(int rows)
        {
            var people = (IEnumerable<Person>)null;
            var now = DateTime.UtcNow;
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("Dapper.DbConnection.Query<T>");
            using (var connection = new SqlConnection(RepoDbConnectionString))
            {
                people = connection.Query<Person>(sql: $"SELECT TOP {rows} * FROM [dbo].[Person];");
            }
            Console.WriteLine($"Dapper: {people.Count()} rows for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestEntityFramework(int rows)
        {
            var now = DateTime.UtcNow;
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("EntityFramework.Database.SqlQuery<T>");
            var context = new System.Data.Entity.DbContext(RepoDbConnectionString);
            var people = context.Database.SqlQuery<Person>($"SELECT TOP {rows} * FROM [dbo].[Person];");
            Console.WriteLine($"EntityFramework: {people.Where((p) => true).Count()} rows for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestRepoDbQuery(int rows)
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var people = (IEnumerable<Person>)null;
            var now = DateTime.UtcNow;
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("RepoDb.DbRepository.Query");
            people = repository.Query(top: rows);
            Console.WriteLine($"RepoDb: {people.Count()} rows for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestRepoDbExecuteQuery(int rows)
        {
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("RepoDb.Extensions.DbConnectionExtension.ExecuteQuery");
            using (var connection = new SqlConnection(RepoDbConnectionString))
            {
                var now = DateTime.UtcNow;
                var objects = connection.ExecuteQuery($"SELECT TOP {rows} * FROM [dbo].[Person];");
                Console.WriteLine($"RepoDb: {objects.Count()} rows for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
            }
        }

        private static void TestDapperLoop()
        {
            var people = (IEnumerable<Person>)null;
            var loops = 0;
            var now = DateTime.UtcNow;
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("Dapper: Looping Query Execution");
            using (var connection = new SqlConnection(RepoDbConnectionString))
            {
                while (loops < 500)
                {
                    people = connection.Query<Person>(sql: $"SELECT TOP {10} * FROM [dbo].[Person];");
                    loops++;
                }
            }
            Console.WriteLine($"Dapper: {loops} loops (top 10 rows each) for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestRepoDbQueryLoop()
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var people = (IEnumerable<Person>)null;
            var loops = 0;
            var now = DateTime.UtcNow;
            Console.WriteLine(new string(char.Parse("-"), 50));
            Console.WriteLine("RepoDb: Looping Query Execution");
            while (loops < 500)
            {
                people = repository.Query(top: 10);
                loops++;
            }
            Console.WriteLine($"RepoDb: {loops} loops (top 10 rows each) for {(DateTime.UtcNow - now).TotalSeconds} second(s).");
        }

        private static void TestInNotInBetweenNotBetweenAnyAllOperation()
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var people = (IEnumerable<Person>)null;

            // Combined
            people = repository.Query(new
            {
                Id = new
                {
                    Operation = Operation.All,
                    Value = new object[]
                    {
                        new { Operation = Operation.In, Value = new int[]{ 5000, 5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008, 5009, 5010 } },
                        new { Operation = Operation.NotIn, Value = new int[]{ 5002, 5003, 5009 } },
                        new { Operation = Operation.Between, Value = new object[]{ 5001, 5010 } },
                        new { Operation = Operation.NotBetween, Value = new int[]{ 5006, 5010 } }
                    }
                }
            });
            // Expect: 5001, 5004, 5005

            // In
            people = repository.Query(new
            {
                Id = new { Operation = Operation.In, Value = new[] { 6000, 6001, 6002, 6003, 6004, 6005 } }
            });
            // Expect: 6000, 6001, 6002, 6003, 6004, 6005

            // Between and In
            people = repository.Query(new
            {
                QueryGroups = new[]
                {
                    new { Id = new { Operation = Operation.Between, Value = new[] { 6000, 6010 } } },
                    new { Id = new { Operation = Operation.In, Value = new[] { 6000, 6001, 6002, 6003 } } }
                }
            });
            // Expect: 6000, 6001, 6002, 6003

            // Between and NotIn
            people = repository.Query(new
            {
                QueryGroups = new[]
                {
                    new { Id = new { Operation = Operation.Between, Value = new[] { 6000, 6010 } } },
                    new { Id = new { Operation = Operation.NotIn, Value = new[] { 6000, 6001, 6002, 6003 } } }
                }
            });
            // Expect: 6004 - 6010

            // Between and NotBetween
            people = repository.Query(new
            {
                QueryGroups = new[]
                {
                    new { Id = new { Operation = Operation.Between, Value = new[] { 6000, 6010 } } },
                    new { Id = new { Operation = Operation.NotBetween, Value = new[] { 6002, 6007 } } }
                }
            });
            // Expect: 6000, 6001, 6008, 6009, 6010
        }

        private static void TestBatchQuery()
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var rowsPerBatch = 777;
            var batches = repository.CountBig() / rowsPerBatch;
            for (var page = 0; page < batches; page++)
            {
                var people = repository.BatchQuery(
                    page,
                    rowsPerBatch,
                    OrderField.Parse(new
                    {
                        Id = Order.Descending
                    }));
                Console.WriteLine($"Page: {page}, Rows: {people.Count()}, From: {people.Min(p => p.Id)}, To: {people.Max(p => p.Id)}");
            }
        }

        private static void TestInlineUpdate()
        {
            var repository = new PersonRepository(RepoDbConnectionString);
            var affectedRows = repository.InlineUpdate(new
            {
                DateUpdated = DateTime.UtcNow
            },
            new
            {
                Id = new { Operation = Operation.Between, Value = new[] { 600, 950 } },
                Name = new { Operation = Operation.Like, Value = "Na%" }
            },
            true);
        }

        private static void TestCrud()
        {
            // Repository
            var repository = new PersonRepository(RepoDbConnectionString);
            var people = (IEnumerable<Person>)null;
            var person = (Person)null;
            var personId = new Random().Next(3000, 20000);
            var affectedRows = 0;

            // Count
            affectedRows = repository.Count();

            // BatchQuery
            Console.WriteLine("BatchQuery");
            people = repository.BatchQuery(0, 1000, OrderField.Parse(new { Id = Order.Descending }));

            // Query 100K
            Console.WriteLine("Query: 100K");
            people = repository.Query(new { Id = new { Operation = Operation.GreaterThan, Value = 100 } }, top: 100000);

            // BulkInsert
            Console.WriteLine("BulkInsert: 100K");
            affectedRows = repository.BulkInsert(people);

            // Insert
            Console.WriteLine("Insert");
            person = new Person()
            {
                Name = $"Name: {Guid.NewGuid().ToString()}",
                Address = $"Address: {Guid.NewGuid().ToString()}",
                DateInserted = DateTime.UtcNow,
                DateOfBirth = DateTime.UtcNow.Date.AddYears(-32),
                DateUpdated = DateTime.UtcNow,
                Worth = 6000000
            };
            personId = Convert.ToInt32(repository.Insert(person));

            // Verify
            Console.WriteLine($"Query: {personId}");
            person = repository.Query(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Update
            Console.WriteLine($"Update: {personId}");
            person.Name = $"Name: {Guid.NewGuid().ToString()} (Updated)";
            person.Address = $"Address: {Guid.NewGuid().ToString()} (Updated)";
            person.DateUpdated = DateTime.UtcNow;
            affectedRows = repository.Update(person, new { Id = person.Id });

            // Verify
            Console.WriteLine($"Query: {personId}");
            person = repository.Query(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Merge
            Console.WriteLine($"Merge: {personId}");
            person.Name = $"{Guid.NewGuid().ToString()} (Merged)";
            person.Address = $"Address: {Guid.NewGuid().ToString()} (Merged)";
            person.DateUpdated = DateTime.UtcNow;
            affectedRows = repository.Merge(person, Field.From(new string[] { "Id" }));

            // Verify
            Console.WriteLine($"Query: {personId}");
            person = repository.Query(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // InlineUpdate
            Console.WriteLine("InlineUpdate");
            affectedRows = repository.InlineUpdate(new
            {
                Name = $"Name: {Guid.NewGuid().ToString()} (Inline Updated)"
            },
            new
            {
                Id = personId
            });

            // Verify
            Console.WriteLine($"Query: {personId}");
            person = repository.Query(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Delete
            Console.WriteLine($"Delete: {personId}");
            affectedRows = repository.Delete(new { Id = personId });

            // Verify
            Console.WriteLine($"Query: {personId}");
            person = repository.Query(personId).FirstOrDefault();
            if (person != null)
            {
                throw new NullReferenceException("Person should be null. We have just deleted it.");
            }
        }
    }
}
