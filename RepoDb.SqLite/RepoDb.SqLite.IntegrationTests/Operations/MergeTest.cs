using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestMergeForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge<CompleteTable>(table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge<CompleteTable>(table,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMergeAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.MergeAsync<CompleteTable>(table,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMergeViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateCompleteTableProperties(table);
                table.ColumnInt = 0;
                table.ColumnChar = "C";

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var table = Helper.CreateCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var obj = new
                {
                    table.Id,
                    ColumnInt = int.MaxValue
                };

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        [TestMethod]
        public void TestMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var obj = new
                {
                    table.Id,
                    ColumnInt = int.MaxValue
                };
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Create the tables
                Database.CreateTables(connection);

                // Setup
                var table = Helper.CreateCompleteTablesAsDynamics(1).First();

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var obj = new
                {
                    table.Id,
                    ColumnInt = int.MaxValue
                };

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        [TestMethod]
        public void TestMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var obj = new
                {
                    table.Id,
                    ColumnInt = int.MaxValue
                };
                var qualifiers = new[]
                {
                    new Field("Id", typeof(long))
                };

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(result);

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        #endregion

        #endregion
    }
}
