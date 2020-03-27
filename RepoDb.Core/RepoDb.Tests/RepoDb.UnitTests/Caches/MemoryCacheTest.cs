using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public class MemoryCacheTest
    {
        [TestMethod]
        public void TestMemoryCacheAdd()
        {
            // Prepare
            var cache = new MemoryCache();
            var key = "Key";
            var value = new object();

            // Act
            cache.Add(key, value);

            // Assert
            Assert.AreEqual(1, cache.OfType<object>().Count());
        }

        [TestMethod]
        public void TestMemoryCacheCacheItem()
        {
            // Prepare
            var cache = new MemoryCache();
            var item = new CacheItem<object>("Key", new object());

            // Act
            cache.Add(item);

            // Assert
            Assert.AreEqual(1, cache.OfType<object>().Count());
        }

        [TestMethod]
        public void TestMemoryCacheClear()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add(new CacheItem<object>("Key", new object()));

            // Act
            cache.Clear();

            // Assert
            Assert.IsFalse(cache.OfType<object>().Any());
        }

        [TestMethod]
        public void TestMemoryCacheContains()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add(new CacheItem<object>("Key", new object()));

            // Act
            var actual = cache.Contains("Key");

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMemoryCacheGet()
        {
            // Prepare
            var cache = new MemoryCache();
            var key = "Key";
            var value = new object();
            cache.Add(key, value);

            // Act
            var actual = cache.Get<object>(key);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(value, actual.Value);
        }

        [TestMethod]
        public void TestMemoryCacheGetExpired()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object(), 0);

            // Act
            var actual = cache.Get<object>("Key");

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestMemoryCacheOverrideExpiredItem()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object(), 0);

            // Act
            var actual = cache.Get<object>("Key");

            // Assert
            Assert.IsNull(actual);

            // Reprepare
            cache.Add("Key", new object());

            // React
            actual = cache.Get<object>("Key");

            // Reassert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void TestMemoryCacheManualExpirationDate()
        {
            // Prepare
            var cache = new MemoryCache();
            var expirationInMinutes = 60;
            cache.Add("Key", new object(), expirationInMinutes);

            // Act
            var actual = cache.Get<object>("Key");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expirationInMinutes, (actual.Expiration - actual.CreatedDate).TotalMinutes);
        }

        [TestMethod]
        public void TestMemoryCacheManualExpirationDateViaCacheItem()
        {
            // Prepare
            var cache = new MemoryCache();
            var expirationInMinutes = 60;
            var cacheItem = new CacheItem<object>("Key", new object(), expirationInMinutes);
            cache.Add(cacheItem);

            // Act
            var actual = cache.Get<object>("Key");

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expirationInMinutes, (actual.Expiration - actual.CreatedDate).TotalMinutes);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnAddingNewItemAtMemoryCacheWithTheSameKey()
        {
            // Prepare
            var cache = new MemoryCache();
            cache.Add("Key", new object(), throwException: true);

            // Act/Assert
            cache.Add("Key", new object(), throwException: true);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionAtMemoryCacheOnAddingACacheWithNegativeExpiration()
        {
            // Prepare
            var cache = new MemoryCache();

            // Act/Assert
            cache.Add("Key", "Value", -1);
        }

        [TestMethod, ExpectedException(typeof(ItemNotFoundException))]
        public void ThrowExceptionAtMemoryCacheOnRemovingAKeyThatIsNotPresent()
        {
            // Prepare
            var cache = new MemoryCache();

            // Act/Assert
            cache.Remove("Key", true);
        }
    }
}
