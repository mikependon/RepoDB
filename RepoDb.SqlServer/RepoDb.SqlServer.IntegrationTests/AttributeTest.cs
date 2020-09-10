using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;

namespace RepoDb.SqlServer.IntegrationTests
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
        public class MdsAttributeTable
        {
            public int Id { get; set; }

            [MicrosoftSqlServerTypeMap(SqlDbType.UniqueIdentifier)]
            public Guid SessionId { get; set; }

            [MicrosoftSqlServerTypeMap(SqlDbType.Binary)]
            public byte[] ColumnBinary { get; set; }

            [MicrosoftSqlServerTypeMap(SqlDbType.BigInt)]
            public long ColumnBigInt { get; set; }

            [MicrosoftSqlServerTypeMap(SqlDbType.DateTime2)]
            public DateTime ColumnDateTime2 { get; set; }

            [MicrosoftSqlServerTypeMap(SqlDbType.Text)]
            public string ColumnNVarChar { get; set; }
        }

        [Table("CompleteTable")]
        public class SdsAttributeTable
        {
            public int Id { get; set; }

            [SystemSqlServerTypeMap(SqlDbType.UniqueIdentifier)]
            public Guid SessionId { get; set; }

            [SystemSqlServerTypeMap(SqlDbType.Binary)]
            public byte[] ColumnBinary { get; set; }

            [SystemSqlServerTypeMap(SqlDbType.BigInt)]
            public long ColumnBigInt { get; set; }

            [SystemSqlServerTypeMap(SqlDbType.DateTime2)]
            public DateTime ColumnDateTime2 { get; set; }

            [SystemSqlServerTypeMap(SqlDbType.Text)]
            public string ColumnNVarChar { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<MdsAttributeTable> CreateMdsAttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new MdsAttributeTable
                {
                    Id = i,
                    ColumnBigInt = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnBinary = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDateTime2 = DateTime.UtcNow.AddDays(-random.Next(100)),
                    ColumnNVarChar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}",
                    SessionId = Guid.NewGuid()
                };
            }
        }

        private IEnumerable<SdsAttributeTable> CreateSdsAttributeTables(int count = 10)
        {
            var random = new Random();
            for (var i = 0; i < count; i++)
            {
                yield return new SdsAttributeTable
                {
                    Id = i,
                    ColumnBigInt = Convert.ToInt64(random.Next(int.MaxValue)),
                    ColumnBinary = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()),
                    ColumnDateTime2 = DateTime.UtcNow.AddDays(-random.Next(100)),
                    ColumnNVarChar = $"ColumnNVarChar-{i}-{Guid.NewGuid()}",
                    SessionId = Guid.NewGuid()
                };
            }
        }

        #endregion

        #region MDS

        [TestMethod]
        public void TestSqlConnectionForInsertForMicrosoftSqlServerTypeMapAttribute()
        {
            // Setup
            var table = CreateMdsAttributeTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<MdsAttributeTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<MdsAttributeTable>());

                // Query
                var queryResult = connection.QueryAll<MdsAttributeTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionForInsertAllForMicrosoftSqlServerTypeMapAttribute()
        {
            // Setup
            var tables = CreateMdsAttributeTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<MdsAttributeTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<MdsAttributeTable>());

                // Query
                var queryResult = connection.QueryAll<MdsAttributeTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionForQueryForMicrosoftSqlServerTypeMapAttribute()
        {
            // Setup
            var table = CreateMdsAttributeTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<MdsAttributeTable>(table);

                // Query
                var queryResult = connection.Query<MdsAttributeTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionForQueryAllForMicrosoftSqlServerTypeMapAttribute()
        {
            // Setup
            var tables = CreateMdsAttributeTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<MdsAttributeTable>(tables);

                // Query
                var queryResult = connection.QueryAll<MdsAttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        #endregion

        #region SDS

        [TestMethod]
        public void TestSqlConnectionForInsertForSystemSqlServerTypeMapAttribute()
        {
            // Setup
            var table = CreateSdsAttributeTables(1).First();

            using (var connection = new System.Data.SqlClient.SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Insert<SdsAttributeTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<SdsAttributeTable>());

                // Query
                var queryResult = connection.QueryAll<SdsAttributeTable>().First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionForInsertAllForSystemSqlServerTypeMapAttribute()
        {
            // Setup
            var tables = CreateSdsAttributeTables(10).AsList();

            using (var connection = new System.Data.SqlClient.SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<SdsAttributeTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<SdsAttributeTable>());

                // Query
                var queryResult = connection.QueryAll<SdsAttributeTable>();

                // Assert
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionForQueryForSystemSqlServerTypeMapAttribute()
        {
            // Setup
            var table = CreateSdsAttributeTables(1).First();

            using (var connection = new System.Data.SqlClient.SqlConnection(Database.ConnectionString))
            {
                // Act
                var id = connection.Insert<SdsAttributeTable>(table);

                // Query
                var queryResult = connection.Query<SdsAttributeTable>(id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionForQueryAllForSystemSqlServerTypeMapAttribute()
        {
            // Setup
            var tables = CreateSdsAttributeTables(10).AsList();

            using (var connection = new System.Data.SqlClient.SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll<SdsAttributeTable>(tables);

                // Query
                var queryResult = connection.QueryAll<SdsAttributeTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
            }
        }

        #endregion
    }
}
