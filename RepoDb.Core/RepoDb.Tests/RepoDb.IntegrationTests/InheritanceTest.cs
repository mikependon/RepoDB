using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class InheritanceTest
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

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForInheritance()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<InheritedIdentityTable, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, insertResult);
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(entity.Id > 0);
                Assert.AreEqual(1, connection.CountAll<InheritedIdentityTable>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForInheritance()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<InheritedIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        #endregion

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForInheritanceViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForInheritanceViaPrimary()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<InheritedIdentityTable>(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForInheritance()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForInheritanceViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForInheritanceViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<InheritedIdentityTable, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<InheritedIdentityTable>(entity, entity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForIdentityTableWithDifferentPrimaries()
        {
            // Setup
            var entities = Helper.CreateInheritedIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<InheritedIdentityTable>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = connection.UpdateAll<InheritedIdentityTable>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity,
                        queryResult.ElementAt(entities.IndexOf(entity))));
            }
        }

        #endregion
    }
}
