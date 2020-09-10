using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Extensions;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using RepoDb.PostgreSql.IntegrationTests.Models;
using System.Linq;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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
        public void TestPostgreSqlConnectionMergeAllForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
        public void TestPostgreSqlConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
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
