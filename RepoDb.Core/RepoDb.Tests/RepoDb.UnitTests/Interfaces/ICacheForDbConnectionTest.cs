using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ICacheForDbConnectionTest
    {
        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<CacheDbConnection>(new CustomDbSetting(), true);
            DbHelperMapper.Add<CacheDbConnection>(new CustomDbHelper(), true);
            StatementBuilderMapper.Add<CacheDbConnection>(new CustomStatementBuilder(), true);
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

        #region Query

        [TestMethod]
        public void TestDbConnectionQueryCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (QueryGroup)null,
                fields: null,
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
            new CacheDbConnection().Query<CacheEntity>(what: null,
                fields: null,
                orderBy: null,
                top: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration,
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
        public void TestDbConnectionQueryCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().Query<CacheEntity>(where: (QueryField)null,
                fields: null,
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
                fields: null,
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
                fields: null,
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
                fields: null,
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

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionQueryAllCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAll<CacheEntity>(fields: null,
                orderBy: null,
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
        public void TestDbConnectionQueryAllCaching()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAll<CacheEntity>(fields: null,
                orderBy: null,
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
        public void TestDbConnectionQueryAllCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAll<CacheEntity>(fields: null,
                orderBy: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration,
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

        #endregion

        #region Async

        #region Query

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                fields: null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(what: null,
                fields: null,
                orderBy: null,
                top: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryField()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryField)null,
                fields: null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryFields()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(where: (IEnumerable<QueryField>)null,
                fields: null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                fields: null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAsyncCachingViaQueryGroup()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                fields: null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region QueryAll

        [TestMethod]
        public void TestDbConnectionQueryAllAsyncCachingWithoutExpression()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAllAsync<CacheEntity>(fields: null,
                orderBy: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAllAsyncCaching()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAllAsync<CacheEntity>(fields: null,
                orderBy: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void TestDbConnectionQueryAllAsyncCachingViaDynamics()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var cacheKey = "MemoryCacheKey";
            var cacheItemExpiration = 60;

            // Act
            new CacheDbConnection().QueryAllAsync<CacheEntity>(fields: null,
                orderBy: null,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null).Wait();

            // Assert
            cache.Verify(c => c.GetAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.AddAsync<IEnumerable<CacheEntity>>(It.Is<string>(s => s == cacheKey),
                It.IsAny<IEnumerable<CacheEntity>>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #endregion
    }
}
