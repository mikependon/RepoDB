using System;
using System.Data.SqlClient;
using System.Linq;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.IntegrationTests.Extensions;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using RepoDb.Attributes;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class MillisecondsTest
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

        [Map("TypeMap")]
        private class TestDateTimeClass
        {
            public Guid SessionId { get; set; }
            public DateTime date_column { get; set; }
            public DateTime datetime_column { get; set; }
            public DateTime datetime2_column { get; set; }
        }

        [TestMethod]
        public void TestDateTime()
        {
            //arrange
            var sessionId = Guid.NewGuid();
            var date = DateTime.Parse("2018-01-01");
            var dateTime = DateTime.Parse("2018-01-01 13:10:12.000");
            var dateTime2 = DateTime.Parse("2018-01-01 01:10:00.1234567");

            //act
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            var returnedId = repository.Insert(new TestDateTimeClass
            {
                SessionId = sessionId,
                date_column = date,
                datetime_column = dateTime,
                datetime2_column = dateTime2
            });

            //assert
            var actual = repository.Query<TestDateTimeClass>(new { SessionId = sessionId }).FirstOrDefault();
            actual.ShouldNotBeNull();
            actual.date_column.ShouldBe(date.Date);
            actual.datetime_column.ShouldBe(dateTime);
            actual.datetime2_column.ShouldBe(dateTime2);

            // Dispose
            repository.Dispose();
        }

        [Map("TypeMap")]
        private class TestDateTimeNullableClass
        {
            public Guid SessionId { get; set; }
            public DateTime? date_column { get; set; }
            public DateTime? datetime_column { get; set; }
            public DateTime? datetime2_column { get; set; }
        }

        [TestMethod]
        public void TestDateTimeNullable()
        {
            //arrange
            var sessionId = Guid.NewGuid();
            var date = DateTime.Parse("2018-01-01");
            var dateTime = DateTime.Parse("2018-01-01 13:10:12.000");
            var dateTime2 = DateTime.Parse("2018-01-01 01:10:00.1234567");

            //act
            var repository = new DbRepository<SqlConnection>(Constants.TestDatabase);
            repository.Insert(new TestDateTimeNullableClass
            {
                SessionId = sessionId,
                date_column = date,
                datetime_column = dateTime,
                datetime2_column = dateTime2
            });

            //assert
            var actual = repository.Query<TestDateTimeClass>(new { SessionId = sessionId }).FirstOrDefault();
            actual.ShouldNotBeNull();
            actual.date_column.ShouldBe(date.Date);
            actual.datetime_column.ShouldBe(dateTime);
            actual.datetime2_column.ShouldBe(dateTime2);

            // Dispose
            repository.Dispose();
        }
    }
}