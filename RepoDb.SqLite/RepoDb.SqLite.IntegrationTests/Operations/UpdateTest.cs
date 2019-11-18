using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestUpdateViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
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
        public void TestUpdateViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
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
        public void TestUpdateAsyncViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
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
        public void TestUpdateAsyncViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
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
        public void TestUpdateAsyncViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
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
        public void TestUpdateViaTableNameViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestUpdateAsyncViaTableNameViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, new QueryField("Id", table.Id)).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryFields).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestUpdateAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var table = Database.CreateCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<CompleteTable>(), table, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<CompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion
    }
}
