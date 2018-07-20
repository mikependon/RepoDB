using System;
using System.Data.SqlClient;
using System.Linq;
using AutoFixture;
using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using Shouldly;

namespace RepoDb.IntegrationTests
{
    [TestFixture()]
    public class TestBulkOps : FixturePrince
    {
        [Test]
        public void TestBulkInsert()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            var fixture = new Fixture();
            var fixtureDataList = fixture.Build<Customer>()
                .With(x => x.FirstName, "Juan")
                .With(x => x.LastName, "dela Cruz")
                .With(x => x.MiddleName, "Pinto")
                .With(x => x.Address, "San Lorenzo, Makati, Philippines 4225")
                .With(x => x.Email, "juandelacruz@gmail.com")
                .With(x => x.LastUpdatedUtc, DateTime.UtcNow)
                .With(x => x.LastUserId, Environment.UserName)
                .CreateMany(100);

            //act
            var affectedRows = repository.BulkInsert(fixtureDataList);

            //assert
            affectedRows.ShouldBe(100);

            var savedDataList = repository.Query<Customer>(top:10).ToList();
            foreach (var savedData in savedDataList)
            {
                var fixtureData = fixtureDataList.Single(s => s.GlobalId == savedData.GlobalId);
                fixtureData.ShouldNotBeNull();
                fixtureData.FirstName.ShouldBe(savedData.FirstName);
                fixtureData.LastName.ShouldBe(savedData.LastName);
                fixtureData.MiddleName.ShouldBe(savedData.MiddleName);
                fixtureData.Address.ShouldBe(savedData.Address);
                fixtureData.Email.ShouldBe(savedData.Email);
                fixtureData.IsActive.ShouldBe(savedData.IsActive);
                fixtureData.LastUpdatedUtc.ShouldBe(savedData.LastUpdatedUtc);
                fixtureData.LastUserId.ShouldBe(savedData.LastUserId);
            }

        }

        [Test]
        public void TestBulkInsertWithUnorderedColumns()
        {
            //arrange
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);

            repository.ExecuteNonQuery(@"                
                CREATE TABLE [dbo].[TestTable](
	                [FullName] [NVARCHAR](50) NULL,
	                [Age] INT NULL,
	                [BirthDate] DATETIME NULL
                ) ");

            var fixture = new Fixture();
            var fixtureData = fixture.Build<TestTable>()
                .With(x => x.FullName, "Juan dela Cruz Pinto")
                .With(x => x.Age, 25)
                .With(x => x.BirthDate, new DateTime(1993, 1, 1))
                .CreateMany(10);

            //act
            var affectedRows = repository.BulkInsert(fixtureData);

            //assert
            affectedRows.ShouldBe(10);
        }

        [Map("[dbo].[TestTable]")]
        public class TestTable : DataEntity
        {
            public int Age { get; set; }
            public DateTime BirthDate { get; set; }
            public string FullName { get; set; }
        }
    }
}
