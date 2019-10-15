using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.StatementBuilders;
using RepoDb.UnitTests.CustomObjects;
using RepoDb.UnitTests.Setup;
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
            DbSettingMapper.Add(typeof(CustomDbConnectionForDbConnectionICache), Helper.DbSetting, true);
            DbValidatorMapper.Add(typeof(CustomDbConnectionForDbConnectionICache), Helper.DbValidator, true);
        }

        #region SubClasses

        private class CustomDbConnectionForDbConnectionICache : CustomDbConnection { }

        private class CacheEntity
        {
            [Primary, Identity]
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                cacheItemExpiration, /* cacheItemExpiration */
                (int?)null, /* commandTimeout */
                (IDbTransaction)null, /* transaction */
                cache.Object, /* cache */
                (ITrace)null, /* trace */
                new SqlServerStatementBuilder() /* statementBulder */);

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            new CustomDbConnectionForDbConnectionICache().Query<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder()).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>((object)null, /* whereOrPrimaryKey */
                (IEnumerable<OrderField>)null, /* orderBy */
                (int?)null, /* top */
                (string)null, /* hints */
                cacheKey, /* cacheKey */
                cacheItemExpiration, /* cacheItemExpiration */
                (int?)null, /* commandTimeout */
                (IDbTransaction)null, /* transaction */
                cache.Object, /* cache */
                (ITrace)null, /* trace */
                new SqlServerStatementBuilder() /* statementBulder */).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>(where: (QueryField)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder()).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>(where: (IEnumerable<QueryField>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder()).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>(where: (Expression<Func<CacheEntity, bool>>)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder()).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
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
            var result = new CustomDbConnectionForDbConnectionICache().QueryAsync<CacheEntity>(where: (QueryGroup)null,
                orderBy: null,
                top: 0,
                hints: null,
                cacheKey: cacheKey,
                cacheItemExpiration: cacheItemExpiration,
                commandTimeout: null,
                transaction: null,
                cache: cache.Object,
                trace: null,
                statementBuilder: new SqlServerStatementBuilder()).Result;

            // Assert
            cache.Verify(c => c.Get(It.Is<string>(s => s == cacheKey),
                It.IsAny<bool>()), Times.Once);
            cache.Verify(c => c.Add(It.Is<string>(s => s == cacheKey),
                It.IsAny<object>(),
                It.Is<int>(i => i == cacheItemExpiration),
                It.IsAny<bool>()), Times.Once);
        }

        #endregion
    }
}
