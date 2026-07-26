using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.Extensions;
using RepoDb.Oracle.IntegrationTests.Models;
using RepoDb.Oracle.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Oracle.IntegrationTests.Operations
{
    /// <summary>
    /// NOTE: OracleDbSetting.IsMultiStatementExecutable is false, so QueryMultiple/QueryMultipleAsync
    /// transparently perform one round-trip per requested type instead of a single combined command.
    /// That is fully transparent to the calls below - they use the exact same public API shape as
    /// every other provider. All arities (T2 through T7) are covered here; TransactionTests.cs also
    /// exercises T2 through T7 within a transaction, but the scenarios below are independent of that.
    ///
    /// This project only has a single entity/table (CompleteTable) - unlike the SqlServer reference,
    /// which queries two distinct entity types for its T2 case. Here, CompleteTable is used for every
    /// type parameter, differentiated only by the per-type "top" value (and, for T4, an additional
    /// narrowing predicate) - each resultset is still independently verifiable via Item1/Item2/etc.
    /// </summary>
    [TestClass]
    public class QueryMultipleTest
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
        public void TestOracleConnectionQueryMultipleForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5,
                    top6: 6);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                Assert.AreEqual(6, result.Item6.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item6.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5,
                    top6: 6,
                    top7: 7);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                Assert.AreEqual(6, result.Item6.Count());
                Assert.AreEqual(7, result.Item7.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item6.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item7.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT2ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT3ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT4ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT5ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT6ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Assert.AreEqual(1, result.Item6.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
                Helper.AssertPropertiesEquality(tables[5], result.Item6.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT7ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    e => e.Id == tables[6].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Assert.AreEqual(1, result.Item6.Count());
                Assert.AreEqual(1, result.Item7.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
                Helper.AssertPropertiesEquality(tables[5], result.Item6.First());
                Helper.AssertPropertiesEquality(tables[6], result.Item7.First());
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT2DifferentFilters()
        {
            // Setup: a single table, so "multiple different queries" is expressed as two disjoint
            // filters (on ColumnInt, which is safe to filter/order on per this project's LOB/XMLType
            // restrictions) rather than two different entity types.
            var tables = Database.CreateCompleteTables(10).ToList();
            var firstHalfIds = tables.Take(5).Select(t => t.Id).ToArray();
            var secondHalfIds = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable>(e => firstHalfIds.Contains(e.Id),
                    e => secondHalfIds.Contains(e.Id));

                // Assert
                Assert.AreEqual(5, result.Item1.Count());
                Assert.AreEqual(5, result.Item2.Count());
                result.Item1.AsList().ForEach(item => Assert.IsTrue(firstHalfIds.Contains(item.Id)));
                result.Item2.AsList().ForEach(item => Assert.IsTrue(secondHalfIds.Contains(item.Id)));
            }
        }

        [TestMethod]
        public void TestOracleConnectionQueryMultipleForT2WithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert: AreTableHintsSupported == false for Oracle - any non-null/non-whitespace
                // "hints" argument must throw, rather than silently being ignored.
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id > 0,
                        e => e.Id > 0,
                        top1: 1,
                        top2: 2,
                        hints1: "NOLOCK",
                        hints2: "NOLOCK"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5,
                    top6: 6);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                Assert.AreEqual(6, result.Item6.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item6.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5,
                    top6: 6,
                    top7: 7);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                Assert.AreEqual(3, result.Item3.Count());
                Assert.AreEqual(4, result.Item4.Count());
                Assert.AreEqual(5, result.Item5.Count());
                Assert.AreEqual(6, result.Item6.Count());
                Assert.AreEqual(7, result.Item7.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item3.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item4.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item5.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item6.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item7.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT2ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT3ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT4ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT5ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT6ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Assert.AreEqual(1, result.Item6.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
                Helper.AssertPropertiesEquality(tables[5], result.Item6.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT7ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    e => e.Id == tables[6].Id);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Assert.AreEqual(1, result.Item3.Count());
                Assert.AreEqual(1, result.Item4.Count());
                Assert.AreEqual(1, result.Item5.Count());
                Assert.AreEqual(1, result.Item6.Count());
                Assert.AreEqual(1, result.Item7.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
                Helper.AssertPropertiesEquality(tables[2], result.Item3.First());
                Helper.AssertPropertiesEquality(tables[3], result.Item4.First());
                Helper.AssertPropertiesEquality(tables[4], result.Item5.First());
                Helper.AssertPropertiesEquality(tables[5], result.Item6.First());
                Helper.AssertPropertiesEquality(tables[6], result.Item7.First());
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT2DifferentFilters()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).ToList();
            var firstHalfIds = tables.Take(5).Select(t => t.Id).ToArray();
            var secondHalfIds = tables.Skip(5).Select(t => t.Id).ToArray();

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => firstHalfIds.Contains(e.Id),
                    e => secondHalfIds.Contains(e.Id));

                // Assert
                Assert.AreEqual(5, result.Item1.Count());
                Assert.AreEqual(5, result.Item2.Count());
                result.Item1.AsList().ForEach(item => Assert.IsTrue(firstHalfIds.Contains(item.Id)));
                result.Item2.AsList().ForEach(item => Assert.IsTrue(secondHalfIds.Contains(item.Id)));
            }
        }

        [TestMethod]
        public async Task TestOracleConnectionQueryMultipleAsyncForT2WithHintsThrows()
        {
            // Setup
            Database.CreateCompleteTables(10);

            using (var connection = new OracleConnection(Database.ConnectionString))
            {
                // Act/Assert
                await Assert.ThrowsAsync<NotSupportedException>(() =>
                    connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id > 0,
                        e => e.Id > 0,
                        top1: 1,
                        top2: 2,
                        hints1: "NOLOCK",
                        hints2: "NOLOCK"));
            }
        }

        #endregion

        #endregion
    }
}
