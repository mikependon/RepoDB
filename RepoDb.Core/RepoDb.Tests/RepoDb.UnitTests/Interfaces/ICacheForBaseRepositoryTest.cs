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
    public class ICacheForBaseRepositoryTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add(typeof(CacheDbConnection), new CustomDbSetting(), true);
            DbHelperMapper.Add(typeof(CacheDbConnection), new CustomDbHelper(), true);
            StatementBuilderMapper.Add(typeof(CacheDbConnection), new CustomStatementBuilder(), true);
        }

        #region SubClasses

        private class CacheDbConnection : CustomDbConnection { }

        private class CacheEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class CacheEntityRepository : BaseRepository<CacheEntity, CacheDbConnection>
        {
            public CacheEntityRepository(ICache cache, int cacheItemExpiration)
                : base("ConnectionString",
                      0,
                      ConnectionPersistency.PerCall,
                      cache,
                      cacheItemExpiration,
                      null)
            { }
        }

        #endregion

        #region Sync

        [TestMethod]
        public void TestBaseRepositoryQueryCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query(where: (QueryGroup)null,
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
        public void TestBaseRepositoryQueryCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query((object)null, /* whereOrPrimaryKey */
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
        public void TestBaseRepositoryQueryCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query(where: (QueryField)null,
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
        public void TestBaseRepositoryQueryCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query(where: (IEnumerable<QueryField>)null,
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
        public void TestBaseRepositoryQueryCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query(where: (Expression<Func<CacheEntity, bool>>)null,
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
        public void TestBaseRepositoryQueryCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            repository.Query(where: (QueryGroup)null,
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
        public void TestBaseRepositoryQueryAsyncCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync(where: (QueryGroup)null,
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
        public void TestBaseRepositoryQueryAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync((object)null, /* whereOrPrimaryKey */
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
        public void TestBaseRepositoryQueryAsyncCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync(where: (QueryField)null,
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
        public void TestBaseRepositoryQueryAsyncCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync(where: (IEnumerable<QueryField>)null,
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
        public void TestBaseRepositoryQueryAsyncCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync(where: (Expression<Func<CacheEntity, bool>>)null,
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
        public void TestBaseRepositoryQueryAsyncCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;
            var repository = new CacheEntityRepository(cache.Object, cacheItemExpiration);

            // Act
            var result = repository.QueryAsync(where: (QueryGroup)null,
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
