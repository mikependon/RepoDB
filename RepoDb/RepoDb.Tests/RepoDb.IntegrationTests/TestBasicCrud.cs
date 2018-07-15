using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using NUnit.Framework;
using RepoDb;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using Shouldly;

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class TestBasicCrud
    {
        [SetUp]
        public void Setup()
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
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
                //DateInsertedUtc = DateTime.UtcNow,
                DateInsertedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            //act
            var id = repository.Insert(fixtureData);

            //assert
            var customer = repository.Query<Customer>(id).FirstOrDefault();

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
            //customer.DateInsertedUtc.ShouldBe(fixtureData.DateInsertedUtc);
            //customer.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
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
                DateInsertedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            //act
            repository.Update(fixtureData, new { fixtureData.Id });

            //assert
            var savedData = repository.Query<Customer>(fixtureData.Id).FirstOrDefault();
            savedData.ShouldNotBeNull();
            savedData.Id.ShouldNotBe(0);
            savedData.GlobalId.ShouldBe(fixtureData.GlobalId);
            savedData.FirstName.ShouldBe(fixtureData.FirstName);
            savedData.LastName.ShouldBe(fixtureData.LastName);
            savedData.MiddleName.ShouldBe(fixtureData.MiddleName);
            savedData.Address.ShouldBe(fixtureData.Address);
            savedData.Email.ShouldBe(fixtureData.Email);
            savedData.IsActive.ShouldBe(fixtureData.IsActive);
            //savedData.DateInsertedUtc.ShouldBe(fixtureData.DateInsertedUtc);
            //savedData.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            savedData.LastUserId.ShouldBe(fixtureData.LastUserId);
        }
        
        [Test]
        public void TestDelete()
        {
            //arrange

            //act
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var rowsAffected = repository.Delete<Customer>(new { Id = 1 });

            //assert
            rowsAffected.ShouldBe(1);
        }

        [Test]
        public void TestMergeExpectInsert()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            var fixtureData = new Customer
            {
                GlobalId = Guid.NewGuid(),
                FirstName = "Juan-MERGED",
                LastName = "de la Cruz-MERGED",
                MiddleName = "Pinto-MERGED",
                Address = "San Lorenzo, Makati, Philippines 4225-MERGED",
                IsActive = true,
                Email = "juandelacruz@gmai.com-MERGED",
                DateInsertedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                LastUserId = Environment.UserName
            };

            //act
            repository.Merge(fixtureData);

            //assert
            var customer = repository.Query<Customer>(new { GlobalId = fixtureData.GlobalId }).FirstOrDefault();
            customer.ShouldNotBeNull();
            customer.Id.ShouldNotBe(0);
            customer.GlobalId.ShouldBe(fixtureData.GlobalId);
            customer.FirstName.ShouldBe(fixtureData.FirstName);
            customer.LastName.ShouldBe(fixtureData.LastName);
            customer.MiddleName.ShouldBe(fixtureData.MiddleName);
            customer.Address.ShouldBe(fixtureData.Address);
            customer.Email.ShouldBe(fixtureData.Email);
            customer.IsActive.ShouldBe(fixtureData.IsActive);
            //customer.DateInsertedUtc.ShouldBe(fixtureData.DateInsertedUtc);
            //customer.LastUpdatedUtc.ShouldBe(fixtureData.LastUpdatedUtc);
            customer.LastUserId.ShouldBe(fixtureData.LastUserId);
        }

        [Test]
        public void TestMergeExpectDelete()
        {
            //arrange

            //act
            throw new NotImplementedException();

            //assert            
        }

        [Test]
        public void TestMergeExpectUpdate()
        {
            //arrange

            //act
            throw new NotImplementedException();

            //assert            
        }

        [TearDown]
        public void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }
    }
}