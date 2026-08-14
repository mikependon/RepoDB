using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForBetweenOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.Between, new[] { 4, 6 });

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForNotBetweenOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotBetween, new[] { 4, 6 });

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForArrayContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new long[] { 4, 5 }).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForEmptyArrayContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new long[] { }).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForArrayContainsOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => values.Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForListContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new List<long>() { 4, 5 }).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForEmptyListContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new List<long>()).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForListContainsOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => values.Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStringContainsOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.Contains("NVARCHAR2"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStartsEndsWithOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.StartsWith("NVar"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStringEndsWithOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.EndsWith("CHAR1"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForArrayContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new long[] { 4, 5 }).Contains(item.Id) == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForArrayContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !(new long[] { 4, 5 }).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForArrayContainsAsNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => values.Contains(item.Id) == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForArrayContainsAsUnaryNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new long[] { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !values.Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForListContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => (new List<long>() { 4, 5 }).Contains(item.Id) == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForListContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !(new List<long>() { 4, 5 }).Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForListContainsAsNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => values.Contains(item.Id) == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForListContainsAsUnaryNotOperationViaVariable()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var values = new List<long>() { 4, 5 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !values.Contains(item.Id));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForStringContainsAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.Contains("NVARCHAR2") == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStringContainsAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !item.ColumnNVarChar.Contains("NVARCHAR2"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForStartsEndsWithAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.StartsWith("NVar") == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStartsEndsWithAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !item.ColumnNVarChar.StartsWith("NVar"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForStringEndsWithAsNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => item.ColumnNVarChar.EndsWith("CHAR1") == false);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForStringEndsWithAsUnaryNotOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(item => !item.ColumnNVarChar.EndsWith("CHAR1"));

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForInOperationViaArray()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.In, new[] { 4, 7 });

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForInOperationViaList()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var value = new List<int> { 4, 7 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.In, value);

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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
        public async Task TestSqlConnectionQueryAsyncForNotInOperationViaArray()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotIn, new[] { 4, 7 });

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForNotInOperationViaList()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);
            var value = new List<int> { 4, 7 };

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.Id), Operation.NotIn, value);

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForLikeOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.ColumnNVarChar), Operation.Like, "NVARCHAR1%"); // Matching: NVARCHAR1, NVARCHAR10

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

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

            using (var connection = new SqlConnection(Database.ConnectionString))
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

        [TestMethod]
        public async Task TestSqlConnectionQueryAsyncForNotLikeOperation()
        {
            // Setup
            var entities = Helper.CreateIdentityTables(10);

            using (var connection = new SqlConnection(Database.ConnectionString))
            {
                // Prepare
                var field = new QueryField(nameof(IdentityTable.ColumnNVarChar), Operation.NotLike, "NVARCHAR1%"); // Not Matching: NVARCHAR1, NVARCHAR10

                // Act
                await connection.InsertAllAsync<IdentityTable>(entities);

                // Act
                var queryResult = await connection.QueryAsync<IdentityTable>(field);

                // Assert
                Assert.AreEqual(8, queryResult.Count());
                queryResult.AsList().ForEach(item => Helper.AssertPropertiesEquality(entities.First(entity => entity.Id == item.Id), item));
            }
        }

        #endregion
    }
}
