using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
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
        public void TestDb2ConnectionDeleteWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>((object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(tables.First());

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(e => e.Id == tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete<CompleteTable>(queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>((object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncWithoutExpressionWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public async Task TestDb2ConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(tables.First());

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(e => e.Id == tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync<CompleteTable>(queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

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
        public void TestDb2ConnectionDeleteViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public void TestDb2ConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = connection.Delete(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), (object)null);

            // Assert
            Assert.AreEqual(tables.Count(), result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), tables.First().Id);

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new { Id = tables.First().Id });

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), new QueryField("Id", tables.First().Id));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryFields);

            // Assert
            Assert.AreEqual(8, result);
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act
            var result = await connection.DeleteAsync(ClassMappedNameCache.Get<CompleteTable>(), queryGroup);

            // Assert
            Assert.AreEqual(8, result);
        }

        #endregion

        #endregion

        #region Hints

        [TestMethod]
        public void TestDb2ConnectionDeleteWithHintsThrows()
        {
            // Setup
            var tables = Database.CreateCompleteTables(1);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            Assert.Throws<System.NotSupportedException>(() =>
                connection.Delete<CompleteTable>(tables.First().Id, hints: "NOLOCK"));
        }

        [TestMethod]
        public async Task TestDb2ConnectionDeleteAsyncWithHintsThrows()
        {
            // Setup
            var tables = Database.CreateCompleteTables(1);

            using var connection = new DB2Connection(Database.ConnectionString);

            // Act/Assert: AreTableHintsSupported = false for Db2 - BaseStatementBuilder.GuardHints
            // throws for any non-null/non-whitespace hints, regardless of operation.
            await Assert.ThrowsAsync<System.NotSupportedException>(() =>
                connection.DeleteAsync<CompleteTable>(tables.First().Id, hints: "NOLOCK"));
        }

        #endregion
    }
}
