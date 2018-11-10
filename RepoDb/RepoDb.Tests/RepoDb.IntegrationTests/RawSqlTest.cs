using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class RawSqlTest
    {
        [TestMethod]
        public void TestExecuteQueryWithArrayParameters()
        {
            // Prepare
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customers = new List<Customer>();
            for (var i = 1; i <= 10; i++)
            {
                customers.Add(new Customer
                {
                    Id = i,
                    GlobalId = Guid.NewGuid(),
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    MiddleName = $"MiddleName{i}",
                    Address = $"Address{i}",
                    IsActive = true,
                    Email = $"Email{i}",
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName
                });
            }
            repository.BulkInsert(customers);

            // Act
            var globalIds = customers.Select(customer => customer.GlobalId).ToArray();
            var result = repository.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE GlobalId IN (@Ids);", new { Ids = globalIds });

            // Assert
            Assert.AreEqual(customers.Count, result.ToList().Count);
            result.ToList().ForEach(customer =>
            {
                var value = customers.FirstOrDefault(c => c.GlobalId == customer.GlobalId);
                Assert.IsNotNull(value);
            });
        }

        [TestMethod]
        public void TestExecuteScalarWithArrayParameters()
        {
            // Prepare
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customers = new List<Customer>();
            for (var i = 1; i <= 10; i++)
            {
                customers.Add(new Customer
                {
                    Id = i,
                    GlobalId = Guid.NewGuid(),
                    FirstName = $"FirstName{i}",
                    LastName = $"LastName{i}",
                    MiddleName = $"MiddleName{i}",
                    Address = $"Address{i}",
                    IsActive = true,
                    Email = $"Email{i}",
                    LastUpdatedUtc = DateTime.UtcNow,
                    LastUserId = Environment.UserName
                });
            }
            repository.BulkInsert(customers);

            // Act
            var globalIds = customers.Select(customer => customer.GlobalId).ToArray();
            var result = repository.ExecuteScalar("SELECT MAX(LastUpdatedUtc) FROM [dbo].[Customer] WHERE GlobalId IN (@Ids);", new { Ids = globalIds });
            var expected = customers.Max(c => c.LastUpdatedUtc);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
