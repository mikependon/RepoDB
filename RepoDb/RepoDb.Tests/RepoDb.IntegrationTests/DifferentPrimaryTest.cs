using System;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class DifferentPrimaryTest
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
        public void TestSqlConnectionInsertForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Assert
                Assert.AreEqual(entity.Id, insertResult);
                Assert.IsTrue(insertResult > 0);
                Assert.IsTrue(entity.Id > 0);
                Assert.AreEqual(1, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<IdentityTableWithDifferentPrimary>());

                // Act
                var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

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
        public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = connection.Delete<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForIdentityTableWithDifferentPrimaryViaPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary>(entity);

                // Act
                var deleteResult = connection.Delete<IdentityTableWithDifferentPrimary>(entity.RowGuid);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<IdentityTableWithDifferentPrimary>());
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForIdentityTableWithDifferentPrimary()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Act
                var queryResult = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region Update

        [TestMethod]
        public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaDataEntity()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<IdentityTableWithDifferentPrimary>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

                // Assert
                Assert.IsNotNull(data);
                Helper.AssertPropertiesEquality(entity, data);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForIdentityTableWithDifferentPrimaryViaPrimaryKey()
        {
            // Setup
            var entity = Helper.CreateIdentityTableWithDifferentPrimary();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<IdentityTableWithDifferentPrimary, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<IdentityTableWithDifferentPrimary>(entity, entity.RowGuid);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var data = connection.Query<IdentityTableWithDifferentPrimary>(entity.RowGuid).FirstOrDefault();

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
            var entities = Helper.CreateIdentityTableWithDifferentPrimaries(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTableWithDifferentPrimary>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = connection.UpdateAll<IdentityTableWithDifferentPrimary>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<IdentityTableWithDifferentPrimary>();

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
