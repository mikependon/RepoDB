using System;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace RepoDb.IntegrationTests
{
    [TestFixture]
    public class BulkOpsTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }

        [TestMethod]
        public void TestBulkInsert()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var fixture = new Fixture();
            var fixtureDataList = fixture.Build<Customer>()
                .With(x => x.FirstName, "FirstName")
                .With(x => x.LastName, "dela Cruz")
                .With(x => x.MiddleName, "Pinto")
                .With(x => x.Address, "Address")
                .With(x => x.Email, "Test@Emaill.com")
                .With(x => x.LastUpdatedUtc, DateTime.UtcNow)
                .With(x => x.LastUserId, Environment.UserName)
                .CreateMany(100);

            // Act
            var affectedRows = repository.BulkInsert(fixtureDataList);
            var customers = repository.Query<Customer>(top: 10).ToList();

            // Assert
            affectedRows.ShouldBe(100);
            foreach (var customer in customers)
            {
                var fixtureData = fixtureDataList.Single(s => s.GlobalId == customer.GlobalId);
                fixtureData.ShouldNotBeNull();
                fixtureData.FirstName.ShouldBe(customer.FirstName);
                fixtureData.LastName.ShouldBe(customer.LastName);
                fixtureData.MiddleName.ShouldBe(customer.MiddleName);
                fixtureData.Address.ShouldBe(customer.Address);
                fixtureData.Email.ShouldBe(customer.Email);
                fixtureData.IsActive.ShouldBe(customer.IsActive);
                fixtureData.LastUpdatedUtc.ShouldBe(customer.LastUpdatedUtc);
                fixtureData.LastUserId.ShouldBe(customer.LastUserId);
            }
        }

        [TestMethod]
        public void TestBulkInsertWithUnorderedColumns()
        {
            // Setup
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            repository.ExecuteNonQuery(@"                
                CREATE TABLE [dbo].[TestTable](
	                [FullName] [NVARCHAR](50) NULL,
	                [Age] INT NULL,
	                [BirthDate] DATETIME NULL
                ) ");
            var fixture = new Fixture();
            var fixtureData = fixture.Build<TestTable>()
                .With(x => x.FullName, "A B C")
                .With(x => x.Age, 25)
                .With(x => x.BirthDate, new DateTime(1993, 1, 1))
                .CreateMany(10);

            // Act
            var affectedRows = repository.BulkInsert(fixtureData);

            // Assert
            affectedRows.ShouldBe(10);
        }

        public class TestTable
        {
            public int Age { get; set; }
            public DateTime BirthDate { get; set; }
            public string FullName { get; set; }
        }
    }
}
