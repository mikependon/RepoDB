using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RepoDb.MySqlConnector.IntegrationTests
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
            public int Id { get; set; }

            [MySqlConnectorTypeMap(MySqlDbType.Blob)]
            public byte[] ColumnBlob { get; set; }

            [MySqlConnectorTypeMap(MySqlDbType.Int64)]
            public long ColumnBigInt { get; set; }

            [MySqlConnectorTypeMap(MySqlDbType.DateTime)]
            public DateTime ColumnDateTime2 { get; set; }

            [MySqlConnectorTypeMap(MySqlDbType.VarChar)]
            public string ColumnVarChar { get; set; }
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
                    ColumnBigInt = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnBlob = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDateTime2 = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                    ColumnVarChar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}"
                };
            }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestMySqlConnectionForInsertForMySqlMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionForInsertAllForMySqlMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionForQueryForMySqlMapAttribute()
        {
            // Setup
            var table = CreateAttributeTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<AttributeTable>(table);

                // Query
                var queryResult = connection.Query<AttributeTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionForQueryAllForMySqlMapAttribute()
        {
            // Setup
            var tables = CreateAttributeTables(10).AsList();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
