using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.MDS
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
        public void TestSqLiteConnectionQueryMultipleForT2()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleForT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleForT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleForT5()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleForT6()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleForT7()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT2()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT5()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT6()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public void TestSqLiteConnectionQueryMultipleAsyncForT7()
        {
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
            using (var connection = new SqliteConnection(Database.ConnectionStringMDS))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
