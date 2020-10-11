using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInteger = 0;
                table.ColumnCharacter = "C";

                // Act
                var result = connection.MergeAsync<CompleteTable>(table,
                    qualifiers: qualifiers).Result;

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
        public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsExpandoObjects(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, connection.CountAll<CompleteTable>());
                Assert.AreEqual(((dynamic)table).Id, result);
                Helper.AssertMembersEquality(queryResult.First(), table);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncAsExpandoObjectViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;

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
        public void TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers: qualifiers).Result;

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
