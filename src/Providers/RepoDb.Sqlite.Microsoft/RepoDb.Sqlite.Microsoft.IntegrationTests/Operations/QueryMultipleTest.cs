#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Models;
using RepoDb.Sqlite.Microsoft.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Sqlite.Microsoft.IntegrationTests.Operations.MDS
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
            using (var connection = new SqliteConnection(Database.ConnectionString))
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
        public void TestSqLiteConnectionQueryMultipleForT2ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    top1: 1,
                    top2: 1);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionQueryMultipleForT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
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
        public void TestSqLiteConnectionQueryMultipleForT3ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1);

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
        public void TestSqLiteConnectionQueryMultipleForT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
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
        public void TestSqLiteConnectionQueryMultipleForT4ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1);

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
        public void TestSqLiteConnectionQueryMultipleForT5()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
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
        public void TestSqLiteConnectionQueryMultipleForT5ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1);

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
        public void TestSqLiteConnectionQueryMultipleForT6()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
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
        public void TestSqLiteConnectionQueryMultipleForT6ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1,
                    top6: 1);

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
        public void TestSqLiteConnectionQueryMultipleForT7()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
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

        [TestMethod]
        public void TestSqLiteConnectionQueryMultipleForT7ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    e => e.Id == tables[6].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1,
                    top6: 1,
                    top7: 1);

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
        public void ThrowExceptionQueryMultipleWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryMultiple<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
                        e => e.Id > 0,
                        top1: 1,
                        hints1: "WhatEver",
                        top2: 2,
                        hints2: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT2()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT2ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    top1: 1,
                    top2: 1);

                // Assert
                Assert.AreEqual(1, result.Item1.Count());
                Assert.AreEqual(1, result.Item2.Count());
                Helper.AssertPropertiesEquality(tables[0], result.Item1.First());
                Helper.AssertPropertiesEquality(tables[1], result.Item2.First());
            }
        }

        [TestMethod]
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT3()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT3ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1);

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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT4()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT4ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1);

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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT5()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT5ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1);

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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT6()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT6ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1,
                    top6: 1);

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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT7()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
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
        public async Task TestSqLiteConnectionQueryMultipleAsyncForT7ViaId()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection).AsList();

                // Act
                var result = await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable, MdsCompleteTable>(e => e.Id == tables[0].Id,
                    e => e.Id == tables[1].Id,
                    e => e.Id == tables[2].Id,
                    e => e.Id == tables[3].Id,
                    e => e.Id == tables[4].Id,
                    e => e.Id == tables[5].Id,
                    e => e.Id == tables[6].Id,
                    top1: 1,
                    top2: 1,
                    top3: 1,
                    top4: 1,
                    top5: 1,
                    top6: 1,
                    top7: 1);

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
        public async Task ThrowExceptionQueryMultipleAsyncWithHints()
        {
            using (var connection = new SqliteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateMdsCompleteTables(10, connection);

                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryMultipleAsync<MdsCompleteTable, MdsCompleteTable>(e => e.Id > 0,
                        e => e.Id > 0,
                        top1: 1,
                        hints1: "WhatEver",
                        top2: 2,
                        hints2: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
