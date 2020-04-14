using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class PropertyMappedNameCacheTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyMappedNameCache.Flush();
        }

        #region SubClasses

        private class PropertyMappedNameCacheTestClass
        {
            public string ColumnString { get; set; }
            [Map("PropertyName")]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        [TestMethod]
        public void TestWithoutMapAttribute()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMappedNameCacheTestClass>(e => e.ColumnString);
            var expected = "ColumnString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttribute()
        {
            // Act
            var property = PropertyCache.Get<PropertyMappedNameCacheTestClass>()
                .First(p => p.PropertyInfo.Name == "PropertyString");
            var expected = "PropertyName";

            // Assert
            Assert.AreEqual(expected, property.GetMappedName());
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExcpetionOnPropertyMappingCacheIfThePropertyIsNull()
        {
            // Setup
            PropertyMappedNameCache.Get<PropertyMappedNameCacheTestClass>((Field)null);
        }

        #endregion
    }
}
