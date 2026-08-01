using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
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
        public void TestOracleConnectionQueryViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryViaPrimaryKeyWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionQueryViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(e => e.Id == table.Id).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(new { table.Id }).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(new QueryField("Id", table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(queryFields).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Query<CompleteTable>(queryGroup).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public void TestOracleConnectionQueryWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionQueryAsyncViaPrimaryKey()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncViaPrimaryKeyWithAutomaticConversion()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public async Task TestOracleConnectionQueryAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(e => e.Id == table.Id)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(new { table.Id })).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(new QueryField("Id", table.Id))).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(queryFields)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = (await connection.QueryAsync<CompleteTable>(queryGroup)).First();

            // Assert
            Helper.AssertPropertiesEquality(table, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryAsyncWithTop()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

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
        public void TestOracleConnectionQueryWithHintsThrows()
        {
            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Oracle - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Query<CompleteTable>((object)null, hints: "NOLOCK"));
        }

        #endregion
    }
}
