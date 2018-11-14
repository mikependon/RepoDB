using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public class MemoryCacheTest
    {
        [TestMethod]
        public void TestAdd()
        {
            // Prepare
            var cache = new MemoryCache();
            var key = "Key";
            var value = new object();

            // Act
            cache.Add(key, value);

            // Assert
            Assert.AreEqual(1, cache.Count());
        }

        [TestMethod]
        public void TestCacheItem()
        {
            // Prepare
            var cache = new MemoryCache();
            var item = new CacheItem("Key", new object());

            // Act
            cache.Add(item);

            // Assert
            Assert.AreEqual(1, cache.Count());
        }

        [TestMethod]
        public void TestClear()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add(new CacheItem("Key", new object()));

            // Act
            cache.Clear();

            // Assert
            Assert.IsFalse(cache.Any());
        }

        [TestMethod]
        public void TestContains()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add(new CacheItem("Key", new object()));

            // Act
            var actual = cache.Contains("Key");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestGet()
        {
            // Prepare
            var cache = new MemoryCache();
            var key = "Key";
            var value = new object();
            cache.Add(key, value);

            // Act
            var actual = cache.Get(key);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(value, actual.Value);
        }

        [TestMethod]
        public void TestGetExpired()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object(), 0);

            // Act
            var actual = cache.Get("Key");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestOverrideExpiredItem()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object(), 0);

            // Act
            var actual = cache.Get("Key");

            // Assert
            Assert.IsNull(actual);

            // Reprepare
            cache.Add("Key", new object());

            // React
            actual = cache.Get("Key");

            // Reassert
            Assert.IsNotNull(actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnAddingNewItemWithTheSameKey()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object());

            // Act/Assert
            cache.Add("Key", new object());
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnAddingACacheWithNegativeExpiration()
        {
            // Prepare
            var cache = new MemoryCache();

            // Act/Assert
            cache.Add("Key", "Value", -1);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnRemovingAKeyThatIsNotPresent()
        {
            // Prepare
            var cache = new MemoryCache();

            // Act/Assert
            cache.Remove("Key");
        }
    }
}
