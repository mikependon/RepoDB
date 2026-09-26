#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.CockroachDb;
using RepoDb.Enumerations;
using RepoDb.CockroachDb.IntegrationTests.Models;
using RepoDb.CockroachDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.CockroachDb.IntegrationTests.Operations
{
    [TestClass]
    public class CountTest
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
        public void TestCockroachDbConnectionCountWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbConnectionCountWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Count<CompleteTable>((object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>((object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(new { tables.First().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbConnectionCountAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.CountAsync<CompleteTable>((object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestCockroachDbConnectionCountViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestCockroachDbConnectionCountViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCockroachDbConnectionCountViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestCockroachDbConnectionCountAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCockroachDbConnectionCountAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new CockroachDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
