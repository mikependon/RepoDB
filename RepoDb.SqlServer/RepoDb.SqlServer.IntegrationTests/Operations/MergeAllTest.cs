using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Linq;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.SqlServer.IntegrationTests.Models;
using Microsoft.Data.SqlClient;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class MergeAllTest
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
        public void TestSqlConnectionMergeAllForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<CompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync<CompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<CompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<CompleteTable>(tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<NonIdentityCompleteTable>(tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Assert
                Assert.AreEqual(tables.Count, result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers);

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsExpandoObjectViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsExpandoObjects(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());
                Assert.AreEqual(tables.Count, result);
                Assert.IsTrue(tables.All(table => ((dynamic)table).Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(queryResult.First(e => e.Id == ((dynamic)table).Id), table));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<CompleteTable>());

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables,
                    qualifiers).Result;

                // Assert
                Assert.AreEqual(tables.Count, connection.CountAll<NonIdentityCompleteTable>());

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count, queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion
    }
}
