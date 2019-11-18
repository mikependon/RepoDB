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
        public void TestMinWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaQueryFields()
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
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaQueryGroup()
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
                var result = connection.Min<CompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMinWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.Min<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAsyncWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaQueryFields()
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
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaQueryGroup()
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
                var result = connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAsyncWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.MinAsync<CompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestMinViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaTableNameViaQueryFields()
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
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinViaTableNameViaQueryGroup()
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
                var result = connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMinViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.Min(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestMinAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaTableNameViaQueryFields()
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
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod]
        public void TestMinAsyncViaTableNameViaQueryGroup()
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
                var result = connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), result);
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnMinAsyncViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionString))
            {
                // Setup
                var tables = Database.CreateCompleteTables(10, connection);

                // Act
                connection.MinAsync(ClassMappedNameCache.Get<CompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
