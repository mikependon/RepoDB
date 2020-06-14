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
        public void TestMySqlConnectionMaxWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaQueryFields()
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
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaQueryGroup()
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
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionMaxWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Max<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);
            var ids = new[] { tables.First().Id, tables.Last().Id };

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaQueryFields()
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
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaQueryGroup()
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
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionMaxAsyncWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMySqlConnectionMaxViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionMaxViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionMaxViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
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
        public void TestMySqlConnectionMaxViaTableNameViaQueryFields()
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
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxViaTableNameViaQueryGroup()
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
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMySqlConnectionMaxViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaTableNameWithoutExpression()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaTableNameViaDynamic()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaTableNameViaQueryField()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaTableNameViaQueryFields()
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
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestMySqlConnectionMaxAsyncViaTableNameViaQueryGroup()
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
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMySqlConnectionMaxAsyncViaTableNameWithHints()
        {
            // Setup
            var tables = Database.CreateCompleteTables(10);

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
