using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ICacheForDbRepositoryTest
    {
        public class CacheEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #region Sync

        [TestMethod]
        public void TestDbRepositoryQueryCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>(orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (string)null, /* hints */
                cachKey, /* cacheKey */
                (IDbTransaction)null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            repository.Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        #endregion

        #region Async

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>(orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (string)null, /* hints */
                cachKey, /* cacheKey */
                (IDbTransaction)null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cachKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CustomDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null,
                new SqlStatementBuilder());

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cachKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cachKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cachKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        #endregion
    }
}
