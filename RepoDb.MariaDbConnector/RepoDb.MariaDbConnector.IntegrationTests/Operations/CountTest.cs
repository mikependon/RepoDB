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
        public void TestMariaDbConnectionCountWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaQueryFields()
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
                var result = connection.Count<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaQueryGroup()
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
                var result = connection.Count<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionCountWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public async Task TestMariaDbConnectionCountAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaQueryFields()
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
                var result = await connection.CountAsync<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaQueryGroup()
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
                var result = await connection.CountAsync<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnMariaDbConnectionCountAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.CountAsync<CompleteTable>((object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMariaDbConnectionCountViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaTableNameViaQueryFields()
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
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMariaDbConnectionCountViaTableNameViaQueryGroup()
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
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void ThrowExceptionOnMariaDbConnectionCountViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
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
        public async Task TestMariaDbConnectionCountAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaTableNameViaQueryFields()
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
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task TestMariaDbConnectionCountAsyncViaTableNameViaQueryGroup()
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
                var result = await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnMariaDbConnectionCountAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MariaDbConnection(Database.ConnectionString))
            {
                // Act
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                    await connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                        (object)null,
                        hints: "WhatEver"));
            }
        }

        #endregion

        #endregion
    }
}
