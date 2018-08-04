using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestClass]
    public class ICacheForDbRepositoryTest
    {
        public class CacheEntity : DataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestQueryCacheKey()
        {
            // Prepare
            var cache = new Mock<ICache>();
            var repository = new Mock<DbRepository<CustomDbConnection>>("ConnectionString", cache.Object);

            // Setup
            cache.Setup(c => c.Get(It.IsAny<string>()));

            // Act
            repository.Object.Query<CacheEntity>(cacheKey: "MemoryCacheKey");

            // Assert
            cache.Verify(c => c.Get(It.IsAny<string>()), Times.Once);
        }
    }
}
