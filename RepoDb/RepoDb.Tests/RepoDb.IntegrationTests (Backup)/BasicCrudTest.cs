using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.IntegrationTests.Extensions;
using Shouldly;

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class BasicCrudTest : FixturePrince
    {
        [SetUp]
        public void SetupCrudTables()
        {
            SetupHelper.ExecuteEmbeddedSqlFile("RepoDb.IntegrationTests.Setup.RefreshDB.sql");
        }

        [Test]
        public void TestInsert()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            var fixtureData = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "Juan",
                LastName = "de la Cruz",
                MiddleName = "Pinto",
                Address = "San Lorenzo, Makati, Philippines 4225",
                IsActive = true,
                Email = "juandelacruz@gmai.com",
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
        }

        [Test]
        public void TestUpdate()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixtureData = new Customer
            {
                Id = 1,
                FirstName = "Juan-EDITED",
                LastName = "de la Cruz-EDITED",
                MiddleName = "Pinto-EDITED",
                Address = "San Lorenzo, Makati, Philippines 4225-EDITED",
                IsActive = true,
                Email = "juandelacruz@gmai.com-EDITED",
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

        [Test]
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

        [Test]
        public void TestMergeInsert()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            var fixtureData = new Customer
            {
                Id = 99,
                GlobalId = Guid.NewGuid(),
                FirstName = "Juan-MERGED",
                LastName = "de la Cruz-MERGED",
                MiddleName = "Pinto-MERGED",
                Address = "San Lorenzo, Makati, Philippines 4225-MERGED",
                IsActive = true,
                Email = "juandelacruz@gmai.com-MERGED",
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
        
        [Test]
        public void TestMergeUpdate()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixtureData = new Customer
            {
                Id = 1,
                FirstName = "Juan-MERGED",
                LastName = "de la Cruz-MERGED",
                MiddleName = "Pinto-MERGED",
                Address = "San Lorenzo, Makati, Philippines 4225-MERGED",
                IsActive = true,
                Email = "juandelacruz@gmai.com-MERGED",
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = $"{Environment.UserName}-MERGED"
            };

            //act
            repository.Merge(fixtureData);

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
    }
}