using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

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
        public void TestSqlConnectionQueryAllAsyncWithEntityTableNameWithTypeResult()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<string>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.Parse<IdentityTable>(e => e.ColumnNVarChar)).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                Assert.IsTrue(result.All(e => string.IsNullOrEmpty(e) == false));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaEntityTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaEntityTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionQueryAllAsync()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>().Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaEntityTableNameAsDynamic()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaEntityTableNameAsExpandoObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => ((dynamic)item).Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaEntityTableNameAsDictionaryStringObject()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>()).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => (long)item["Id"] == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(fields: Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionQueryAllAsyncWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, result.FirstOrDefault(item => item.Id == table.Id));
                });
            }
        }

        #endregion

        #region QueryAllAsync<TEntity>(Extra Fields)

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncWithExtraFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<WithExtraFieldsIdentityTable>().Result;

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

        #endregion

        #region QueryAllAsync(TableName)

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncViaDynamicTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>()).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaDynamicTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync<dynamic>(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    Field.From(nameof(IdentityTable.Id), nameof(IdentityTable.ColumnNVarChar))).Result;

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
        public void TestSqlConnectionQueryAsyncAllViaTableName()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>()).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderBy()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    hints: SqlServerTableHints.NoLock).Result;

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
        public void TestSqlConnectionQueryAllAsyncViaTableNameWithOrderByAndWithHints()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            var orderBy = OrderField.Parse(new { Id = Order.Ascending });

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: orderBy,
                    hints: SqlServerTableHints.NoLock).Result;

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

        #endregion
    }
}
