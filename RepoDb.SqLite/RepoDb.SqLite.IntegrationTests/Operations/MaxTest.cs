using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations
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
        public void TestMaxWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Max<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMaxWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.Max<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMaxAsyncWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.MaxAsync<CompleteTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAsyncWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

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
        public void TestMaxViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Max(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMaxViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

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
        public void TestMaxAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMaxAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.MaxAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Max(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMaxAsyncViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

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
