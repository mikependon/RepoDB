using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System;
using System.Linq;

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
        public void TestSqlServerConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge<CompleteTable>(table);
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge<CompleteTable>(table);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
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
                var result = connection.Merge<CompleteTable>(table,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync<CompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertPropertiesEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
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
                var result = connection.MergeAsync<CompleteTable>(table,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

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
        public void TestSqlServerConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
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
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table);

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInt = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInt = int.MaxValue
            };
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncAsDynamicViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTablesAsDynamics(1).First();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)table).Result;

                // Assert
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInt = int.MaxValue
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInt = int.MaxValue
            };
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInt, queryResult.First().ColumnInt);
            }
        }

        #endregion

        #endregion
    }
}
