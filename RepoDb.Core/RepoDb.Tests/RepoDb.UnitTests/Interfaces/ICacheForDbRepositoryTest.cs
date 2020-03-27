using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(CacheDbConnection), new CustomDbSetting(), true);
            StatementBuilderMapper.Add(typeof(CacheDbConnection), new CustomStatementBuilder(), true);
        }

        #region SubClasses

        private class CacheDbConnection : CustomDbConnection { }

        private class CacheEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestDbRepositoryQueryCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                (IDbTransaction)null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            repository.Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
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
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                (IDbTransaction)null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbRepositoryQueryAsyncCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new DbRepository<CacheDbConnection>("ConnectionString",
                0,
                ConnectionPersistency.PerCall,
                cache.Object,
                cacheItemExpiration,
                null);

            // Act
            var result = repository.QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                transaction: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        #endregion
    }
}
