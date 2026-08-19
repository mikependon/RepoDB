using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.MariaDbConnector;
using RepoDb.Enumerations;
using RepoDb.MariaDb.IntegrationTests.Models;
using RepoDb.MariaDb.IntegrationTests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RepoDb.MariaDb.IntegrationTests.Operations
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
        public void TestMariaDbConnectionSumWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionSumWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Sum<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnMariaDbConnectionSumAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync<CompleteTable>(e => e.ColumnInt,
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMariaDbConnectionSumViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionSumViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionSumViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                Assert.Throws<NotSupportedException>(() =>
                    connection.Sum(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInt", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionSumAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Sum(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnMariaDbConnectionSumAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.SumAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        new Field("ColumnInt", typeof(int)),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
