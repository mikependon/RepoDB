using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace RepoDb.IntegrationTests.Caches
{
    [TestClass]
    public class DbRepositoryCachesTest
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

        #region Query

        [TestMethod]
        public void TestSqlConnectionQueryCacheViaDynamics()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query<IdentityTable>((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (int?)null, /* top */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    (IDbTransaction)null);

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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query<IdentityTable>(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null);
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query<IdentityTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null);
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query<IdentityTable>(where: (Expression<Func<IdentityTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null);
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.Query<IdentityTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestSqlConnectionQueryCacheWithoutExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAll<IdentityTable>(orderBy: null,
                    cacheKey: cacheKey,
                    transaction: null);
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region QueryAsync

        [TestMethod]
        public void TestSqlConnectionQueryAsyncCacheViaDynamics()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync<IdentityTable>((object)null, /* whereOrPrimaryKey */
                    (IEnumerable<OrderField>)null, /* orderBy */
                    (int?)null, /* top */
                    (string)null, /* hints */
                    cacheKey, /* cacheKey */
                    (IDbTransaction)null).Result;

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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync<IdentityTable>(where: (QueryField)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null).Result;
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync<IdentityTable>(where: (IEnumerable<QueryField>)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null).Result;
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync<IdentityTable>(where: (Expression<Func<IdentityTable, bool>>)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null).Result;
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
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAsync<IdentityTable>(where: (QueryGroup)null,
                    orderBy: null,
                    top: 0,
                    hints: null,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion

        #region QueryAllAsync

        [TestMethod]
        public void TestSqlConnectionQueryAllAsyncCacheWithoutExpression()
        {
            // Setup
            var cache = new MemoryCache();
            var entity = GetIdentityTable();
            var cacheKey = "SimpleTables";
            var cacheItemExpiration = 60;

            using (var repository = new DbRepository<SqlConnection>(Database.ConnectionStringForRepoDb,
                0,
                ConnectionPersistency.PerCall,
                cache,
                cacheItemExpiration,
                null,
                null))
            {
                // Act
                entity.Id = Convert.ToInt32(repository.Insert(entity));

                // Act
                var result = repository.QueryAllAsync<IdentityTable>(orderBy: null,
                    cacheKey: cacheKey,
                    transaction: null).Result;
                var item = cache.Get<IEnumerable<IdentityTable>>(cacheKey);

                // Assert
                Assert.AreEqual(1, result.Count());
                Assert.IsNotNull(item);
                Assert.AreEqual(cacheItemExpiration, (item.Expiration - item.CreatedDate).TotalMinutes);
            }
        }

        #endregion
    }
}
