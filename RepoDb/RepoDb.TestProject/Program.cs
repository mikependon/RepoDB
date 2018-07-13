using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using RepoDb.Enumerations;
using RepoDb.TestProject.Models;
using RepoDb.TestProject.Repositories;

namespace RepoDb.TestProject
{
    class Program
    {
        private static readonly string RepoDbConnectionString = @"Server=.;Database=RepoDb;Integrated Security=True;";
        private static readonly string InventoryConnectionString = "Initial Catalog=.;Database=Northwind;Integrated Security=True;";

        static void Main(string[] args)
        {
            //TestInventory();
            TestAllOperations();
            //TestInNotInBetweenNotBetweenAnyAllOperation();
            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public static void TestInventory()
        {
            using (var repository = new DbRepository<SqlConnection>(InventoryConnectionString, ConnectionPersistency.Instance))
            {
                var current = DateTime.UtcNow;
                var orders = repository.Query<CustomerDto>(recursive: true, recursionDepth: 10);
                var lapsedInSeconds = (DateTime.UtcNow - current).TotalSeconds;
                Console.WriteLine($"Recursive query lapsed for total of {lapsedInSeconds} second(s).");
                //var customers = repository.Query<CustomerDto>();
                //customers.ToList().ForEach(customer =>
                //{
                //    //var rows = repository.InlineUpdate<CustomerDto>(new { customer.FirstName }, new { customer.Id }, true);
                //    //rows = Convert.ToInt32(repository.Insert(customer));
                //    //rows = repository.Update(customer);
                //    //rows = repository.Merge(customer, Field.Parse(new { customer.Id }));

                //    //// Customer
                //    Console.WriteLine($"Customer: {customer.FirstName} {customer.LastName} from {customer.City}, {customer.Country}");
                //    // Orders
                //    var orders = repository.Query<OrderDto>(new { CustomerId = customer.Id });
                //    orders.ToList().ForEach(order =>
                //    {
                //        Console.WriteLine($"   Order: {order.OrderNumber}, " +
                //            $"Date: {order.OrderDate.GetValueOrDefault().ToString("u")}, Total Amount: {order.TotalAmount}");
                //        // OrderItem
                //        var orderItem = repository.Query<OrderItemDto>(new { OrderId = order.Id }).FirstOrDefault();
                //        if (orderItem != null)
                //        {
                //            // Product
                //            var product = repository.Query<ProductDto>(new { Id = orderItem.ProductId }).FirstOrDefault();
                //            if (product != null)
                //            {
                //                Console.WriteLine($"      Product: {product.ProductName}, Price: {orderItem.UnitPrice}, Quantity: {orderItem.Quantity}, Total: {orderItem.UnitPrice * orderItem.Quantity} ");
                //                // Supplier
                //                var supplier = repository.Query<SupplierDto>(new { Id = product.SupplierId }).FirstOrDefault();
                //                if (supplier != null)
                //                {
                //                    Console.WriteLine($"      Supplier: {supplier.CompanyName}, City: {supplier.City}, Country: {supplier.Country}, Contact: {supplier.ContactName}, Phone: {supplier.Phone} ");
                //                }
                //            }
                //        }
                //    });
                //    //Console.ReadLine();
                //});
            }
            Console.ReadLine();
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
                        new { Operation = Operation.NotIn, Value = new int[] { 5002, 5003, 5009 } },
                        new { Operation = Operation.Between, Value = new object[] { 5001, 5010 } },
                        new { Operation = Operation.NotBetween, Value = new int[] { 5006, 5010 } }
                    }
                }
            });
            // Expect: 5001, 5004, 5005

            // In
            people = repository.Query(new
            {
                Id = new
                {
                    Operation = Operation.In,
                    Value = new[] { 6000, 6001, 6002, 6003, 6004, 6005 }
                }
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
            var batches = repository.Count() / rowsPerBatch;
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

        private static void TestAllOperations()
        {
            // Repository
            var repository = new DbRepository<SqlConnection>(RepoDbConnectionString, ConnectionPersistency.Instance);

            // Truncate
            repository.Truncate<Animal>();

            // Count
            Console.WriteLine($"Counting Person Records: {repository.Count<Person>()}");
            Console.WriteLine($"Counting Animal Records: {repository.Count<Animal>()}");

            // BatchQuery
            Console.WriteLine("BatchQuery Person");
            var batchQueryResult = repository.BatchQuery<Person>(0, 1000, OrderField.Parse(new { Id = Order.Descending }));

            // Query 100K
            Console.WriteLine("Query Person: 100K");
            var queryResult = repository.Query<Person>(new
            {
                Id = new
                {
                    Operation = Operation.GreaterThan,
                    Value = 100
                }
            }, top: 100000);

            // BulkInsert
            Console.WriteLine("BulkInsert Person: 100K");
            var bulkInsertResult = repository.BulkInsert(queryResult);

            // Insert with Guid Primary Key
            Console.WriteLine("Insert with Guid PrimaryKey");
            var animalId = repository.Insert(new Animal()
            {
                Id = Guid.NewGuid(),
                Name = $"Name: {Guid.NewGuid().ToString()}",
                Address = $"Address: {Guid.NewGuid().ToString()}",
                DateInserted = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow
            });

            // Verify
            Console.WriteLine($"Verify Insert with Guid PrimaryKey: {animalId}");
            var animal = repository.Query<Animal>(animalId).FirstOrDefault();
            if (animal == null)
            {
                throw new NullReferenceException("Animal is null.");
            }

            // Insert with Identity PrimaryKey
            Console.WriteLine("Insert with Identity PrimaryKey");
            var personId = repository.Insert(new Person()
            {
                Name = $"Name: {Guid.NewGuid().ToString()}",
                Address = $"Address: {Guid.NewGuid().ToString()}",
                DateInserted = DateTime.UtcNow,
                DateOfBirth = DateTime.UtcNow.Date.AddYears(-32),
                DateUpdated = DateTime.UtcNow,
                Worth = new Random().Next(30000, 60000)
            });

            // Verify
            Console.WriteLine($"Verify Insert with Identity PrimaryKey: {personId}");
            var person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Check InlineInsert with Guid
            Console.WriteLine($"InlineInsert Animal");
            animalId = repository.InlineInsert<Animal>(new
            {
                Id = Guid.NewGuid(),
                Name = $"NAME-{Guid.NewGuid().ToString()} - InlineInsert",
                Address = $"ADDR-{Guid.NewGuid().ToString()} - InlineInsert"
            });

            // Verify
            Console.WriteLine($"Verify InlineInsert with Guid PrimaryKey: {animalId}");
            animal = repository.Query<Animal>(animalId).FirstOrDefault();
            if (animal == null)
            {
                throw new NullReferenceException("Animal is null.");
            }

            // Check InlineInsert with Identity
            Console.WriteLine($"InlineInsert with Identity PrimaryKey");
            personId = repository.InlineInsert<Person>(new
            {
                Name = $"NAME-{Guid.NewGuid().ToString()} - InlineInsert",
                Address = $"ADDR-{Guid.NewGuid().ToString()} - InlineInsert"
            });

            // Verify
            Console.WriteLine($"Verify Insert with Identity PrimaryKey: {personId}");
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // InlineUpdate
            Console.WriteLine("InlineUpdate with Guid PrimaryKey");
            var affectedRows = repository.InlineUpdate<Animal>(new
            {
                Name = $"NAME-{Guid.NewGuid().ToString()} - InlineUpdate",
                Address = $"ADDR-{Guid.NewGuid().ToString()} - InlineUpdate"
            },
            new
            {
                Id = animalId
            });

            // Verify
            Console.WriteLine($"Verify InlineUpdate with Guid PrimaryKey: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the inline update.");
            }
            animal = repository.Query<Animal>(animalId).FirstOrDefault();
            if (animal == null)
            {
                throw new NullReferenceException("Animal is null.");
            }

            // InlineUpdate
            Console.WriteLine("InlineUpdate with Identity PrimaryKey");
            affectedRows = repository.InlineUpdate<Person>(new
            {
                Name = $"NAME-{Guid.NewGuid().ToString()} - InlineUpdate",
                Address = $"ADDR-{Guid.NewGuid().ToString()} - InlineUpdate"
            },
            new
            {
                Id = personId
            });

            // Verify
            Console.WriteLine($"Verify InlineUpdate with Identity PrimaryKey: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the inline update.");
            }
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Check InlineMerge
            Console.WriteLine($"InlineMerge with Guid PrimaryKey: {animalId}");
            affectedRows = repository.InlineMerge<Animal>(new
            {
                Id = animalId,
                Name = $"{animal.Name} - InlineMerge",
                Address = $"{animal.Name} - InlineMerge"
            });

            // Verify
            Console.WriteLine($"Verify InlineMerge with Guid PrimaryKey: {animalId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the inline merge.");
            }
            animal = repository.Query<Animal>(animalId).FirstOrDefault();
            if (animal == null)
            {
                throw new NullReferenceException("Animal is null.");
            }

            // Check InlineMerge
            Console.WriteLine($"InlineMerge with Identity PrimaryKey: {personId}");
            affectedRows = repository.InlineMerge<Person>(new
            {
                Id = personId,
                Name = $"{person.Name} - InlineMerge",
                Address = $"{person.Name} - InlineMerge"
            });

            // Verify
            Console.WriteLine($"Verify InlineMerge with Identity PrimaryKey: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the inline merge.");
            }
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Update
            Console.WriteLine($"Update Person: {personId}");
            person.Name = $"Name: {Guid.NewGuid().ToString()} (Updated)";
            person.Address = $"Address: {Guid.NewGuid().ToString()} (Updated)";
            person.DateUpdated = DateTime.UtcNow;
            person.DateOfBirth = DateTime.UtcNow;
            affectedRows = repository.Update(person);

            // Verify
            Console.WriteLine($"Verify Person after Update: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the update.");
            }
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Merge
            Console.WriteLine($"Merge: {personId}");
            person.Name = $"{Guid.NewGuid().ToString()} (Merged)";
            person.Address = $"Address: {Guid.NewGuid().ToString()} (Merged)";
            person.DateUpdated = DateTime.UtcNow;
            affectedRows = repository.Merge(person, Field.Parse(new { person.Id }));

            // Verify
            Console.WriteLine($"Query Person After Merge: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the merge.");
            }
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person == null)
            {
                throw new NullReferenceException("Person is null.");
            }

            // Delete
            Console.WriteLine($"Delete Person: {personId}");
            affectedRows = repository.Delete<Person>(personId);

            // Verify
            Console.WriteLine($"Verify Person After Delete: {personId}");
            if (affectedRows <= 0)
            {
                throw new Exception("No rows has been affected by the delete.");
            }
            person = repository.Query<Person>(personId).FirstOrDefault();
            if (person != null)
            {
                throw new NullReferenceException("Person should be null. We have just deleted it.");
            }

            // Count
            Console.WriteLine($"Person Records: {repository.Count<Person>()}");
            Console.WriteLine($"Animal Records: {repository.Count<Animal>()}");

            // Dispose
            repository.Dispose();
        }
    }
}
