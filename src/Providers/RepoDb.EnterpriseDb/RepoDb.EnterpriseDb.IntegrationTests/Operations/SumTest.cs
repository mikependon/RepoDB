#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Enumerations;
using RepoDb.EnterpriseDb.IntegrationTests.Models;
using RepoDb.EnterpriseDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.EnterpriseDb.IntegrationTests.Operations
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
        public void TestEnterpriseDbConnectionSumWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionSumWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionSumAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionSumAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionSumViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionSumViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
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
        public async Task TestEnterpriseDbConnectionSumAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionSumAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionSumAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInteger", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
