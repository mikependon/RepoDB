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
    public class MinTest
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
        public void TestEnterpriseDbConnectionMinWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaQueryFields()
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
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaQueryGroup()
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
                var result = connection.Min<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionMinWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Min<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaQueryFields()
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
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaQueryGroup()
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
                var result = await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionMinAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MinAsync<CompleteTable>(e => e.ColumnInteger,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaTableNameViaQueryFields()
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
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestEnterpriseDbConnectionMinViaTableNameViaQueryGroup()
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
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbConnectionMinViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInteger", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaTableNameViaQueryFields()
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
                var result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestEnterpriseDbConnectionMinAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInteger", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInteger), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnEnterpriseDbConnectionMinAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new EDBConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInteger", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
