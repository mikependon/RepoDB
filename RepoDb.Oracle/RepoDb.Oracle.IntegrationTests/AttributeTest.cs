using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Attributes.Parameter.Oracle;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace RepoDb.Oracle.IntegrationTests
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
        // OracleDbTypeAttribute directly on the parameter without also going through the separate
        // Guid<->byte[] PropertyHandler mechanism used elsewhere in this suite - RAW(16) already maps
        // to byte[] natively, no conversion needed.
        [Table("CompleteTable")]
        public class OracleAttributeTable
        {
            public int Id { get; set; }

            [OracleDbType(OracleDbType.Raw)]
            public byte[] SessionId { get; set; }

            [OracleDbType(OracleDbType.NVarchar2)]
            public string ColumnVarchar { get; set; }

            [OracleDbType(OracleDbType.Decimal)]
            public decimal ColumnNumber { get; set; }

            [OracleDbType(OracleDbType.Date)]
            public DateTime ColumnDate { get; set; }

            [OracleDbType(OracleDbType.TimeStamp)]
            public DateTime ColumnTimestamp { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<OracleAttributeTable> CreateOracleAttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new OracleAttributeTable
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

        #region OracleDbType

        [TestMethod]
        public void TestOracleConnectionForInsertForOracleDbTypeAttribute()
        {
            // Setup
            var table = CreateOracleAttributeTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            connection.Insert<OracleAttributeTable>(table);

            // Assert
            Assert.AreEqual(1, connection.CountAll<OracleAttributeTable>());

            // Query
            var queryResult = connection.QueryAll<OracleAttributeTable>().First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionForInsertAllForOracleDbTypeAttribute()
        {
            // Setup
            var tables = CreateOracleAttributeTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            connection.InsertAll<OracleAttributeTable>(tables);

            // Assert
            Assert.AreEqual(tables.Count, connection.CountAll<OracleAttributeTable>());

            // Query
            var queryResult = connection.QueryAll<OracleAttributeTable>();

            // Assert
            tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
        }

        [TestMethod]
        public void TestOracleConnectionForQueryForOracleDbTypeAttribute()
        {
            // Setup
            var table = CreateOracleAttributeTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var id = connection.Insert<OracleAttributeTable>(table);

            // Query
            var queryResult = connection.Query<OracleAttributeTable>(id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, queryResult);
        }

        [TestMethod]
        public void TestOracleConnectionForQueryAllForOracleDbTypeAttribute()
        {
            // Setup
            var tables = CreateOracleAttributeTables(10).AsList();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            connection.InsertAll<OracleAttributeTable>(tables);

            // Query
            var queryResult = connection.QueryAll<OracleAttributeTable>();

            // Assert
            Assert.AreEqual(tables.Count, queryResult.Count());
        }

        #endregion
    }
}
