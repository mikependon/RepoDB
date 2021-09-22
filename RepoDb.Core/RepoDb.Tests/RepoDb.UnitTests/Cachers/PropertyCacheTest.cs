using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using System.Linq;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public class PropertyCacheTest
    {
        #region SubClasses

        public class BaseClass
        {
            public int Id { get; set; }
            public string Property1 { get; set; }
        }

        public class DerivedClass : BaseClass
        {
            public string Property2 { get; set; }
            [Map("Property4")]
            public string Property3 { get; set; }
        }

        #endregion

        #region BaseClass

        [TestMethod]
        public void TestPropertyCacheForBaseClass()
        {
            // Act
            var actual = PropertyCache.Get<BaseClass>();
            var expected = 2;

            // Assert
            Assert.AreEqual(expected, actual.Count());
        }

        [TestMethod]
        public void TestPropertyCacheForBaseClassGetPropertyViaPropertyName()
        {
            // Act
            var actual = PropertyCache.Get<BaseClass>("Id");
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForBaseClassGetPropertyViaExpression()
        {
            // Act
            var actual = PropertyCache.Get<BaseClass>(e => e.Id);
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForBaseClassGetPropertyViaField()
        {
            // Act
            var actual = PropertyCache.Get<BaseClass>(new Field("Id"));
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        #endregion

        #region DerivedClass

        [TestMethod]
        public void TestPropertyCacheForDerivedClass()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>();
            var expected = 4;

            // Assert
            Assert.AreEqual(expected, actual.Count());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetPropertyViaPropertyName()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>("Property2");
            var expected = "Property2";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetPropertyViaExpression()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>(e => e.Property2);
            var expected = "Property2";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetPropertyViaField()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>(new Field("Property2"));
            var expected = "Property2";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetMappedPropertyViaPropertyName()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>("Property4");
            var expected = "Property4";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetMappedPropertyViaExpression()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>(e => e.Property3);
            var expected = "Property4";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        [TestMethod]
        public void TestPropertyCacheForDerivedClassGetMappedPropertyViaField()
        {
            // Act
            var actual = PropertyCache.Get<DerivedClass>(new Field("Property4"));
            var expected = "Property4";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        #endregion
    }
}
