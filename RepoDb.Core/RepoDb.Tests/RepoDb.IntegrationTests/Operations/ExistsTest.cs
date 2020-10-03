using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests.Operations
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

        #region Exists<TEntity>

        [TestMethod]
        public void TestSqlConnectionExistsWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists<IdentityTable>(field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryFields()
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
                var result = connection.Exists<IdentityTable>(fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaQueryGroup()
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
                var result = connection.Exists<IdentityTable>(queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionExistsAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>((object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryFields()
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
                var result = connection.ExistsAsync<IdentityTable>(fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsAsyncViaQueryGroup()
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
                var result = connection.ExistsAsync<IdentityTable>(queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region Exists(TableName)

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryFields()
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
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaTableNameViaQueryGroup()
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
                var result = connection.Exists(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionExistsViaAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 }).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaAsyncViaTableNameViaQueryFields()
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
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaAsyncViaTableNameViaQueryGroup()
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
                var result = connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup).Result;

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion
    }
}
