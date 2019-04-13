using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.IntegrationTests.Caches
{
    [TestClass]
    public class SqlConnectionCachesTest
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

        #region Helper

        private SimpleTable GetSimpleTable()
        {
            var random = new Random();
            return new SimpleTable
            {
                ColumnBit = true,
                ColumnDateTime = DateTime.UtcNow,
                ColumnDateTime2 = DateTime.UtcNow,
                ColumnDecimal = Convert.ToDecimal(random.Next(int.MinValue, int.MaxValue)),
                ColumnFloat = Convert.ToSingle(random.Next(int.MinValue, int.MaxValue)),
                ColumnInt = random.Next(int.MinValue, int.MaxValue),
                ColumnNVarChar = Guid.NewGuid().ToString()
            };
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestSqlConnectionQueryCacheWithoutExpression()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>(orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    cacheItemExpiration, /* cacheItemExpiration */
                    (int?)null, /* commandTimeout */
                    (IDbTransaction)null, /* transaction */
                    cache, /* cache */
                    (ITrace)null, /* trace */
                    new SqlStatementBuilder() /* statementBulder */);

                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryField()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFields()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaExpression()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>(where: (Expression<Func<SimpleTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryGroup()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<SimpleTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheWithoutExpression()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>(orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    cacheItemExpiration, /* cacheItemExpiration */
                    (int?)null, /* commandTimeout */
                    (IDbTransaction)null, /* transaction */
                    cache, /* cache */
                    (ITrace)null, /* trace */
                    new SqlStatementBuilder() /* statementBulder */).Result;

                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryField()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFields()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaExpression()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>(where: (Expression<Func<SimpleTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryGroup()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetSimpleTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<SimpleTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion
    }
}
