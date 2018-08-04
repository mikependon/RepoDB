using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class RecursiveQueryTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Clear the data first
            Clear();

            // Manage recursion
            RecursionManager.SetRecursiveMaximumRecursion(4);

            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Customer1
                var customerId = repository.Insert(new RecursiveCustomer
                {
                    Address = "Address",
                    DateInsertedUtc = DateTime.UtcNow,
                    Email = "customer1@email.com",
                    FirstName = "FirstName",
                    GlobalId = Guid.NewGuid(),
                    IsActive = true,
                    LastName = "LastName",
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName,
                    MiddleName = "MiddleName"
                });

                // Order1
                var orderId = repository.Insert(new RecursiveOrder
                {
                    CustomerId = Convert.ToInt64(customerId),
                    DateInsertedUtc = DateTime.UtcNow,
                    Freight = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.NewLine,
                    OrderDateUtc = DateTime.UtcNow.Date.AddDays(-15),
                    SubTotal = 1.5M,
                    Tax = 1M,
                    TotalDue = 3
                });

                // OrderDetail1
                var orderDetailId = repository.Insert(new RecursiveOrderDetail
                {
                    DateInsertedUtc = DateTime.UtcNow,
                    Discount = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName,
                    LineTotal = 1.5M,
                    OrderId = Convert.ToInt32(orderId),
                    ProductCode = "MILK",
                    ProductName = "NESTLE",
                    Quantity = 3,
                    UnitPrice = 30
                });

                // OrderDetail2
                orderDetailId = repository.Insert(new RecursiveOrderDetail
                {
                    DateInsertedUtc = DateTime.UtcNow,
                    Discount = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName,
                    LineTotal = 1.5M,
                    OrderId = Convert.ToInt32(orderId),
                    ProductCode = "CHEESE",
                    ProductName = "NESTLE",
                    Quantity = 3,
                    UnitPrice = 30
                });

                // Order2
                orderId = repository.Insert(new RecursiveOrder
                {
                    CustomerId = Convert.ToInt64(customerId),
                    DateInsertedUtc = DateTime.UtcNow,
                    Freight = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.NewLine,
                    OrderDateUtc = DateTime.UtcNow.Date.AddDays(-15),
                    SubTotal = 1.5M,
                    Tax = 1M,
                    TotalDue = 3
                });

                // OrderDetail3
                orderDetailId = repository.Insert(new RecursiveOrderDetail
                {
                    DateInsertedUtc = DateTime.UtcNow,
                    Discount = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName,
                    LineTotal = 1.5M,
                    OrderId = Convert.ToInt32(orderId),
                    ProductCode = "COFFEE",
                    ProductName = "NESTLE",
                    Quantity = 3,
                    UnitPrice = 30
                });

                // OrderDetail4
                orderDetailId = repository.Insert(new RecursiveOrderDetail
                {
                    DateInsertedUtc = DateTime.UtcNow,
                    Discount = 1.5M,
                    GlobalId = Guid.NewGuid(),
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName,
                    LineTotal = 1.5M,
                    OrderId = Convert.ToInt32(orderId),
                    ProductCode = "CONDIMENTS",
                    ProductName = "NESTLE",
                    Quantity = 3,
                    UnitPrice = 30
                });
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Clear();
        }

        private static void Clear()
        {
            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                repository.DeleteAll<RecursiveOrderDetail>();
                repository.DeleteAll<RecursiveOrder>();
                repository.DeleteAll<RecursiveCustomer>();
            }
        }

        [TestMethod]
        public void TestWithRecursive()
        {
            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act
                var actual = repository.Query<RecursiveCustomer>(new { Email = "customer1@email.com" }, recursive: true).First();

                // Assert
                Assert.AreEqual(2, actual.Orders.Count());
                actual
                    .Orders
                    .ToList().ForEach(order =>
                    {
                    // Assert Parent RecursiveCustomer
                    Assert.AreEqual(1, order.Customers.Count());

                    // Assert Child OrderDetails
                    Assert.AreEqual(2, order.OrderDetails.Count());

                    // Assert Child OrderDetails
                    order.OrderDetails.ToList().ForEach(orderDetail =>
                        {

                        // Assert Parent RecursiveOrder
                        Assert.AreEqual(1, orderDetail.Orders.Count());

                        });
                    });
            }
        }

        [TestMethod]
        public void TestWithRecursionDepthAsZero()
        {
            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act
                var actual = repository.Query<RecursiveCustomer>(new { Email = "customer1@email.com" },
                recursive: true,
                recursionDepth: 0).First();

                // Assert
                Assert.IsNull(actual.Orders);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnPassingRecursionDepthAsNegative()
        {
            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act/Assert
                repository.Query<RecursiveCustomer>(new { Email = "customer1@email.com" },
                recursive: true,
                recursionDepth: -1);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnPassingRecursionDepthGreaterThanTheSettings()
        {
            using (var repository = new DbRepository<SqlConnection>(Constants.TestDatabase))
            {
                // Act/Assert
                repository.Query<RecursiveCustomer>(new { Email = "customer1@email.com" },
                recursive: true,
                recursionDepth: 100);
            }
        }
    }
}
