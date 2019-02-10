using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class QueryCacheTest
    {
        [TestMethod]
        public void TestQueryWithCacheKey()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var globalId = Guid.NewGuid();
            var cacheKey = globalId.ToString();

            // Act - Query
            var customerId = repository.Insert(new Customer
            {
                Address = "Address",
                DateInsertedUtc = DateTime.UtcNow,
                Email = "customer1@email.com",
                FirstName = "FirstName",
                GlobalId = globalId,
                IsActive = true,
                LastName = "LastName",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName,
                MiddleName = "MiddleName"
            });
            var actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();

            // Assert - Query
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, repository.Cache.Count());

            // Act - Delete
            repository.Delete<Customer>(c => c.GlobalId == globalId);
            actual = repository.Query<Customer>(c => c.GlobalId == globalId).FirstOrDefault();

            // Assert - Delete
            Assert.IsNull(actual);

            // Act - QueryWithCacheKey
            actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();

            // Asert - Query
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void TestQueryWithCacheKeyAndManuallyClearingACache()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var globalId = Guid.NewGuid();
            var cacheKey = globalId.ToString();

            // Act - Query
            var customerId = repository.Insert(new Customer
            {
                Address = "Address",
                DateInsertedUtc = DateTime.UtcNow,
                Email = "customer1@email.com",
                FirstName = "FirstName",
                GlobalId = globalId,
                IsActive = true,
                LastName = "LastName",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName,
                MiddleName = "MiddleName"
            });
            var actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();

            // Assert - Query
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, repository.Cache.Count());

            // Act - Delete
            repository.Delete<Customer>(c => c.GlobalId == globalId);
            actual = repository.Query<Customer>(c => c.GlobalId == globalId).FirstOrDefault();

            // Assert - Delete
            Assert.IsNull(actual);

            // Act - Clear
            repository.Cache.Clear();

            // Act - QueryWithCacheKey
            actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();
            Assert.AreEqual(0, repository.Cache.Count());

            // Asert - Query
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestQueryWithCacheKeyAndManuallyRemovingACache()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var globalId = Guid.NewGuid();
            var cacheKey = globalId.ToString();

            // Act - Query
            var customerId = repository.Insert(new Customer
            {
                Address = "Address",
                DateInsertedUtc = DateTime.UtcNow,
                Email = "customer1@email.com",
                FirstName = "FirstName",
                GlobalId = globalId,
                IsActive = true,
                LastName = "LastName",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName,
                MiddleName = "MiddleName"
            });
            var actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();

            // Assert - Query
            Assert.IsNotNull(actual);
            Assert.AreEqual(1, repository.Cache.Count());

            // Act - Delete
            repository.Delete<Customer>(c => c.GlobalId == globalId);
            actual = repository.Query<Customer>(c => c.GlobalId == globalId).FirstOrDefault();

            // Assert - Delete
            Assert.IsNull(actual);

            // Act - Clear
            repository.Cache.Remove(cacheKey);

            // Act - QueryWithCacheKey
            actual = repository.Query<Customer>(c => c.GlobalId == globalId, cacheKey: cacheKey).FirstOrDefault();
            Assert.AreEqual(0, repository.Cache.Count());

            // Asert - Query
            Assert.IsNull(actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnRemovingNonExistingCache()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            // Act/Assert
            repository.Cache.Remove("NonExistingKey");
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnAddingMultipleCacheKey()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            // Act/Assert
            repository.Cache.Add("Key", "Value");
            repository.Cache.Add("Key", "Value");
        }
    }
}
