using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Data.SqlClient;
using RepoDb.SqlServer.IntegrationTests.Models;
using RepoDb.SqlServer.IntegrationTests.Setup;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.SqlServer.IntegrationTests.Operations
{
    [TestClass]
    public class InsertAllTest
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
        public void TestPostgreSqlConnectionInsertAllForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<CompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll<NonIdentityCompleteTable>(tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync<CompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
                Assert.IsTrue(tables.All(table => table.Id > 0));

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync<NonIdentityCompleteTable>(tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertPropertiesEquality(table, queryResult.First(item => item.Id == table.Id));
                });
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<CompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAll(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables);

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameAsyncForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table,
                        queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsDynamicsForIdentity()
        {
            // Setup
            var tables = Helper.CreateCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<CompleteTable>();

                // Assert
                tables.ForEach(table =>
                {
                    Helper.AssertMembersEquality(table, 
                        queryResult.OrderBy(e => e.Id).ElementAt((int)tables.IndexOf(table)));
                });
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllViaTableNameAsyncForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        [TestMethod]
        public void TestPostgreSqlConnectionInsertAllAsyncViaTableNameAsDynamicsForNonIdentity()
        {
            // Setup
            var tables = Helper.CreateNonIdentityCompleteTablesAsDynamics(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.InsertAllAsync(ClassMappedNameCache.Get<NonIdentityCompleteTable>(),
                    tables).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);

                // Act
                var queryResult = connection.QueryAll<NonIdentityCompleteTable>();

                // Assert
                tables.ForEach(table => Helper.AssertMembersEquality(table, queryResult.First(e => e.Id == table.Id)));
            }
        }

        #endregion

        #endregion
    }
}
