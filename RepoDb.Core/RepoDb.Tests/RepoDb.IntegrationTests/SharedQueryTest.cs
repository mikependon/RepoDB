using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class SharedQueryTest
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

        #region Query

        /*
         * IdentityTable
         */

        [TestMethod]
        public void TestSqlConnectionSharedQueryForIdentityTable()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.ColumnInt == item.ColumnInt);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForIdentityTableViaQueryField()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    new { tables.Last().Id }).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForIdentityTableViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    (new QueryField("Id", tables.Last().Id)).AsEnumerable()).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForIdentityTableViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateIdentityTables(10);
            
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var queryFields = new[]
                {
                    new QueryField("Id", tables.First().Id),
                    new QueryField("Id", tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields, Conjunction.Or);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<IdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.ColumnInt == item.ColumnInt);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        /*
         * NonIdentity
         */

        [TestMethod]
        public void TestSqlConnectionSharedQueryForNonIdentityTable()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.ColumnInt == item.ColumnInt);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForNonIdentityTableViaQueryField()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    new { tables.Last().Id }).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForNonIdentityTableViaQueryFields()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    (new QueryField("Id", tables.Last().Id)).AsEnumerable()).FirstOrDefault();

                // Assert
                Helper.AssertPropertiesEquality(tables.Last(), result);
            }
        }

        [TestMethod]
        public void TestSqlConnectionSharedQueryForNonIdentityTableViaQueryGroup()
        {
            // Setup
            var tables = Helper.CreateNonIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll(tables);

                // Setup
                var queryFields = new[]
                {
                    new QueryField("Id", tables.First().Id),
                    new QueryField("Id", tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields, Conjunction.Or);

                // Act
                var result = connection.Query<SharedIdentityTable>(ClassMappedNameCache.Get<NonIdentityTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item =>
                {
                    var target = tables.First(t => t.ColumnInt == item.ColumnInt);
                    Helper.AssertPropertiesEquality(target, item);
                });
            }
        }

        #endregion
    }
}
