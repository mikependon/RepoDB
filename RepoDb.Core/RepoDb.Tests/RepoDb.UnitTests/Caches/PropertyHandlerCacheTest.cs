using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Caches
{
    [TestClass]
    public partial class PropertyHandlerCacheTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerMapper.Clear();
            PropertyHandlerCache.Flush();
        }

        #region PropertyHandler

        private class StringPropertyHandler : IPropertyHandler<string, string>
        {
            public string Get(string input, ClassProperty property)
            {
                throw new NotImplementedException();
            }

            public string Set(string input, ClassProperty property)
            {
                throw new NotImplementedException();
            }
        }

        private class TextPropertyHandler : IPropertyHandler<string, string>
        {
            public string Get(string input, ClassProperty property)
            {
                throw new NotImplementedException();
            }

            public string Set(string input, ClassProperty property)
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region SubClasses

        private class PropertyTypeHandlerCacheTestClass
        {
            public string ColumnString { get; set; }
            [PropertyHandler(typeof(TextPropertyHandler))]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region Type Level

        [TestMethod]
        public void TestPropertyHandlerCacheWithoutMapping()
        {
            // Act
            var result = PropertyHandlerCache.Get<PropertyHandlerCacheTest, StringPropertyHandler>();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestPropertyHandlerCacheWithMapping()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(stringPropertyHandler);

            // Act
            var result = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>();
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Property Level

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaPropertyName()
        {
            // Setup
            var propertyName = "ColumnString";
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(propertyName);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaField()
        {
            // Setup
            var field = new Field("ColumnString");
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(field, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(field);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaExpression()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, StringPropertyHandler>(e => e.ColumnString);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            var propertyName = "PropertyString";

            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(actual);

            // Act (Existing)
            actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("PropertyString");

            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(actual);

            // Act (Existing)
            actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void TestPropertyHandlerCachePropertyMappingViaExpressionWithMapAttribute()
        {
            // Act
            var actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(actual);

            // Act (Existing)
            actual = PropertyHandlerCache.Get<PropertyTypeHandlerCacheTestClass, TextPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(actual);
        }

        #endregion

        #endregion
    }
}
