using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.Extensions;
using RepoDb.SqlServer.IntegrationTests.Setup;
using RepoDb.SqlServer.IntegrationTests.Models;
using System.Linq;

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
        public void TestSqlServerConnectionMergeAllForIdentityForEmptyTable()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllForIdentityForNonEmptyTable()
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllForIdentityForNonEmptyTableWithQualifiers()
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAll<NonIdentityCompleteTable>(tables);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllForNonIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<CompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForIdentityForNonEmptyTable()
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForIdentityForNonEmptyTableWithQualifiers()
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
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                tables.ForEach(table => Helper.UpdateNonIdentityCompleteTableProperties(table));

                // Act
                var result = connection.MergeAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncForNonIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertPropertiesEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForIdentityForNonEmptyTable()
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

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    entities);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table =>
                {
                    Assert.AreEqual(table.ColumnInt,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)entities.IndexOf(table)).ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<CompleteTable>(),
                    entities,
                    qualifiers);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table =>
                {
                    Assert.AreEqual(table.ColumnInt,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)entities.IndexOf(table)).ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTable()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.ElementAt(tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    entities);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table =>
                {
                    Assert.AreEqual(table.ColumnInt,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)entities.IndexOf(table)).ColumnInt);
                });
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    entities,
                    qualifiers);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameAsyncForIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTable()
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

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllViaTableNameAsyncForIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entities).Result;

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.First(e => e.Id == table.Id).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entities,
                    qualifiers).Result;

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTables(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTable()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table,
                      queryResult.OrderBy(e => e.Id).ElementAt(tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
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

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForEmptyTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table))));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    entities).Result;

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        [TestMethod]
        public void TestSqlServerConnectionMergeAllAsyncAsDynamicsViaTableNameForNonIdentityForNonEmptyTableWithQualifiers()
        {
            // Setup
            var tables = Database.CreateNonIdentityCompleteTables(10).AsList();
            var entities = tables.Select(table => new
            {
                Id = table.Id,
                ColumnInt = int.MaxValue
            }).AsList();
            var qualifiers = new[]
            {
                new Field("Id", typeof(long))
            };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MergeAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    entities,
                    qualifiers).Result;

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                Assert.AreEqual(tables.Count(), queryResult.Count());
                entities.ForEach(table => Assert.AreEqual(table.ColumnInt, queryResult.ElementAt((int)entities.IndexOf(table)).ColumnInt));
            }
        }

        #endregion

        #endregion
    }
}
