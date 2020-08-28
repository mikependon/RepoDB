using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System;
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
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
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
        public void TestPostgreSqlConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
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
        public void TestPostgreSqlConnectionMergeViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
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
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInteger = int.MaxValue
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                Assert.AreEqual(obj.ColumnInteger, queryResult.First().ColumnInteger);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInteger = int.MaxValue
            };
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Merge(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers: qualifiers);

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInteger, queryResult.First().ColumnInteger);
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
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
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
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
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
                Assert.IsTrue(Convert.ToInt64(result) > 0);

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.AreEqual(1, queryResult?.Count());
                Helper.AssertMembersEquality(table, queryResult.First());
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInteger = int.MaxValue
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
                Assert.AreEqual(obj.ColumnInteger, queryResult.First().ColumnInteger);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionMergeAsyncAsDynamicViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var obj = new
            {
                table.Id,
                ColumnInteger = int.MaxValue
            };
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)obj,
                    qualifiers: qualifiers).Result;

                // Assert
                Assert.AreEqual(table.Id, Convert.ToInt64(result));

                // Act
                var queryResult = connection.Query<CompleteTable>(Convert.ToInt64(result));

                // Assert
                Assert.IsTrue(queryResult.Count() > 0);
                Assert.AreEqual(obj.ColumnInteger, queryResult.First().ColumnInteger);
            }
        }

        #endregion

        #endregion
    }
}
