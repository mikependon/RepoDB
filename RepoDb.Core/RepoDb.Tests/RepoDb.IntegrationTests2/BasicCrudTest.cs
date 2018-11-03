using System;
using System.Data.SqlClient;
using System.Linq;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.IntegrationTests.Extensions;
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

        [TestMethod]
        public void TestInsert()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            var fixtureData = new Customer
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

            //act
            var returnedId = repository.Insert(fixtureData);

            //assert
            var customer = repository.Query<Customer>(new { Id = returnedId }).FirstOrDefault();

            //assert
            customer.ShouldNotBeNull();
            customer.Id.ShouldNotBe(0);
            customer.GlobalId.ShouldBe(fixtureData.GlobalId);
            customer.FirstName.ShouldBe(fixtureData.FirstName);
            customer.LastName.ShouldBe(fixtureData.LastName);
            customer.MiddleName.ShouldBe(fixtureData.MiddleName);
            customer.Address.ShouldBe(fixtureData.Address);
            customer.Email.ShouldBe(fixtureData.Email);
            customer.IsActive.ShouldBe(fixtureData.IsActive);
            fixtureData.LastUpdatedUtc.ShouldBeEx(customer.LastUpdatedUtc);
            customer.LastUserId.ShouldBe(fixtureData.LastUserId);

            //assert - raw - this will test the cached type Customer whether the mapping
            //is not affected after calling the Query method.
            customer = repository.ExecuteQuery<Customer>("SELECT FirstName, LastName, Address, Email FROM [dbo].[Customer] WHERE Id = @Id;", new { Id = returnedId }).FirstOrDefault();

            //assert - raw
            customer.ShouldNotBeNull();
            customer.FirstName.ShouldBe(fixtureData.FirstName);
            customer.LastName.ShouldBe(fixtureData.LastName);
            customer.Address.ShouldBe(fixtureData.Address);
            customer.Email.ShouldBe(fixtureData.Email);
        }

        [TestMethod]
        public void TestUpdate()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixtureData = new Customer
            {
                Id = 1,
                FirstName = "FirstName-EDITED",
                LastName = "LastName-EDITED",
                MiddleName = "MiddleName-EDITED",
                Address = "Address-EDITED",
                IsActive = true,
                Email = "Test@Email.com-EDITED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            //act
            repository.Update(fixtureData, new { fixtureData.Id });

            //assert
            var savedData = repository.Query<Customer>(new { fixtureData.Id }).FirstOrDefault();
            savedData.ShouldNotBeNull();
            savedData.Id.ShouldNotBe(0);
            savedData.GlobalId.ShouldBe(fixtureData.GlobalId);
            savedData.FirstName.ShouldBe(fixtureData.FirstName);
            savedData.LastName.ShouldBe(fixtureData.LastName);
            savedData.MiddleName.ShouldBe(fixtureData.MiddleName);
            savedData.Address.ShouldBe(fixtureData.Address);
            savedData.Email.ShouldBe(fixtureData.Email);
            savedData.IsActive.ShouldBe(fixtureData.IsActive);
            savedData.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            savedData.LastUserId.ShouldBe(fixtureData.LastUserId);
        }

        [TestMethod]
        public void TestDelete()
        {
            //arrange and act
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var rowsAffected = repository.Delete<Customer>(new { Id = 1 });

            //assert
            rowsAffected.ShouldBe(1);
            var savedData = repository.Query<Customer>(new { Id = 1 }).FirstOrDefault();
            savedData.ShouldBeNull();
        }

        [TestMethod]
        public void TestMergeInsert()
        {
            //arrange
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

            //act
            repository.Merge(fixtureData);

            //assert
            var customer = repository.Query<Customer>(new { fixtureData.GlobalId }).FirstOrDefault();
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
        public void TestMergeUpdate()
        {
            //arrange
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

            //act
            repository.Merge(fixtureData, Field.From("GlobalId"));

            //assert
            var savedData = repository.Query<Customer>(new { fixtureData.GlobalId }).FirstOrDefault();
            savedData.ShouldNotBeNull();
            savedData.GlobalId.ShouldBe(fixtureData.GlobalId);
            savedData.FirstName.ShouldBe(fixtureData.FirstName);
            savedData.LastName.ShouldBe(fixtureData.LastName);
            savedData.MiddleName.ShouldBe(fixtureData.MiddleName);
            savedData.Address.ShouldBe(fixtureData.Address);
            savedData.Email.ShouldBe(fixtureData.Email);
            savedData.IsActive.ShouldBe(fixtureData.IsActive);
            savedData.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            savedData.LastUserId.ShouldBe(fixtureData.LastUserId);
        }
    }
}