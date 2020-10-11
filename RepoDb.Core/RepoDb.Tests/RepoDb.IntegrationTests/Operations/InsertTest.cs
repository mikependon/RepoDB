using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Dynamic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class InsertTest
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

        #region Insert<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertViaEntityTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaEntityTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsert()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable>(table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateIdentityTable();

                // Act
                var id = connection.Insert<IdentityTable, long>(item);

                // Assert
                Assert.IsTrue(item.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(item, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertForNonIdentityTable()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var item = Helper.CreateNonIdentityTable();

                // Act
                connection.Insert<NonIdentityTable, Guid>(item);

                // Assert
                Assert.AreNotEqual(Guid.Empty, item.Id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(item, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable, long>(table,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Insert<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertWithExtraFields()
        {
            // Setup
            var table = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<WithExtraFieldsIdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region InsertAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaEntityTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaEntityTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsync()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<IdentityTable, long>(table).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<IdentityTable>(table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForIdentityTable()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<IdentityTable, long>(table);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncForNonIdentityTable()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<NonIdentityTable, Guid>(table).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, table.Id);

                // Act
                var result = connection.QueryAll<NonIdentityTable>();

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<IdentityTable, long>(table,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region InsertAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionInsertAsyncWithExtraFields()
        {
            // Setup
            var table = Helper.CreateWithExtraFieldsIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<WithExtraFieldsIdentityTable, long>(table).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region Insert(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableNameAsDynamic()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<dynamic, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table);

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableNameAsDynamicWithFields()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<dynamic, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableNameAsExpandoObject()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<ExpandoObject, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(result, (dynamic)table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableNameAsExpandoObjectWithFields()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<ExpandoObject, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                var entity = (dynamic)table;
                Assert.AreEqual(entity.RowGuid, result.RowGuid);
                Assert.AreEqual(entity.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableName()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table);

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaDynamicTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaExpandoObjectTableName()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(result, table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaExpandoObjectTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                var entity = (dynamic)table;
                Assert.AreEqual(entity.RowGuid, result.RowGuid);
                Assert.AreEqual(entity.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameForIdentityTable()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameForNonIdentityTable()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table);

                // Assert
                Assert.AreNotEqual(Guid.Empty, id);

                // Act
                var result = connection.Query<NonIdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var table = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>())?.FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertViaTableNameWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.Insert(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion

        #region InsertAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableNameAsDynamic()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<dynamic, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableNameAsDynamicWithFields()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<dynamic, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableNameAsExpandoObject()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<ExpandoObject, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(result, (dynamic)table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableNameAsExpandoObjectWithFields()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<ExpandoObject, long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                var entity = (dynamic)table;
                Assert.AreEqual(entity.RowGuid, result.RowGuid);
                Assert.AreEqual(entity.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableName()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaDynamicTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateDynamicIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaExpandoObjectTableName()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(result, table);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaExpandoObjectTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateExpandoObjectIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(id > 0);
                Assert.IsTrue(((dynamic)table).Id == id);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                var entity = (dynamic)table;
                Assert.AreEqual(entity.RowGuid, result.RowGuid);
                Assert.AreEqual(entity.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.RowGuid), nameof(IdentityTable.ColumnNVarChar))).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Assert.AreEqual(table.RowGuid, result.RowGuid);
                Assert.AreEqual(table.ColumnNVarChar, result.ColumnNVarChar);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameForIdentityTable()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameForNonIdentityTable()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<Guid>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table).Result;

                // Assert
                Assert.AreNotEqual(Guid.Empty, table.Id);

                // Act
                var result = connection.Query<NonIdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameNameWithIncompleteProperties()
        {
            // Setup
            var table = new { RowGuid = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync<long>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.IsTrue(id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(table, result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionInsertAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var id = connection.InsertAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.IsTrue(table.Id > 0);

                // Act
                var result = connection.Query<IdentityTable>(id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, result);
            }
        }

        #endregion
    }
}
