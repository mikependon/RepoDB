using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
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
        public void TestSqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, c => c.Id == item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update<IdentityTable>(item,
                        c => c.ColumnFloat == item.ColumnFloat && c.ColumnNVarChar == item.ColumnNVarChar);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(last, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(item, hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region Update<TEntity>(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, new { entity.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.Update(entity, c => c.Id == entity.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.Update(entity, queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region UpdateAsync<TEntity>

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, c => c.Id == item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }
        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaExpressionNonPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item,
                        c => c.ColumnFloat == item.ColumnFloat && c.ColumnNVarChar == item.ColumnNVarChar).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(last, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaDataEntityWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(item, hints: SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        #endregion

        #region UpdateAsync<TEntity(With Extra Fields)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, new { entity.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaExpression()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(item);
                    var affectedRows = connection.UpdateAsync(entity, c => c.Id == entity.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncWithExtraFieldViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();

                // Setup
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var entity = Helper.ConverToType<WithExtraFieldsIdentityTable>(last);
                var affectedRows = connection.UpdateAsync(entity, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        #endregion

        #region Update(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        item,
                        item.Id);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                        item,
                        new { item.Id });

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.Update(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameForNonIdentitySingleEntityForEmptyTable()
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
        public void TestSqlConnectionUpdateViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data,
                        hints: SqlServerTableHints.TabLock);

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(KeyFieldNotFoundException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfThereIsNoKeyField()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    ColumnInt = 1,
                    ColumnDecimal = 2
                };
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqlConnectionUpdateViaTableNameIfTheFieldsAreNotFound()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                var data = new
                {
                    Id = 1,
                    AnyField = 1
                };
                connection.Update(ClassMappedNameCache.Get<NonIdentityTable>(), data);
            }
        }

        #endregion

        #region UpdateAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableName()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaPrimaryKey()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                    item,
                    item.Id).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    item.ColumnBit = false;
                    item.ColumnInt = item.ColumnInt * 100;
                    item.ColumnDecimal = item.ColumnDecimal * 100;

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    item,
                    new { item.Id }).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(item => Helper.AssertPropertiesEquality(item, result.First(e => e.Id == item.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var field = new QueryField(nameof(IdentityTable.ColumnInt), 10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnBit = false;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    field).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                field.Reset();
                var result = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    fields).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                fields.ResetAll();
                var result = connection.Query<IdentityTable>(fields);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateViaTableNameAsyncViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var fields = new[]
            {
                new QueryField(nameof(IdentityTable.ColumnBit), true),
                new QueryField(nameof(IdentityTable.ColumnInt), 10)
            };
            var queryGroup = new QueryGroup(fields);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var last = tables.Last();
                last.ColumnFloat = last.ColumnFloat * 100;
                last.ColumnDateTime2 = DateTime.UtcNow;
                last.ColumnDecimal = last.ColumnDecimal * 100;

                // Act
                var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    last,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(1, affectedRows);

                // Act
                queryGroup.Reset();
                var result = connection.Query<IdentityTable>(queryGroup);

                // Assert
                Assert.AreEqual(1, result.Count());
                Helper.AssertPropertiesEquality(last, result.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameForNonIdentityEmptyTableWithIncompleteProperties()
        {
            // Setup
            var item = new { Id = Guid.NewGuid(), ColumnBit = true, ColumnInt = 1 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(ClassMappedNameCache.Get<NonIdentityTable>(), item);

                // Act
                var updateResult = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), item).Result;

                // Assert
                Assert.AreEqual(1, updateResult);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<NonIdentityTable>(), item.Id).First();

                // Assert
                Helper.AssertMembersEquality(item, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                tables.ForEach(item =>
                {
                    // Set Values
                    var data = new
                    {
                        Id = item.Id,
                        ColumnBit = false,
                        ColumnInt = item.ColumnInt * 100,
                        ColumnDecimal = item.ColumnDecimal * 100
                    };

                    // Update each
                    var affectedRows = connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(),
                        data,
                        hints: SqlServerTableHints.TabLock).Result;

                    // Assert
                    Assert.AreEqual(1, affectedRows);
                });
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
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
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
                connection.UpdateAsync(ClassMappedNameCache.Get<NonIdentityTable>(), data).Wait();
            }
        }

        #endregion
    }
}
