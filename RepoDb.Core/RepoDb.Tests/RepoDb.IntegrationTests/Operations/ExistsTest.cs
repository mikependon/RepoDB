using System.Linq;
using System.Threading.Tasks;
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
        private class IdentityTableRepository : BaseRepository<IdentityTable, SqlConnection>
        {
            public IdentityTableRepository(string connectionString) : base(connectionString, (int?)0) { }
        }

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>((object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(item => item.ColumnInt >= 2 && item.ColumnInt <= 8);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(queryGroup);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionExistsAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        #endregion

        #region ExistsAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionExistsViaAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsViaAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync<IdentityTable>(new { ColumnInt = 1 });

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsViaAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5);

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsViaAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionExistsViaAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.ExistsAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExistsViaViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThan, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        #region Exists<TWhat>(Repository)

        [TestMethod]
        public void TestBaseRepositoryExistsViaTWhatWithExistingKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository(Database.ConnectionString))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<long>(tables.First().Id);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task TestBaseRepositoryExistsAsyncViaTWhatWithExistingKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository(Database.ConnectionString))
            {
                // Act
                await repository.InsertAllAsync(tables);

                // Act
                var result = await repository.ExistsAsync<long>(tables.First().Id);

                // Assert
                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public void TestBaseRepositoryExistsViaTWhatWithNonExistingKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository(Database.ConnectionString))
            {
                // Act
                repository.InsertAll(tables);

                // Act
                var result = repository.Exists<long>(-1);

                // Assert
                Assert.IsFalse(result);
            }
        }

        [TestMethod]
        public async Task TestBaseRepositoryExistsAsyncViaTWhatWithNonExistingKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var repository = new IdentityTableRepository(Database.ConnectionString))
            {
                // Act
                await repository.InsertAllAsync(tables);

                // Act
                var result = await repository.ExistsAsync<long>(-1);

                // Assert
                Assert.IsFalse(result);
            }
        }

        #endregion
    }
}
