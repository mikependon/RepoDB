using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForEmptyTableViaEntityTableNameWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));


                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForIdentityForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<IdentityTable>(entity, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Merge<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(entity);

                // Act
                var mergeResult = connection.Merge<WithExtraFieldsIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTableViaEntityTableName()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTableViaEntityTableNameWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncForIdentityForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<IdentityTable>(entity, hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<IdentityTable>());

                // Act
                var queryResult = connection.Query<IdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAsync<TEntity>(Extra Fields)

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<WithExtraFieldsIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(entity);

                // Act
                var mergeResult = await connection.MergeAsync<WithExtraFieldsIdentityTable>(entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<WithExtraFieldsIdentityTable>());

                // Act
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Merge(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameWithFieldsForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameWithFieldsForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Assert.AreEqual(queryResult.ColumnNVarChar, ((dynamic)entity).ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForExpandoObjectNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Setup
                var table = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                table["Id"] = entity.Id;

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForExpandoObjectNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Setup
                var table = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                table["Id"] = entity.Id;

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table,
                    qualifiers: Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.Merge<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableEmptyTableWithIncompleteProperties()
        {
            // Setup
            var entity = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityTableForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionMergeViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var entity = Helper.CreateDynamicNonKeyedTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Merge<object>(ClassMappedNameCache.Get<NonKeyedTable>(), (object)entity);
            }
        }

        #endregion

        #region MergeAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaDynamicTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaDynamicTableNameWithFieldsForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaDynamicTableNameForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaDynamicTableNameWithFieldsForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Assert.AreEqual(queryResult.ColumnNVarChar, ((dynamic)entity).ColumnNVarChar);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Setup
                var table = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                table["Id"] = entity.Id;

                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Setup
                var table = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                table["Id"] = entity.Id;

                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table,
                    qualifiers: Field.From(nameof(NonIdentityTable.Id)));

                // Assert
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());
                Assert.AreEqual(((dynamic)entity).Id, mergeResult);

                // Act
                var queryResult = connection.Query<NonIdentityTable>(mergeResult).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt)));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = await connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) }));

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableEmptyTableWithIncompleteProperties()
        {
            // Setup
            var entity = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll(ClassMappedNameCache.Get<NonIdentityTable>()));

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = await connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public async Task ThrowExceptionOnSqlConnectionMergeAsyncViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var entity = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                await connection.MergeAsync(ClassMappedNameCache.Get<NonKeyedTable>(),
                    (object)entity);
            }
        }

        #endregion
    }
}
