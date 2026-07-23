using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class MergeTest
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
        public void TestSqlConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<IdentityCompleteTable>(table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<IdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge<IdentityCompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync<IdentityCompleteTable>(table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<IdentityCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = await connection.MergeAsync<IdentityCompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForExpandoObjectIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForExpandoObjectIdentityForNonEmptyTable()
        {
            // Setup
            Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                Helper.UpdateCompleteTableAsExpandoObjectProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectIdentityForNonEmptyTable()
        {
            // Setup
            Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                Helper.UpdateCompleteTableAsExpandoObjectProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }
        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateIdentityCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<IdentityCompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<IdentityCompleteTable>());
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<IdentityCompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion
    }
}
