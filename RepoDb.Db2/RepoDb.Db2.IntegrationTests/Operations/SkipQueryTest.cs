using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Db2.IntegrationTests.Models;
using RepoDb.Db2.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Db2.IntegrationTests.Operations
{
    [TestClass]
    public class SkipQueryTest
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
        public void TestDb2ConnectionSkipQueryFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act: exercises the "OFFSET x ROWS FETCH NEXT y ROWS ONLY" override.
                var result = connection.SkipQuery<CompleteTable>(
                    0,
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
        public void TestDb2ConnectionSkipQueryFirstBatchAscendingWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = connection.SkipQuery<CompleteTable>(
                        0,
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
        public void TestDb2ConnectionSkipQueryFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
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
        public void TestDb2ConnectionSkipQueryThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
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
        public void TestDb2ConnectionSkipQueryThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    6,
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
        public void TestDb2ConnectionSkipQueryViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var last5Ids = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = connection.SkipQuery<CompleteTable>(
                    0,
                    2,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    e => last5Ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Assert.IsTrue(last5Ids.Contains(item.Id)));
            }
        }

        [TestMethod]
        public void TestDb2ConnectionSkipQueryWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Db2 - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.SkipQuery<CompleteTable>(
                        0,
                        3,
                        OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                        where: (object)null,
                        hints: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestDb2ConnectionSkipQueryAsyncFirstBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
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
        public async Task TestDb2ConnectionSkipQueryAsyncFirstBatchAscendingWithAutomaticConversion()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                GlobalConfiguration.Options.ConversionType = ConversionType.Automatic;
                try
                {
                    // Act
                    var result = await connection.SkipQueryAsync<CompleteTable>(
                        0,
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
        public async Task TestDb2ConnectionSkipQueryAsyncFirstBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
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
        public async Task TestDb2ConnectionSkipQueryAsyncThirdBatchAscending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
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
        public async Task TestDb2ConnectionSkipQueryAsyncThirdBatchDescending()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    6,
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
        public async Task TestDb2ConnectionSkipQueryAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var last5Ids = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SkipQueryAsync<CompleteTable>(
                    0,
                    2,
                    OrderField.Ascending<CompleteTable>(c => c.Id).AsEnumerable(),
                    e => last5Ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(2, result.Count());
                result.AsList().ForEach(item => Assert.IsTrue(last5Ids.Contains(item.Id)));
            }
        }

        [TestMethod]
        public async Task TestDb2ConnectionSkipQueryAsyncWithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new DB2Connection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.SkipQueryAsync<CompleteTable>(
                        0,
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
