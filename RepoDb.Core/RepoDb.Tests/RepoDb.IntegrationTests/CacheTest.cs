using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Dynamic;

namespace RepoDb.IntegrationTests.Caches
{
    [TestClass]
    public class CacheTest
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

        private IdentityTable GetIdentityTable()
        {
            var random = new Random();
            return new IdentityTable
            {
                RowGuid = Guid.NewGuid(),
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

        #region ExecuteQuery

        [TestMethod]
        public void TestSqlConnectionExecuteQueryCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQuery<IdentityTable>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryCacheAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQuery("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryCacheAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQuery<ExpandoObject>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache);
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryCacheAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQuery<IDictionary<string, object>>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache);
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region ExecuteQueryAsync

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQueryAsync<IdentityTable>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncCacheAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQueryAsync("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache).Result;
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncCacheAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQueryAsync<ExpandoObject>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache).Result;
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionExecuteQueryAsyncCacheAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.ExecuteQueryAsync<IDictionary<string, object>>("SELECT * FROM [sc].[IdentityTable];",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache).Result;
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region ExecuteScalar

        [TestMethod]
        public void TestSqlConnectionExecuteScalarCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var cacheKey = "ServerDateTimeUtc";
                var cacheItemExpiration = 60;

                // Act
                var result = connection.ExecuteScalar<DateTime>("SELECT GETUTCDATE();",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache);
                var item = cache.Get<DateTime>(cacheKey);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsNotNull(item);
                Assert.AreEqual(result, item.Value);
            }
        }

        #endregion

        #region ExecuteScalarAsync

        [TestMethod]
        public void TestSqlConnectionExecuteScalarAsyncCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var cacheKey = "ServerDateTimeUtc";
                var cacheItemExpiration = 60;

                // Act
                var result = connection.ExecuteScalarAsync<DateTime>("SELECT GETUTCDATE();",
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    cache: cache).Result;
                var item = cache.Get<DateTime>(cacheKey);

                // Assert
                Assert.IsNotNull(result);
                Assert.IsNotNull(item);
                Assert.AreEqual(result, item.Value);
            }
        }

        #endregion

        #region Query

        #region TEntity

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IdentityTable>(what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder);

                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IdentityTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IdentityTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IdentityTable>(where: (Expression<Func<IdentityTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IdentityTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region Dynamics

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamicsAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder);

                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldsAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryGroupAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region ExpandoObject

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamicsAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder);

                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldsAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryGroupAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region IDictionary<string, object>

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamicsAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder);

                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryFieldsAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaQueryGroupAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #endregion

        #region QueryAsync

        #region TEntity

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IdentityTable>(what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder).Result;

                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IdentityTable>(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IdentityTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IdentityTable>(where: (Expression<Func<IdentityTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

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
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IdentityTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region Dynamics

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamicsAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder).Result;

                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldsAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.Query(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryGroupAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region ExpandoObject

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamicsAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder).Result;

                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldsAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryGroupAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region IDictionary<string, object>

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamicsAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    what: (object)null,
                    orderBy: (IEnumerable<OrderField>)null,
                    top: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: null,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: Helper.StatementBuilder).Result;

                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryFieldsAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaQueryGroupAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestSqlConnectionQueryAllCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAll<IdentityTable>(orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllCacheAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAll(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllCacheAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAll<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllCacheAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAll<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null);
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncCache()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAllAsync<IdentityTable>(orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncCacheAsDynamics()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAllAsync(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<dynamic>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncCacheAsExpandoObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAllAsync<ExpandoObject>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<ExpandoObject>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncCacheAsDictionaryStringObject()
        {
            using (var connection = new SqlConnection(Database.ConnectionStringForRepoDb))
            {
                // Setup
                var cache = new MemoryCache();
                var entity = GetIdentityTable();
                var cacheKey = "SimpleTables";
                var cacheItemExpiration = 60;

                // Act
                entity.Id = Convert.ToInt32(connection.Insert(entity));

                // Act
                var result = connection.QueryAllAsync<IDictionary<string, object>>(ClassMappedNameCache.Get<IdentityTable>(),
                    orderBy: null,
                    hints: null,
                    cacheKey: cacheKey,
                    cacheItemExpiration: cacheItemExpiration,
                    commandTimeout: 0,
                    transaction: null,
                    cache: cache,
                    trace: null,
                    statementBuilder: null).Result;
                var item = cache.Get<IEnumerable<IDictionary<string, object>>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion
    }
}
