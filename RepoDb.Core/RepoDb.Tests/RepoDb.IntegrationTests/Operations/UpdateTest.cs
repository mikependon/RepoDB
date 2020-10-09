using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class UpdateTest
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

        #region Update<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntityTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntityTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntityWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    table.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    new { table.Id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpression()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    c => c.Id == table.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpressionNonPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    c => c.ColumnFloat == table.ColumnFloat && c.ColumnNVarChar == table.ColumnNVarChar);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntityWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update<IdentityTable>(table,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Update<TEntity>(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity,
                    entity.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity,
                    new { entity.Id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaExpression()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity,
                    c => c.Id == entity.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.Update<WithExtraFieldsIdentityTable>(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region UpdateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntityTableName()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntityTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal))).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntityWithFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnBit), nameof(IdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal))).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    table.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    c => c.Id == table.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }
        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpressionNonPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    c => c.ColumnFloat == table.ColumnFloat && c.ColumnNVarChar == table.ColumnNVarChar).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntityWithHints()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync<IdentityTable>(table,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region UpdateAsync<TEntity(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    entity.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    new { entity.Id }).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaExpression()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    c => c.Id == entity.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(table);
                var affectedRows = connection.UpdateAsync<WithExtraFieldsIdentityTable>(entity,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Update<IdentityTable>(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamicTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(result.ColumnBit, data.ColumnBit);
                Assert.AreEqual(result.ColumnInt, data.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, data.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamicTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                var entity = (dynamic)data;
                Assert.AreEqual(result.ColumnBit, entity.ColumnBit);
                Assert.AreEqual(result.ColumnInt, entity.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, entity.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpandoObjectTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                data["Id"] = table.Id;

                // Act
                var affectedRows = connection.Update<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(result, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpandoObjectTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                data["Id"] = table.Id;

                // Act
                var affectedRows = connection.Update<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                var entity = (dynamic)data;
                Assert.AreEqual(result.ColumnBit, entity.ColumnBit);
                Assert.AreEqual(result.ColumnInt, entity.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, entity.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data);

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal)));

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamicAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    table.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table,
                    table.Id);

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    new { table.Id });

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameForNonIdentityForEmptyTable()
        {
            // Setup
            var table = Helper.CreateDynamicNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), (object)table);

                // Act
                var updateResult = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)table);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (Guid)table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var table = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), table);

                // Act
                var updateResult = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameWithHints()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    hints: SqlServerTableHints.TabLock);

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionUpdateIfThereIsNoKeyField()
        {
            // Setup
            var data = Helper.CreateNonKeyedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Update<NonKeyedTable>(data);
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfThereIsNoKeyField()
        {
            // Setup
            var data = Helper.CreateDynamicNonKeyedTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), (object)data);
            }
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfTheFieldsAreNotFound()
        {
            // Setup
            var data = new
            {
                Id = 1,
                AnyField = 1
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        #endregion

        #region UpdateAsync<IdentityTable>(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamicTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                Assert.AreEqual(result.ColumnBit, data.ColumnBit);
                Assert.AreEqual(result.ColumnInt, data.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, data.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamicTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal))).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                var entity = (dynamic)data;
                Assert.AreEqual(result.ColumnBit, data.ColumnBit);
                Assert.AreEqual(result.ColumnInt, data.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, data.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpandoObjectTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                data["Id"] = table.Id;

                // Act
                var affectedRows = connection.UpdateAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                Helper.AssertMembersEquality(result, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpandoObjectTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = Helper.CreateExpandoObjectNonIdentityTable() as IDictionary<string, object>;
                data["Id"] = table.Id;

                // Act
                var affectedRows = connection.UpdateAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal))).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Setup
                var result = connection.Query<NonIdentityTable>(table.Id).FirstOrDefault();

                // Assert
                var entity = (dynamic)data;
                Assert.AreEqual(result.ColumnBit, entity.ColumnBit);
                Assert.AreEqual(result.ColumnInt, entity.ColumnInt);
                Assert.AreEqual(result.ColumnDecimal, entity.ColumnDecimal);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableName()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameWithFields()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    fields: Field.From(nameof(NonIdentityTable.Id), nameof(NonIdentityTable.ColumnBit), nameof(NonIdentityTable.ColumnInt), nameof(IdentityTable.ColumnDecimal))).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamicAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync<object>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    table.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaAsyncViaTableNameViaPrimaryKey()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                table,
                table.Id).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnInt = table.ColumnInt * 100;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                var queryResult = connection.Query<IdentityTable>(table.Id)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnBit = false;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var field = new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt);

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var queryResult = connection.Query<IdentityTable>(field)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var queryResult = connection.Query<IdentityTable>(fields)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Helper.CreateIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                table.ColumnFloat = table.ColumnFloat * 100;
                table.ColumnDateTime2 = DateTime.UtcNow;
                table.ColumnDecimal = table.ColumnDecimal * 100;

                // Setup
                var fields = new[]
                {
                    new QueryField(nameof(IdentityTable.ColumnBit), table.ColumnBit),
                    new QueryField(nameof(IdentityTable.ColumnInt), table.ColumnInt)
                };
                var queryGroup = new QueryGroup(fields);

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    table,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var queryResult = connection.Query<IdentityTable>(queryGroup)?.FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var table = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), table);

                // Act
                var updateResult = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(),
                    table.Id).First();

                // Assert
                Helper.AssertMembersEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameWithHints()
        {
            // Setup
            var table = Helper.CreateNonIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(table);

                // Setup
                var data = new
                {
                    table.Id,
                    ColumnBit = false,
                    ColumnInt = table.ColumnInt * 100,
                    ColumnDecimal = table.ColumnDecimal * 100
                };

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data,
                    hints: SqlServerTableHints.TabLock).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionUpdateAsyncViaTableNameIfThereIsNoKeyField()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data).Wait();
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqlConnectionUpdateAsyncViaTableNameIfTheFieldsAreNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    data).Wait();
            }
        }

        #endregion
    }
}
