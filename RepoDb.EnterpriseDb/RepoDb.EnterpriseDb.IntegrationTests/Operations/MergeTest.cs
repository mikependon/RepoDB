using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<CompleteTable>(table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInteger = 0;
                table.ColumnCharacter = "C";

                // Act
                var result = connection.Merge<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInteger = 0;
                table.ColumnCharacter = "C";

                // Act
                var result = await connection.MergeAsync<CompleteTable>(table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), entity);
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInteger = 0;
                table.ColumnCharacter = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertMembersEquality(queryResult.First(), entity);
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion
    }
}
