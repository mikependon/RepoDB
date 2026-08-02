using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    [TestClass]
    public class BatchQueryTest
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
        public void TestOracleConnectionBatchQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act: exercises the "OFFSET x ROWS FETCH NEXT y ROWS ONLY" override.
                var result = connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryFirstBatchAscendingWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.BatchQuery<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null);

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                    Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var last5Ids = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.BatchQuery<CompleteTable>(0,
                    2,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    e => last5Ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Assert.IsTrue(last5Ids.Contains(item.Id)));
            }
        }

        [TestMethod]
        public void TestOracleConnectionBatchQueryWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.BatchQuery<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        where: (object)null,
                        hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncFirstBatchAscendingWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.BatchQueryAsync<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        (object)null);

                    // Assert
                    Assert.AreEqual(3, result.Count());
                    Helper.AssertPropertiesEquality(tables.ElementAt(0), result.ElementAt(0));
                    Helper.AssertPropertiesEquality(tables.ElementAt(2), result.ElementAt(2));
                }
                finally
                {
                    GlobalConfiguration.Options.ConversionType = ConversionType.Default;
                }
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(9), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(7), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(6), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(8), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(2,
                    3,
                    OrderField.Descending<CompleteTable>(c => c.Id).AsEnumerable(),
                    (object)null);

                // Assert
                Assert.AreEqual(3, result.Count());
                Helper.AssertPropertiesEquality(tables.ElementAt(3), result.ElementAt(0));
                Helper.AssertPropertiesEquality(tables.ElementAt(1), result.ElementAt(2));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var last5Ids = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.BatchQueryAsync<CompleteTable>(0,
                    2,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    e => last5Ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Assert.IsTrue(last5Ids.Contains(item.Id)));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionBatchQueryAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.BatchQueryAsync<CompleteTable>(0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        where: (object)null,
                        hints: "NOLOCK"));
            }
        }

        #endregion

        #endregion
    }
}
