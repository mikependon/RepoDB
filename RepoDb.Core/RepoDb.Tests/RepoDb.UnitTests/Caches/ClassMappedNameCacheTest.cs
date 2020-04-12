using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public partial class ClassMappedNameCacheTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ClassMappedNameCache.Flush();
            ClassMapper.Clear();
        }

        #region SubClasses

        private class ClassMappedNameCacheTestClass
        {
        }

        [Map("[dbo].[Person]")]
        private class ClassMappedNameCacheTestWithMapAttribute
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestWithoutMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<ClassMappedNameCacheTestClass>();
            var expected = "ClassMappedNameCacheTestClass";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<ClassMappedNameCacheTestWithMapAttribute>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithClassMapperMapping()
        {
            // Setup
            ClassMapper.Add<ClassMappedNameCacheTestClass>("[dbo].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMappedNameCacheTestClass>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttributeAndWithClassMapperMapping()
        {
            // Setup
            ClassMapper.Add<ClassMappedNameCacheTestWithMapAttribute>("[sales].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMappedNameCacheTestWithMapAttribute>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExcpetionOnClassMappingCacheIfTheTypeIsNull()
        {
            // Setup
            ClassMappedNameCache.Get(null);
        }

        #endregion
    }
}
