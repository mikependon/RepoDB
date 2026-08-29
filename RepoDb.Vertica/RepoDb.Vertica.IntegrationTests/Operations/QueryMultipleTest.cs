#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vertica.Data.VerticaClient;
using RepoDb.Extensions;
using RepoDb.Vertica.IntegrationTests.Models;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.Vertica.IntegrationTests.Operations
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
        public void TestVerticaConnectionQueryMultipleForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT2ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public void TestVerticaConnectionQueryMultipleForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT3ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public void TestVerticaConnectionQueryMultipleForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT4ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public void TestVerticaConnectionQueryMultipleForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT5ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public void TestVerticaConnectionQueryMultipleForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT6ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public void TestVerticaConnectionQueryMultipleForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public void TestVerticaConnectionQueryMultipleForT7ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.QueryMultiple<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.QueryMultiple<CompleteTable, CompleteTable>(e => e.Id > 0,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT2()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT2ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT3()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT3ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT4()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT4ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT5()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT5ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT6()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT6ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT7()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
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
        public async Task TestVerticaConnectionQueryMultipleAsyncForT7ViaId()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10).AsList();

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.QueryMultipleAsync<CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable, CompleteTable>(e => e.Id == tables[0].Id,
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
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new VerticaConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.QueryMultipleAsync<CompleteTable, CompleteTable>(e => e.Id > 0,
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
