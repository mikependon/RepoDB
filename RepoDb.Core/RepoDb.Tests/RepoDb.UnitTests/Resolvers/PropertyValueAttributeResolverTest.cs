using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Resolvers;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Cachers
{
    [TestClass]
    public partial class PropertyValueAttributeResolverTest
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
        public void TestPropertyValueAttributeResolverWithAttributesViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyInt", true);

            // Act
            var actual = new PropertyValueAttributeResolver().Resolve(classProperty.PropertyInfo);

            // Assert
            Assert.AreEqual(2, actual.Count());
        }

        /*
         * Without Attributes
         */

        [TestMethod]
        public void TestPropertyValueAttributeResolverWithMappedAttributesViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyString", true);

            // Act
            var actual = new PropertyValueAttributeResolver().Resolve(classProperty.PropertyInfo, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeResolverWithMappedAttributesAndWithIncludeMappingsFalseViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyString", true);

            // Act
            var actual = new PropertyValueAttributeResolver().Resolve(classProperty.PropertyInfo, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        /*
         * Attribute Collisions
         */

        [TestMethod]
        public void TestPropertyValueAttributeResolverCollisionsViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyDecimal", true);

            // Act
            var actual = new PropertyValueAttributeResolver().Resolve(classProperty.PropertyInfo, true);

            // Assert
            Assert.AreEqual(11, actual.Count());
        }

        [TestMethod]
        public void TestPropertyValueAttributeResolverCollisionsWithIncludeMappingsFalseViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyDecimal", true);

            // Act
            var actual = new PropertyValueAttributeResolver().Resolve(classProperty.PropertyInfo, false);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        #endregion
    }
}
