using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Models;
using RepoDb.MySql.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
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
        public void TestMySqlConnectionUpdateViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateAsyncViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaExpression()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table, e => e.Id == table.Id).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table, new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table, new QueryField("Id", table.Id)).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table, queryFields).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<CompleteTable>(table, queryGroup).Result;

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
        public void TestMySqlConnectionUpdateViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaExpandoObject()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                var entity = Helper.CreateCompleteTablesAsExpandoObjects(1).First();
                ((IDictionary<string, object>)entity)["Id"] = table.Id;

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    entity).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertMembersEquality(queryResult, entity);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id)).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<CompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var table = Database.CreateCompleteTables(1).First();
            var queryFields = new[]
            {
                new QueryField("Id", table.Id),
                new QueryField("ColumnInt", table.ColumnInt)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup).Result;

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
