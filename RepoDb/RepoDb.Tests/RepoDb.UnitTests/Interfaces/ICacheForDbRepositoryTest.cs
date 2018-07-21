using Moq;
using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Interfaces
{
    [TestFixture]
    public class ICacheForDbRepositoryTest
    {
        private class CacheEntity : DataEntity
        {
            [Primary, Identity]
            public int Id { get; set; }
            public string Name { get; set; }
        }

        [Test]
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
