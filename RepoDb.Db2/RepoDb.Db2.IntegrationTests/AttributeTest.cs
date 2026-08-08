using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Attributes.Parameter.Db2;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests
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

        // Re-maps onto the real "CompleteTable" physical table (see Setup/Database.cs). "SessionId" is
        // typed as byte[] here (instead of Guid) specifically so this class can exercise
        // Db2DbTypeAttribute directly on the parameter without also going through the separate
        // Guid<->byte[] PropertyHandler mechanism used elsewhere in this suite - RAW(16) already maps
        // to byte[] natively, no conversion needed.
        [Table("CompleteTable")]
        public class Db2AttributeTable
        {
            public int Id { get; set; }

            [Db2DbType(Db2DbType.Raw)]
            public byte[] SessionId { get; set; }

            [Db2DbType(Db2DbType.NVarchar2)]
            public string ColumnVarchar { get; set; }

            [Db2DbType(Db2DbType.Decimal)]
            public decimal ColumnNumber { get; set; }

            [Db2DbType(Db2DbType.Date)]
            public DateTime ColumnDate { get; set; }

            [Db2DbType(Db2DbType.TimeStamp)]
            public DateTime ColumnTimestamp { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<Db2AttributeTable> CreateDb2AttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new Db2AttributeTable
                {
                    SessionId = Guid.NewGuid().ToByteArray(),
                    ColumnVarchar = $"ColumnVarchar-{i}-{Guid.NewGuid()}",
                    ColumnNumber = Math.Round(Convert.ToDecimal(random.NextDouble() * 1000), 12),
                    ColumnDate = DateTime.UtcNow.Date.AddDays(-random.Next(100)),
                    ColumnTimestamp = DateTime.UtcNow
                };
            }
        }

        #endregion

        #region Db2DbType

        [TestMethod]
        public void TestDb2ConnectionForInsertForDb2DbTypeAttribute()
        {
            // Setup
            var table = CreateDb2AttributeTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            connection.Insert<Db2AttributeTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<Db2AttributeTable>());

            // Query
            var queryResult = connection.QueryAll<Db2AttributeTable>().First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionForInsertAllForDb2DbTypeAttribute()
        {
            // Setup
            var tables = CreateDb2AttributeTables(10).AsList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            connection.InsertAll<Db2AttributeTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, connection.CountAll<Db2AttributeTable>());

            // Query
            var queryResult = connection.QueryAll<Db2AttributeTable>();

            // Assert
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestDb2ConnectionForQueryForDb2DbTypeAttribute()
        {
            // Setup
            var table = CreateDb2AttributeTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var id = connection.Insert<Db2AttributeTable>(table);

            // Query
            var queryResult = connection.Query<Db2AttributeTable>(id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestDb2ConnectionForQueryAllForDb2DbTypeAttribute()
        {
            // Setup
            var tables = CreateDb2AttributeTables(10).AsList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            connection.InsertAll<Db2AttributeTable>(tables);

            // Query
            var queryResult = connection.QueryAll<Db2AttributeTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }

        [TestMethod]
        public async Task TestDb2ConnectionForInsertAsyncForDb2DbTypeAttribute()
        {
            // Setup
            var table = CreateDb2AttributeTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            await connection.InsertAsync<Db2AttributeTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<Db2AttributeTable>());

            // Query
            var queryResult = connection.QueryAll<Db2AttributeTable>().First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionForInsertAllAsyncForDb2DbTypeAttribute()
        {
            // Setup
            var tables = CreateDb2AttributeTables(10).AsList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            await connection.InsertAllAsync<Db2AttributeTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, connection.CountAll<Db2AttributeTable>());

            // Query
            var queryResult = connection.QueryAll<Db2AttributeTable>();

            // Assert
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public async Task TestDb2ConnectionForQueryAsyncForDb2DbTypeAttribute()
        {
            // Setup
            var table = CreateDb2AttributeTables(1).First();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            var id = connection.Insert<Db2AttributeTable>(table);

            // Query
            var queryResult = (await connection.QueryAsync<Db2AttributeTable>(id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public async Task TestDb2ConnectionForQueryAllAsyncForDb2DbTypeAttribute()
        {
            // Setup
            var tables = CreateDb2AttributeTables(10).AsList();

            using var connection = new Db2Connection(Database.ConnectionString);

            // Act
            connection.InsertAll<Db2AttributeTable>(tables);

            // Query
            var queryResult = await connection.QueryAllAsync<Db2AttributeTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }

        #endregion
    }
}
