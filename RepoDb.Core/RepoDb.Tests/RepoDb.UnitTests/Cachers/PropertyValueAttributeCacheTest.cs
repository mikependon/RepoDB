using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Cachers
{
    [TestClass]
    public partial class PropertyValueAttributeCacheTest
    {
        [TestInitialize]
        public void Initialize()
        {
            var attributes = GetPropertyValueAttributes();
            PropertyValueAttributeMapper.Add<PropertyValueAttributeClass>(e => e.PropertyString, attributes, true);
            PropertyValueAttributeMapper.Add<PropertyValueAttributeClass>(e => e.PropertyDecimal, attributes, true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyValueAttributeCache.Flush();
            PropertyValueAttributeMapper.Clear();
        }

        #region SubClasses

        private class PropertyValueAttributeClass
        {
            public string PropertyString { get; set; }

            [
                Name("ColumnInt"),
                Size(256)
            ]
            public string PropertyInt { get; set; }

            [
                Name("ColumnDecimal"),
                DbType(DbType.Decimal),
                Direction(ParameterDirection.InputOutput),
                IsNullable(true),
                Precision(100),
                Scale(2),
                Size(256)
            ]
            public string PropertyDecimal { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<PropertyValueAttribute> GetPropertyValueAttributes() =>
            new PropertyValueAttribute[]
            {
                // Different Values
                new NameAttribute("ColumnString"),
                new DbTypeAttribute(DbType.StringFixedLength),
                new DirectionAttribute(ParameterDirection.ReturnValue),
                new SizeAttribute(512),
                // Same Values
                new IsNullableAttribute(true),
                new PrecisionAttribute(100),
                new ScaleAttribute(2)
            };

        #endregion

        #region Methods

        /*
         * With Attributes
         */

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithAttributesViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyInt";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName);

            // Assert
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithAttributesViaField()
        {
            // Prepare
            var field = new Field("PropertyInt");

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(field);

            // Assert
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithAttributesViaExpression()
        {
            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(e => e.PropertyInt);

            // Assert
            Assert.AreEqual(2, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithAttributesViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyInt", true);

            // Act
            var actual = PropertyValueAttributeCache.Get(classProperty.PropertyInfo);

            // Assert
            Assert.AreEqual(2, actual.Count());
        }

        /*
         * Without Attributes
         */

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesViaMappedPropertyName()
        {
            // Prepare
            var propertyName = "ColumnString";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyString";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesViaField()
        {
            // Prepare
            var field = new Field("PropertyString");

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(field, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesViaExpression()
        {
            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(e => e.PropertyString, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyString", true);

            // Act
            var actual = PropertyValueAttributeCache.Get(classProperty.PropertyInfo, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesWithIncludeMappingsFalseViaMappedPropertyName()
        {
            // Prepare
            var propertyName = "PropertyString";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesWithIncludeMappingsFalseViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyString";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesWithIncludeMappingsFalseViaField()
        {
            // Prepare
            var field = new Field("PropertyString");

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(field, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesWithIncludeMappingsFalseViaExpression()
        {
            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(e => e.PropertyString, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheWithMappedAttributesWithIncludeMappingsFalseViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyString", true);

            // Act
            var actual = PropertyValueAttributeCache.Get(classProperty.PropertyInfo, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        /*
         * Attribute Collisions
         */

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsViaMappedPropertyName()
        {
            // Prepare
            var propertyName = "ColumnDecimal";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyDecimal";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsViaField()
        {
            // Prepare
            var field = new Field("PropertyDecimal");

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(field, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsViaExpression()
        {
            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(e => e.PropertyDecimal, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyDecimal", true);

            // Act
            var actual = PropertyValueAttributeCache.Get(classProperty.PropertyInfo, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsWithIncludeMappingsFalseViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyDecimal";

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(propertyName, false);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsWithIncludeMappingsFalseViaField()
        {
            // Prepare
            var field = new Field("PropertyDecimal");

            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(field, false);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsWithIncludeMappingsFalseViaExpression()
        {
            // Act
            var actual = PropertyValueAttributeCache.Get<PropertyValueAttributeClass>(e => e.PropertyDecimal, false);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeCacheCollisionsWithIncludeMappingsFalseViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyDecimal", false);

            // Act
            var actual = PropertyValueAttributeCache.Get(classProperty.PropertyInfo, false);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        #endregion
    }
}
