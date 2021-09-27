using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionUpdateViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table, e => e.Id == table.Id);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.Update<SdsCompleteTable>(table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table, e => e.Id == table.Id).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table, new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table, new QueryField("Id", table.Id)).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table, queryFields).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync<SdsCompleteTable>(table, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query<SdsCompleteTable>(table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameAsExpandoObjectViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), result).First();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(), table);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(), table, new { table.Id });

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(), table, new QueryField("Id", table.Id));

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(), table, queryFields);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = DbConnectionExtension.Update(connection, ClassMappedNameCache.Get<SdsCompleteTable>(), table, queryGroup);

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameAsExpandoObjectViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                Database.CreateSdsCompleteTables(1, connection).First();
                var table = Helper.CreateSdsCompleteTablesAsExpandoObjects(1).First();

                // Act
                var result = DbConnectionExtension.UpdateAsync(connection, ClassMappedNameCache.Get<SdsCompleteTable>(),
                    table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), result).First();

                // Assert
                Helper.AssertMembersEquality(queryResult, table);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameViaDataEntity()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), table).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), table, new { table.Id }).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), table, new QueryField("Id", table.Id)).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), table, queryFields).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionUpdateAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var table = Database.CreateSdsCompleteTables(1, connection).First();
                var queryFields = new[]
                {
                    new QueryField("Id", table.Id),
                    new QueryField("ColumnInt", table.ColumnInt)
                };
                var queryGroup = new QueryGroup(queryFields);
                Helper.UpdateSdsCompleteTableProperties(table);

                // Act
                var result = connection.UpdateAsync(ClassMappedNameCache.Get<SdsCompleteTable>(), table, queryGroup).Result;

                // Assert
                Assert.AreEqual(1, result);

                // Act
                var queryResult = connection.Query(ClassMappedNameCache.Get<SdsCompleteTable>(), table.Id).First();

                // Assert
                Helper.AssertPropertiesEquality(table, queryResult);
            }
        }

        #endregion

        #endregion
    }
}
