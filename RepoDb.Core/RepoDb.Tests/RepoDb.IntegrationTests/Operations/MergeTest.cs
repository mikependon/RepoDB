using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableViaEntityTableName()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableViaEntityTableNameWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(nameof(IdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<IdentityTable, long>(entity,
                    qualifiers: Field.From(new[] { nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<NonIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<NonIdentityTable, Guid>(entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncForIdentityForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<IdentityTable>(entity, hints: SqlServerTableHints.TabLock).Result;

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
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncWithExtraFieldsForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<WithExtraFieldsIdentityTable>(entity);

                // Act
                var mergeResult = connection.MergeAsync<WithExtraFieldsIdentityTable>(entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameWithFieldsForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTableWithFields()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameWithFieldsForExpandoObjectNonIdentityTableForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateExpandoObjectNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectNonIdentityTableForNonEmptyTable()
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
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForExpandoObjectNonIdentityTableWithQualifierForNonEmptyTable()
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
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table,
                    qualifiers: Field.From(nameof(NonIdentityTable.Id))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifierWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(nameof(NonIdentityTable.ColumnInt))).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableWithQualifiersWithTypedResultForNonEmptyTable()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity);

                // Act
                var mergeResult = connection.MergeAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    qualifiers: Field.From(new[] { nameof(NonIdentityTable.ColumnInt), nameof(NonIdentityTable.ColumnDecimal) })).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableEmptyTableWithIncompleteProperties()
        {
            // Setup
            var entity = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    entity).Result;

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
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityTableForEmptyTableWithHints()
        {
            // Setup
            var entity = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.MergeAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)entity,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(entity.Id, mergeResult);
                Assert.AreEqual(1, connection.CountAll<NonIdentityTable>());

                // Act
                var queryResult = connection.Query<NonIdentityTable>((Guid)entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionMergeAsyncViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var entity = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.MergeAsync(ClassMappedNameCache.Get<NonKeyedTable>(),
                    (object)entity).Wait();
            }
        }

        #endregion
    }
}
