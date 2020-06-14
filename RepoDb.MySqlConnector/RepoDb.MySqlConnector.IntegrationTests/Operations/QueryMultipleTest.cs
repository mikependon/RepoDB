using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Extensions;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations
{
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
        public void TestMySqlConnectionQueryMultipleForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionQueryMultipleForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionQueryMultipleForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionQueryMultipleForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionQueryMultipleForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionQueryMultipleForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionQueryMultipleWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    hints1: "WhatEver",
                    top2: 2,
                    hints2: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionQueryMultipleAsyncForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2).Result;

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(2, result.Item2.Count());
                result.Item1.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
                result.Item2.AsList().ForEach(item => Helper.AssertPropertiesEquality(tables.First(e => e.Id == item.Id), item));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionQueryMultipleAsyncForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3).Result;

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
        public void TestMySqlConnectionQueryMultipleAsyncForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4).Result;

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
        public void TestMySqlConnectionQueryMultipleAsyncForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    top2: 2,
                    top3: 3,
                    top4: 4,
                    top5: 5).Result;

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
        public void TestMySqlConnectionQueryMultipleAsyncForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
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
                    top6: 6).Result;

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
        public void TestMySqlConnectionQueryMultipleAsyncForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id > 0,
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
                    top7: 7).Result;

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

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionQueryMultipleAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id > 0,
                    e => e.Id > 0,
                    top1: 1,
                    hints1: "WhatEver",
                    top2: 2,
                    hints2: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
