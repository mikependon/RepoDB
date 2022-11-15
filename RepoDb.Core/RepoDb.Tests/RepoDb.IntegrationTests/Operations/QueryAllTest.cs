using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.IntegrationTests.Operations
{
    [TestClass]
    public class QueryAllTest
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

        #region QueryAll<TEntity>

        [TestMethod]
        public void TestSqlConnectionQueryAllWithEntityTableNameWithTypeResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<string>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.Parse<IdentityTable>(e => e.ColumnNVarChar));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                Assert.IsTrue(result.All(e => string.IsNullOrEmpty(e) == false));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaEntityTableNameAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<dynamic>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaEntityTableNameAsExpandoObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => ((dynamic)item).Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaEntityTableNameAsDictionaryStringObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => (long)item["Id"] == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAll()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSqlConnectionQueryAllWithInvalidOrderFields()
        {
            // Setup
            var orderBy = new OrderField("InvalidColumn", Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: orderBy.AsEnumerable());
            }
        }

        #endregion

        #region QueryAll<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryAllWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<WithExtraFieldsIdentityTable>();

                // Assert
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                });
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithEntityTableNameWithTypeResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<string>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.Parse<IdentityTable>(e => e.ColumnNVarChar));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                Assert.IsTrue(result.All(e => string.IsNullOrEmpty(e) == false));
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>();

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaEntityTableNameAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaEntityTableNameAsExpandoObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => ((dynamic)item).Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaEntityTableNameAsDictionaryStringObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => (long)item["Id"] == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    var entity = result.FirstOrDefault(item => item.Id == table.Id);
                    Assert.AreEqual(table.ColumnNVarChar, entity.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public async Task ThrowExceptionOnSqlConnectionQueryAllAsyncWithInvalidOrderFields()
        {
            // Setup
            var orderBy = new OrderField("InvalidColumn", Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(orderBy: orderBy.AsEnumerable());
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>(Extra Fields)

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<WithExtraFieldsIdentityTable>();

                // Assert
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Helper.AssertPropertiesEquality(entity, item);
                });
            }
        }

        #endregion

        #region QueryAll(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryAllViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<dynamic>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll<dynamic>(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSqlConnectionQueryAllViaTableNameWithInvalidOrderFields()
        {
            // Setup
            var orderBy = new OrderField("InvalidColumn", Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = connection.QueryAll<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy.AsEnumerable());
            }
        }

        #endregion

        #region QueryAllAsync(TableName)

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar)));

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>());

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod]
        public async Task TestSqlConnectionQueryAllAsyncViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = await connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var entity = tables.First(t => t.Id == item.Id);
                    Assert.AreEqual(entity.Id, item.Id);
                    Assert.AreEqual(entity.RowGuid, item.RowGuid);
                    Assert.AreEqual(entity.ColumnBit, item.ColumnBit);
                    Assert.AreEqual(entity.ColumnDateTime, item.ColumnDateTime);
                    Assert.AreEqual(entity.ColumnDateTime2, item.ColumnDateTime2);
                    Assert.AreEqual(entity.ColumnDecimal, item.ColumnDecimal);
                    Assert.AreEqual(entity.ColumnFloat, item.ColumnFloat);
                    Assert.AreEqual(entity.ColumnNVarChar, item.ColumnNVarChar);
                });
            }
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public async Task ThrowExceptionOnSqlConnectionQueryAllAsyncViaTableNameWithInvalidOrderFields()
        {
            // Setup
            var orderBy = new OrderField("InvalidColumn", Order.Descending);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var result = await connection.QueryAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy.AsEnumerable());
            }
        }

        #endregion
    }
}
