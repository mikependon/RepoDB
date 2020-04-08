using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class PropertyMappedNameCacheTest
    {
        private class PropertyMappedNameCacheTestClass
        {
            public string ColumnString { get; set; }
            [Map("PropertyName")]
            public string PropertyString { get; set; }
        }

        [TestMethod]
        public void TestWithoutMapAttribute()
        {
            // Act
            var property = PropertyCache.Get<PropertyMappedNameCacheTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            var expected = "ColumnString";

            // Assert
            Assert.AreEqual(expected, property.GetMappedName());
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
    }
}
