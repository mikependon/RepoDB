using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter.ClickHouse;
using RepoDb.Extensions;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RepoDb.ClickHouse.IntegrationTests
{
    [TestClass]
    public class AttributeTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Database.Cleanup();
        }

        #region Classes

        [Table("CompleteTable")]
        public class AttributeTable
        {
            public long Id { get; set; }

            [ClickHouseType("String")]
            public string ColumnBlob { get; set; }

            [ClickHouseType("Int64")]
            public long ColumnBigint { get; set; }

            [ClickHouseType("DateTime64(5)")]
            public DateTime ColumnDateTime2 { get; set; }

            [ClickHouseType("String")]
            public string ColumnVarchar { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<AttributeTable> CreateAttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new AttributeTable
                {
                    Id = i,
                    ColumnBigint = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnBlob = Guid.NewGuid().ToString(),
                    ColumnDateTime2 = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                    ColumnVarchar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
                };
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestClickHouseConnectionForInsertForClickHouseMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<AttributeTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionForInsertAllForClickHouseMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<AttributeTable>());

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionForQueryForClickHouseMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<AttributeTable>(table);

                // Query
                var queryResult = connection.Query<AttributeTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionForQueryAllForClickHouseMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new ClickHouseConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<AttributeTable>(tables);

                // Query
                var queryResult = connection.QueryAll<AttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        #endregion
    }
}
