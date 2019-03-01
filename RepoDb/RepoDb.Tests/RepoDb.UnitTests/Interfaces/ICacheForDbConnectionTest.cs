using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ICacheForDbConnectionTest
    {
        public class CacheEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestDbConnectionQueryCacheKey()
        {
            // Prepare
            var trace = new Mock<ITrace>();
            var cache = new Mock<ICache>();
            var connection = new CustomDbConnection();

            // Setup
            cache.Setup(c => c.Get(It.IsAny<string>(), It.IsAny<bool>()));

            // Act
            connection.Query<CacheEntity>(cacheKey: "MemoryCacheKey", cache: cache.Object, trace: trace.Object, statementBuilder: new SqlStatementBuilder());

            // Assert
            cache.Verify(c => c.Get(It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }
    }
}
