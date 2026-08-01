using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class DeleteTest
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
        public void TestOracleConnectionDeleteWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>((object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = connection.Delete<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(tables.First());

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>((object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
            try
            {
                // Act
                var result = await connection.DeleteAsync<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
            finally
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Default;
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(tables.First());

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestOracleConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public async Task TestOracleConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestOracleConnectionDeleteWithHintsThrows()
        {
            // Setup
            var tables = Database.CreateCompleteTables(1);

            using var connection = new OracleConnection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Oracle - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Delete<CompleteTable>(tables.First().Id, hints: "NOLOCK"));
        }

        #endregion
    }
}
