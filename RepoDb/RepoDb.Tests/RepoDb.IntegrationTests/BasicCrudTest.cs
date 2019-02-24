using System;
using System.Data.SqlClient;
using System.Linq;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class BasicCrudTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
            SetupHelper.ExecuteEmbeddedSqlFile("RepoDb.IntegrationTests.Setup.RefreshDB.sql");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            SetupHelper.CleanDatabase();
        }

        // Dynamics

        [TestMethod]
        public void TestCrudViaDynamics()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customer = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                Address = "Address",
                IsActive = true,
                Email = "Test@Email.com",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act (Insert)
            repository.DeleteAll<Customer>();
            var customerId = repository.Insert(customer);

            var result = repository.Query<Customer>(customerId).FirstOrDefault();

            // Assert (Insert)
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.GlobalId.ShouldBe(customer.GlobalId);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);

            // Setup (Update)
            customer = new Customer
            {
                FirstName = "FirstName-EDITED",
                LastName = "LastName-EDITED",
                MiddleName = "MiddleName-EDITED",
                Address = "Address-EDITED",
                IsActive = true,
                Email = "Test@Email.com-EDITED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act (Update)
            repository.Update(customer, customerId);
            result = repository.Query<Customer>(customerId).FirstOrDefault();

            // Assert (Update)
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);

            // Act (Delete)
            repository.Delete<Customer>(customerId);

            // Assert (Delete)
            result = repository.Query<Customer>(customerId).FirstOrDefault();
            result.ShouldBeNull();
        }

        [TestMethod]
        public void TestMergeInsertAndQuery()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customer = new Customer
            {
                Id = 99, // 99 - will be updated if present in the DB, orelse, a new Id will be generated
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName-MERGED",
                LastName = "LastName-MERGED",
                MiddleName = "MiddleName-MERGED",
                Address = "Address-MERGED",
                IsActive = true,
                Email = "Test@Email.com-MERGED",
                DateInsertedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act
            repository.Merge(customer);
            var result = repository.Query<Customer>(c => c.GlobalId == customer.GlobalId).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.GlobalId.ShouldBe(customer.GlobalId);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);
        }

        [TestMethod]
        public void TestMergeUpdateAndQuery()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customer = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName-MERGED",
                LastName = "LastName-MERGED",
                MiddleName = "Pinto-MERGED",
                Address = "Address-MERGED",
                IsActive = true,
                Email = "Test@Email.com-MERGED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = $"{Environment.UserName}-MERGED"
            };

            // Act
            repository.Merge(customer, Field.From(nameof(Customer.GlobalId)));
            var result = repository.Query<Customer>(c => c.GlobalId == customer.GlobalId).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.GlobalId.ShouldBe(customer.GlobalId);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);
        }

        // Expressions

        [TestMethod]
        public void TestCrudViaExpressions()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var customer = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName",
                LastName = "LastName",
                MiddleName = "MiddleName",
                Address = "Address",
                IsActive = true,
                Email = "Test@Email.com",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act (Insert)
            var customerId = (long)repository.Insert(customer);
            var result = repository.Query<Customer>(c => c.Id == customerId).FirstOrDefault();

            // Assert (Insert)
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.GlobalId.ShouldBe(customer.GlobalId);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);

            // Setup (Update)
            customer = new Customer
            {
                FirstName = "FirstName-EDITED",
                LastName = "LastName-EDITED",
                MiddleName = "MiddleName-EDITED",
                Address = "Address-EDITED",
                IsActive = true,
                Email = "Test@Email.com-EDITED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act (Update)
            repository.Update(customer, c => c.Id == customerId);
            result = repository.Query<Customer>(c => c.Id == customerId).FirstOrDefault();

            // Assert (Update)
            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.FirstName.ShouldBe(customer.FirstName);
            result.LastName.ShouldBe(customer.LastName);
            result.MiddleName.ShouldBe(customer.MiddleName);
            result.Address.ShouldBe(customer.Address);
            result.Email.ShouldBe(customer.Email);
            result.IsActive.ShouldBe(customer.IsActive);
            result.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
            result.LastUserId.ShouldBe(customer.LastUserId);

            // Act (Delete)
            repository.Delete<Customer>(c => c.Id == customerId);

            // Assert (Delete)
            result = repository.Query<Customer>(c => c.Id == customerId).FirstOrDefault();
            result.ShouldBeNull();
        }

        [TestMethod]
        public void TestMergeInsertAndQueryViaExpression()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixtureData = new Customer
            {
                Id = 99,
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName-MERGED",
                LastName = "LastName-MERGED",
                MiddleName = "MiddleName-MERGED",
                Address = "Address-MERGED",
                IsActive = true,
                Email = "Test@Email.com-MERGED",
                DateInsertedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            // Act
            repository.Merge(fixtureData);
            var customer = repository.Query<Customer>(c => c.GlobalId == fixtureData.GlobalId).FirstOrDefault();

            // Assert
            customer.ShouldNotBeNull();
            customer.Id.ShouldNotBe(0);
            customer.GlobalId.ShouldBe(fixtureData.GlobalId);
            customer.FirstName.ShouldBe(fixtureData.FirstName);
            customer.LastName.ShouldBe(fixtureData.LastName);
            customer.MiddleName.ShouldBe(fixtureData.MiddleName);
            customer.Address.ShouldBe(fixtureData.Address);
            customer.Email.ShouldBe(fixtureData.Email);
            customer.IsActive.ShouldBe(fixtureData.IsActive);
            customer.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            customer.LastUserId.ShouldBe(fixtureData.LastUserId);
        }

        [TestMethod]
        public void TestMergeUpdateAndQueryViaExpression()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixtureData = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "FirstName-MERGED",
                LastName = "LastName-MERGED",
                MiddleName = "Pinto-MERGED",
                Address = "Address-MERGED",
                IsActive = true,
                Email = "Test@Email.com-MERGED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = $"{Environment.UserName}-MERGED"
            };

            // Act
            repository.Merge(fixtureData, Field.From(nameof(Customer.GlobalId)));
            var result = repository.Query<Customer>(c => c.GlobalId == fixtureData.GlobalId).FirstOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.GlobalId.ShouldBe(fixtureData.GlobalId);
            result.FirstName.ShouldBe(fixtureData.FirstName);
            result.LastName.ShouldBe(fixtureData.LastName);
            result.MiddleName.ShouldBe(fixtureData.MiddleName);
            result.Address.ShouldBe(fixtureData.Address);
            result.Email.ShouldBe(fixtureData.Email);
            result.IsActive.ShouldBe(fixtureData.IsActive);
            result.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            result.LastUserId.ShouldBe(fixtureData.LastUserId);
        }
    }
}