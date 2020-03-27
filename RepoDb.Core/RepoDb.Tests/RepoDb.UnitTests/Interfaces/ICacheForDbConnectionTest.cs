using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ICacheForDbConnectionTest
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
        public void TestDbConnectionQueryCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                cacheItemExpiration, /* cacheItemExpiration */
                (int?)null, /* commandTimeout */
                (IDbTransaction)null, /* transaction */
                cache.Object, /* cache */
                (ITrace)null /* trace */);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null);

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null);

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
        public void TestDbConnectionQueryAsyncCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                cacheItemExpiration, /* cacheItemExpiration */
                (int?)null, /* commandTimeout */
                (IDbTransaction)null, /* transaction */
                cache.Object, /* cache */
                (ITrace)null /* trace */).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Result;

            // Assert
            cache.Verify(c => c.Get<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            var result = new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Result;

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
