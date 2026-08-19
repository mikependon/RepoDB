using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClickHouse.Driver.ADO;
using RepoDb.Enumerations;
using RepoDb.ClickHouse.IntegrationTests.Models;
using RepoDb.ClickHouse.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.ClickHouse.IntegrationTests.Operations
{
    [TestClass]
    public class MaxTest
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
        public void TestClickHouseConnectionMaxWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionMaxWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Max<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnClickHouseConnectionMaxAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestClickHouseConnectionMaxViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestClickHouseConnectionMaxViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnClickHouseConnectionMaxViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInt", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestClickHouseConnectionMaxAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnClickHouseConnectionMaxAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new RepoDbClickHouseConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInt", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
