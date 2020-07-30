using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class SpecialOperationTest
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

        #region Between

        [TestMethod]
        public void TestSqlConnectionQueryForBetweenOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.Between, new[] { 4, 6 });

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(3, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region NotBetween

        [TestMethod]
        public void TestSqlConnectionQueryForNotBetweenOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotBetween, new[] { 4, 6 });

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(7, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region Contains/StartsWith/EndsWith

        #region True

        #region Array.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new long[] { 4, 5 }).Contains(item.Id));

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForEmptyArrayContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new long[] { }).Contains(item.Id));

                // Assert
                Assert.AreEqual(0, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => values.Contains(item.Id));

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region List.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new List<long>() { 4, 5 }).Contains(item.Id));

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForEmptyListContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new List<long>()).Contains(item.Id));

                // Assert
                Assert.AreEqual(0, queryResult.Count());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => values.Contains(item.Id));

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region String.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForStringContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.Contains("NVARCHAR2"));

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        #endregion

        #region String.StartsWith

        [TestMethod]
        public void TestSqlConnectionQueryForStartsEndsWithOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.StartsWith("NVar"));

                // Assert
                Assert.AreEqual(10, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region String.EndsWith

        [TestMethod]
        public void TestSqlConnectionQueryForStringEndsWithOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.EndsWith("CHAR1"));

                // Assert
                Assert.AreEqual(1, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        #endregion

        #endregion

        #region False

        #region Array.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new long[] { 4, 5 }).Contains(item.Id) == false);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !(new long[] { 4, 5 }).Contains(item.Id));

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsAsNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => values.Contains(item.Id) == false);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForArrayContainsAsUnaryNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !values.Contains(item.Id));

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region List.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => (new List<long>() { 4, 5 }).Contains(item.Id) == false);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !(new List<long>() { 4, 5 }).Contains(item.Id));

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsAsNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => values.Contains(item.Id) == false);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForListContainsAsUnaryNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !values.Contains(item.Id));

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region String.Contains

        [TestMethod]
        public void TestSqlConnectionQueryForStringContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.Contains("NVARCHAR2") == false);

                // Assert
                Assert.AreEqual(9, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForStringContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !item.ColumnNVarChar.Contains("NVARCHAR2"));

                // Assert
                Assert.AreEqual(9, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        #endregion

        #region String.StartsWith

        [TestMethod]
        public void TestSqlConnectionQueryForStartsEndsWithAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.StartsWith("NVar") == false);

                // Assert
                Assert.AreEqual(0, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForStartsEndsWithAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !item.ColumnNVarChar.StartsWith("NVar"));

                // Assert
                Assert.AreEqual(0, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region String.EndsWith

        [TestMethod]
        public void TestSqlConnectionQueryForStringEndsWithAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => item.ColumnNVarChar.EndsWith("CHAR1") == false);

                // Assert
                Assert.AreEqual(9, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForStringEndsWithAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(item => !item.ColumnNVarChar.EndsWith("CHAR1"));

                // Assert
                Assert.AreEqual(9, queryResult.Count());
                Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == queryResult.First().Id), queryResult.First());
            }
        }

        #endregion

        #endregion

        #endregion

        #region In

        [TestMethod]
        public void TestSqlConnectionQueryForInOperationViaArray()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.In, new[] { 4, 7 });

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForInOperationViaList()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var value = new List<int> { 4, 7 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.In, value);

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region NotIn

        [TestMethod]
        public void TestSqlConnectionQueryForNotInOperationViaArray()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotIn, new[] { 4, 7 });

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryForNotInOperationViaList()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var value = new List<int> { 4, 7 };

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotIn, value);

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region Like

        [TestMethod]
        public void TestSqlConnectionQueryForLikeOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.ColumnNVarChar), Operation.Like, "NVARCHAR1%"); // Matching: NVARCHAR1, NVARCHAR10

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(2, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion

        #region NoLike

        [TestMethod]
        public void TestSqlConnectionQueryForNotLikeOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.ColumnNVarChar), Operation.NotLike, "NVARCHAR1%"); // Not Matching: NVARCHAR1, NVARCHAR10

                // Act
                connection.InsertAll<IdentityTable>(entities);

                // Act
                var queryResult = connection.Query<IdentityTable>(field);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion
    }
}
