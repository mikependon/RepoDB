using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.SqLite.IntegrationTests.Models;
using RepoDb.SqLite.IntegrationTests.Setup;
using System;
using System.Data.SQLite;
using System.Linq;

namespace RepoDb.SqLite.IntegrationTests.Operations.SDS
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
        public void TestSqLiteConnectionMinWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id));

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionMinWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.Min<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var ids = new[] { tables.First().Id, tables.Last().Id };

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    e => ids.Contains(e.Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => ids.Contains(e.Id)).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionMinAsyncWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.MinAsync<SdsCompleteTable>(e => e.ColumnInt,
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion

        #region TableName

        #region Sync

        [TestMethod]
        public void TestSqLiteConnectionMinViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null);

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id });

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id));

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup);

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteConnectionMinViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.Min(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver");
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaTableNameWithoutExpression()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null).Result;

                // Assert
                Assert.AreEqual(tables.Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaTableNameViaDynamic()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new { tables.First().Id }).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaTableNameViaQueryField()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    new QueryField("Id", tables.First().Id)).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id == tables.First().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaTableNameViaQueryFields()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryFields).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod]
        public void TestSqLiteConnectionMinAsyncViaTableNameViaQueryGroup()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);
                var queryFields = new[]
                {
                    new QueryField("Id", Operation.GreaterThan, tables.First().Id),
                    new QueryField("Id", Operation.LessThan, tables.Last().Id)
                };
                var queryGroup = new QueryGroup(queryFields);

                // Act
                var result = connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    queryGroup).Result;

                // Assert
                Assert.AreEqual(tables.Where(e => e.Id > tables.First().Id && e.Id < tables.Last().Id).Min(e => e.ColumnInt), Convert.ToInt32(result));
            }
        }

        [TestMethod, ExpectedException(typeof(AggregateException))]
        public void ThrowExceptionOnSqLiteConnectionMinAsyncViaTableNameWithHints()
        {
            using (var connection = new SQLiteConnection(Database.ConnectionStringSDS))
            {
                // Setup
                var tables = Database.CreateSdsCompleteTables(10, connection);

                // Act
                connection.MinAsync(ClassMappedNameCache.Get<SdsCompleteTable>(),
                    new Field("ColumnInt", typeof(int)),
                    (object)null,
                    hints: "WhatEver").Wait();
            }
        }

        #endregion

        #endregion
    }
}
