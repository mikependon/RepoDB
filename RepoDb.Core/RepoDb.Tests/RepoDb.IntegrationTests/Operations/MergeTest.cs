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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityForEmptyTableWithFields()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifierForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifierWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifierForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityForEmptyTableWithFields()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query<NonIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<IdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).First();

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
                var queryResult = connection.Query<WithExtraFieldsIdentityTable>(entity.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Merge(TableName)

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaDynamicTableNameWithFieldsForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForEmptyTableWithFields()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeViaTableNameForNonIdentityForEmptyTableWithHints()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
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
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaDynamicTableNameWithFieldsForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForEmptyTableWithFields()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Assert.AreEqual(entity.ColumnNVarChar, queryResult.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersWithTypedResultForEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifierWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityWithQualifiersWithTypedResultForNonEmptyTable()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAsyncViaTableNameForNonIdentityForEmptyTableWithHints()
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
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)entity.Id).First();

                // Assert
                Helper.AssertMembersEquality(entity, queryResult);
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
