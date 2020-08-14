using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
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

        #region Merge<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item, Field.From(nameof(IdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item, Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item, hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(item.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(item, queryResult);
            }
        }

        #endregion

        #region Merge(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Merge(ClassMappedNameCache.Get<IdentityTable>(), (object)item);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentitySingleEntityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(item.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAsyncViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var item = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAsync(ClassMappedNameCache.Get<IdentityTable>(), (object)item).Wait();
            }
        }

        #endregion
    }
}
