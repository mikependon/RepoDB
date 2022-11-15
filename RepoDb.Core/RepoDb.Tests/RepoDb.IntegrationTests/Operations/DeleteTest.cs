using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Operations
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

        #region Delete<TEntity>

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntityViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntityViaPrimaryKeyAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable>((object)item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntityViaPrimaryKeyAsType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable, long>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntityViaPrimaryKeyAsTypeAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable, object>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable>(item);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteWithEmptyQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(Enumerable.Empty<QueryField>());

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(c => c.Id == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                tables.ForEach(item =>
                {
                    // Act
                    var result = connection.Delete<IdentityTable>(item, hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, result);
                });

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region DeleteAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntityViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntityViaPrimaryKeyAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable>((object)item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntityViaPrimaryKeyAsType()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable, long>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntityViaPrimaryKeyAsTypeAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable, object>(item.Id);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable>(item);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>((object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncWithEmptyQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete<IdentityTable>(Enumerable.Empty<QueryField>());

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var last = tables.Last();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(c => c.ColumnInt == last.Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                foreach (var item in tables)
                {
                    // Act
                    var result = await connection.DeleteAsync<IdentityTable>(item, hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, result);
                }

                // Assert
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        #endregion

        #region Delete(TableName)

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameWithPrimaryKeyAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameWithPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameAsEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.First());

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    new QueryField(nameof(IdentityTable.ColumnInt), 6));

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteViaTableNameWithoutConditionButWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionDeleteViaTableNameIfThereIsNoKeyField()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                connection.Delete(ClassMappedNameCache.Get<NonKeyedTable>(), 1);
            }
        }

        #endregion

        #region DeleteAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameWithPrimaryKeyAsObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameWithPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.First().Id);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameAsEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    tables.First());

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(9, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameWithoutCondition()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    new { ColumnInt = 6 });

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 6);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    field);

                // Assert
                Assert.AreEqual(1, result);
                Assert.AreEqual(tables.Count - 1, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    fields);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionDeleteAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.GreaterThanOrEqual, 5),
                new QueryField(nameof(IdentityTable.ColumnInt), Operation.LessThanOrEqual, 8)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.DeleteAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(4, result);
                Assert.AreEqual(6, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteAsyncViaTableNameWithoutConditionButWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Delete(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(10, result);
                Assert.AreEqual(0, connection.CountAll<IdentityTable>());
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public async Task ThrowExceptionOnSqlConnectionDeleteAsyncViaTableNameIfThereIsNoKeyField()
        {
            await using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                await connection.DeleteAsync(ClassMappedNameCache.Get<NonKeyedTable>(), 1);
            }
        }

        #endregion
    }
}
