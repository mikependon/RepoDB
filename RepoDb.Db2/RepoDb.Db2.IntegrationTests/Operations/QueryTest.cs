using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    // NOTE: this file intentionally covers only the single-row Query<TEntity> operation, mirroring
    // RepoDb.SqlServer.IntegrationTests.Operations.QueryTest. QueryAll/BatchQuery are different
    // operations with their own dedicated sibling files in this same Operations folder (owned by other
    // workstreams splitting up this same test-expansion effort), so the QueryAll/BatchQuery scenarios that
    // used to live in this file (in thin form) were moved out rather than duplicated here.
    [TestClass]
    public class QueryTest
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestDb2ConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaPrimaryKeyWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(new { table.Id }).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(queryFields).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(queryGroup).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestDb2ConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act: exercises the "FETCH FIRST n ROWS ONLY" override.
            var result = connection.Query<CompleteTable>((object)null,
                top: 2);

            // Assert
            Assert.AreEqual(2, result.Count());
            foreach (var item in result)
            {
                Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaPrimaryKeyWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = (await connection.QueryAsync<CompleteTable>(table.Id)).First();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(new { table.Id })).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id))).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(queryFields)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(queryGroup)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.QueryAsync<CompleteTable>((object)null,
                top: 2);

            // Assert
            Assert.AreEqual(2, result.Count());
            foreach (var item in result)
            {
                Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item);
            }
        }

        #endregion

        #endregion

        // NOTE: RepoDb.SqlServer.IntegrationTests.Operations.QueryTest also has a "TableName" region here
        // (connection.Query(tableName, ...)). That overload returns IEnumerable<dynamic> in this provider
        // too - there's no typed CompleteTable result to run Helper.AssertPropertiesEquality against, and
        // this project has no Helper.AssertMembersEquality (SqlServer-only) for the dynamic/ExpandoObject
        // case. Skipped rather than asserting loosely against an untyped result.

        #region Hints

        [TestMethod]
        public void TestDb2ConnectionQueryWithHintsThrows()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Query<CompleteTable>((object)null, hints: "NOLOCK"));
        }

        [TestMethod]
        public async Task TestDb2ConnectionQueryAsyncWithHintsThrows()
        {
            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            await Assert.ThrowsAsync<System.NotSupportedException>(() =>
                connection.QueryAsync<CompleteTable>((object)null, hints: "NOLOCK"));
        }

        #endregion
    }
}
