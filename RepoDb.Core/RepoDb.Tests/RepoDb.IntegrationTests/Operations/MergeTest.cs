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
        public void TestSqlConnectionMergeForIdentityForEmptyTableViaEntityTableName()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    item);

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
        public void TestSqlConnectionMergeForIdentityForEmptyTable()
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
        public void TestSqlConnectionMergeForNonIdentityForEmptyTable()
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
        public void TestSqlConnectionMergeForIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeForNonIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeForIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForIdentityForNonEmptyTable()
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
        public void TestSqlConnectionMergeForNonIdentityForNonEmptyTable()
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
        public void TestSqlConnectionMergeForIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForIdentityWithTypedResultForNonEmptyTable()
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
        public void TestSqlConnectionMergeForNonIdentityWithTypedResultForNonEmptyTable()
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
        public void TestSqlConnectionMergeForIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeForIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeForIdentityForEmptyTableWithHints()
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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableViaEntityTableName()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    item).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForNonIdentityForEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForNonIdentityForNonEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithTypedResultForNonEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithTypedResultForNonEmptyTable()
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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(item,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(item);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableWithHints()
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
        public void TestSqlConnectionMergeViaDynamicTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

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
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), item);

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
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
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

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var item = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Merge<object>(ClassMappedNameCache.Get<NonKeyedTable>(), (object)item);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithTypedResultForEmptyTable()
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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForEmptyTableWithHints()
        {
            // Setup
            var item = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(), (object)item,
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
                connection.MergeAsync(ClassMappedNameCache.Get<NonKeyedTable>(), (object)item).Wait();
            }
        }

        #endregion
    }
}
