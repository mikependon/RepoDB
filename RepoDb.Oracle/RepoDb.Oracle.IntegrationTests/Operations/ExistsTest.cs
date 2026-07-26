using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class ExistsTest
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
        public void TestOracleConnectionExistsWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaExpressionNoMatch()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var missingId = tables.Max(t => t.Id) + 1000;

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(e => e.Id == missingId);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.Exists<CompleteTable>((object)null, hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync<CompleteTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestOracleConnectionExistsViaTableNameWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestOracleConnectionExistsViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaTableNameWithoutExpression()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionExistsAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #endregion
    }
}
