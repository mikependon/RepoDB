using Microsoft.VisualStudio.TestTools.UnitTesting;
using EnterpriseDB.EDBClient;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public void TestEnterpriseDbConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInteger", table.ColumnInteger)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
