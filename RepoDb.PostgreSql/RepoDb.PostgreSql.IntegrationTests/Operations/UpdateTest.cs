using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.PostgreSql.IntegrationTests.Models;
using RepoDb.PostgreSql.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.PostgreSql.IntegrationTests.Operations
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

        #region DataEntity

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update<CompleteTable>(table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync<CompleteTable>(table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.Update(ClassMappedNameCache.Get<CompleteTable>(),
                    table,
                    queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public async Task TestPostgreSqlConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new NpgsqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = await connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion
    }
}
