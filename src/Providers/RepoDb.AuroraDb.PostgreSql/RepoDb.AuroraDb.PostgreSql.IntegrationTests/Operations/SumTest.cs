#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.AuroraDb.Npgsql;
using RepoDb.Enumerations;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Models;
using RepoDb.AuroraDb.PostgreSql.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.AuroraDb.PostgreSql.IntegrationTests.Operations
{
    [TestClass]
    public class SumTest
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
        public void TestAuroraDbConnectionSumWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionSumWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Sum<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionSumAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestAuroraDbConnectionSumViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void TestAuroraDbConnectionSumViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnAuroraDbConnectionSumViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInteger", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id }).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id)).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task TestAuroraDbConnectionSumAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup).ConfigureAwait(false);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnAuroraDbConnectionSumAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new AuroraDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInteger", typeof(int)),
                        (object)null,
                        hints: "WhatEver").ConfigureAwait(false)).ConfigureAwait(false);
            }
        }

        #endregion

        #endregion
    }
}
