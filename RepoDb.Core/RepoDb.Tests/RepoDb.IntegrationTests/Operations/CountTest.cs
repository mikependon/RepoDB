using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class CountTest
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

        #region Count<TEntity>

        [TestMethod]
        public void TestSqlConnectionCountWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.AreEqual(7, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region Count(TableName)

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionCountViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Count(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion

        #region CountAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionCountViaAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountViaAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountViaAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.AreEqual(5, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountViaAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionCountViaAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(3, result);
            }
        }

        #endregion
    }
}
