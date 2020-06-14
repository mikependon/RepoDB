using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlConnector;
using RepoDb.Enumerations;
using RepoDb.MySqlConnector.IntegrationTests.Models;
using RepoDb.MySqlConnector.IntegrationTests.Setup;
using System;
using System.Linq;

namespace RepoDb.MySqlConnector.IntegrationTests.Operations
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
        public void TestMySqlConnectionCountWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>((object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count<CompleteTable>(queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionCountWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Count<CompleteTable>((object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionCountAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>((object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>(e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>(new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>(new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>(queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync<CompleteTable>(queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionCountAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAsync<CompleteTable>((object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMySqlConnectionCountViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionCountViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Count(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaTableNameViaQueryFields()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod]
        public void TestMySqlConnectionCountAsyncViaTableNameViaQueryGroup()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var queryFields = new[]
            {
                new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                new QueryField("Id", Operation.LessThan, tables.Last().Id)
            };
            var queryGroup = new QueryGroup(queryFields);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Count(), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionCountAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.CountAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
