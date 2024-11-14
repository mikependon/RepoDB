﻿using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class BaseEnferredInheritanceTest
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

        #region Delete

        [TestMethod]
        public void TestSqlConnectionDeleteForInheritedViaDataEntity()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<Entity<InheritedIdentityTable>>(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        [TestMethod]
        public void TestSqlConnectionDeleteForInheritedViaPrimary()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert(entity);

                // Act
                var deleteResult = connection.Delete<InheritedIdentityTable>(entity.Id);

                // Assert
                Assert.IsTrue(deleteResult > 0);
                Assert.AreEqual(0, connection.CountAll<InheritedIdentityTable>());
            }
        }

        #endregion

        #region Insert

        [TestMethod]
        public void TestSqlConnectionInsertForInherited()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Insert<Entity<InheritedIdentityTable>, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region InsertAll

        [TestMethod]
        public void TestSqlConnectionInsertAllForInherited()
        {
            // Setup
            // warning CA2021: This call will always result in an empty sequence because type 'RepoDb.IntegrationTests.Models.InheritedIdentityTable' is incompatible with type 'RepoDb.IntegrationTests.Models.Entity<RepoDb.IntegrationTests.Models.InheritedIdentityTable>' (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021)
            var entities = Helper.CreateInheritedIdentityTables(10)
                .OfType<Entity<InheritedIdentityTable>>().ToList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<Entity<InheritedIdentityTable>>(entities);

                // Assert
                Assert.AreEqual(entities.Count, insertAllResult);
                Assert.AreEqual(entities.Count, connection.CountAll<InheritedIdentityTable>());

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Merge

        [TestMethod]
        public void TestSqlConnectionMergeForInherited()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeResult = connection.Merge<Entity<InheritedIdentityTable>, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeForInheritedWithNonEmptyTable()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertResult = connection.Merge<Entity<InheritedIdentityTable>, long>(entity);

                // Assert
                Assert.IsTrue(insertResult > 0);
                Assert.AreEqual(entity.Id, insertResult);
                Assert.AreEqual(1, connection.CountAll<InheritedIdentityTable>());

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var mergeResult = connection.Merge<Entity<InheritedIdentityTable>, long>(entity);

                // Assert
                Assert.IsTrue(mergeResult > 0);
                Assert.AreEqual(entity.Id, mergeResult);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region MergeAll

        [TestMethod]
        public void TestSqlConnectionMergeAllForInherited()
        {
            // Setup
            // warning CA2021: This call will always result in an empty sequence because type 'RepoDb.IntegrationTests.Models.InheritedIdentityTable' is incompatible with type 'RepoDb.IntegrationTests.Models.Entity<RepoDb.IntegrationTests.Models.InheritedIdentityTable>' (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021)
            var entities = Helper.CreateInheritedIdentityTables(10)
                .OfType<Entity<InheritedIdentityTable>>().ToList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var mergeAllRequest = connection.MergeAll<Entity<InheritedIdentityTable>>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllRequest);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        [TestMethod]
        public void TestSqlConnectionMergeAllForInheritedWithNonEmptyTables()
        {
            // Setup
            // warning CA2021: This call will always result in an empty sequence because type 'RepoDb.IntegrationTests.Models.InheritedIdentityTable' is incompatible with type 'RepoDb.IntegrationTests.Models.Entity<RepoDb.IntegrationTests.Models.InheritedIdentityTable>' (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021)
            var entities = Helper.CreateInheritedIdentityTables(10)
                .OfType<Entity<InheritedIdentityTable>>().ToList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                var insertAllResult = connection.InsertAll<Entity<InheritedIdentityTable>>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var mergeAllResult = connection.MergeAll<Entity<InheritedIdentityTable>>(entities);

                // Assert
                Assert.AreEqual(entities.Count, mergeAllResult);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryForInherited()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<Entity<InheritedIdentityTable>, long>(entity);

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
        public void TestSqlConnectionUpdateForInheritedViaDataEntity()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<Entity<InheritedIdentityTable>, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<Entity<InheritedIdentityTable>>(entity);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        [TestMethod]
        public void TestSqlConnectionUpdateForInheritedViaPrimaryKey()
        {
            // Setup
            Entity<InheritedIdentityTable> entity = Helper.CreateInheritedIdentityTable();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.Insert<Entity<InheritedIdentityTable>, long>(entity);

                // Setup
                entity.ColumnBit = false;
                entity.ColumnDateTime2 = DateTime.UtcNow;

                // Act
                var updateResult = connection.Update<Entity<InheritedIdentityTable>>(entity, entity.Id);

                // Assert
                Assert.IsTrue(updateResult > 0);

                // Act
                var queryResult = connection.Query<InheritedIdentityTable>(entity.Id).FirstOrDefault();

                // Assert
                Assert.IsNotNull(queryResult);
                Helper.AssertPropertiesEquality(entity, queryResult);
            }
        }

        #endregion

        #region UpdateAll

        [TestMethod]
        public void TestSqlConnectionUpdateAllForInherited()
        {
            // Setup
            // warning CA2021: This call will always result in an empty sequence because type 'RepoDb.IntegrationTests.Models.InheritedIdentityTable' is incompatible with type 'RepoDb.IntegrationTests.Models.Entity<RepoDb.IntegrationTests.Models.InheritedIdentityTable>' (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca2021)
            var entities = Helper.CreateInheritedIdentityTables(10)
                .OfType<Entity<InheritedIdentityTable>>().ToList();

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<Entity<InheritedIdentityTable>>(entities);

                // Setup
                entities.ForEach(entity =>
                {
                    entity.ColumnBit = false;
                    entity.ColumnDateTime2 = DateTime.UtcNow;
                });

                // Act
                var updateAllResult = connection.UpdateAll<Entity<InheritedIdentityTable>>(entities);

                // Assert
                Assert.AreEqual(entities.Count, updateAllResult);

                // Act
                var queryResult = connection.QueryAll<InheritedIdentityTable>();

                // Assert
                Assert.AreEqual(entities.Count, queryResult.Count());
                entities.ForEach(entity =>
                    Helper.AssertPropertiesEquality(entity, queryResult.First(e => e.Id == entity.Id)));
            }
        }

        #endregion
    }
}
