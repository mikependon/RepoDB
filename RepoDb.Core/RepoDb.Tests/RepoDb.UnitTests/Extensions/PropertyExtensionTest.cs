using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using RepoDb.Extensions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Extensions
{
    [TestClass]
    public class PropertyExtensionTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            PropertyValueAttributeMapper.Clear();
        }

        #region SubClasses

        private class PropertyValueAttributeClass
        {
            [
                Name("ColumnString"),
                DbType(DbType.AnsiStringFixedLength),
                Direction(ParameterDirection.InputOutput),
                IsNullable(true),
                Precision(100),
                Scale(2),
                Size(256)
            ]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Helpers

        private IEnumerable<PropertyValueAttribute> GetPropertyValueAttributes() =>
            new PropertyValueAttribute[]
            {
                // Different Values
                new NameAttribute("MappedColumnString"),
                new DbTypeAttribute(DbType.StringFixedLength),
                new DirectionAttribute(ParameterDirection.ReturnValue),
                new SizeAttribute(512),
                // Same Values
                new IsNullableAttribute(true),
                new PrecisionAttribute(100),
                new ScaleAttribute(2)
            };

        #endregion

        [TestMethod]
        public void TestPropertyInfoGetPropertyValueAttributesMethod()
        {
            // Prepare
            var property = PropertyCache.Get<PropertyValueAttributeClass>(e => e.PropertyString);

            // Act
            var attributes = property.PropertyInfo.GetPropertyValueAttributes(false);

            // Assert
            Assert.AreEqual(7, attributes.Count());
        }

        [TestMethod]
        public void TestPropertyInfoGetPropertyValueAttributeMethod()
        {
            // Prepare
            var property = PropertyCache.Get<PropertyValueAttributeClass>(e => e.PropertyString);

            // Act
            var attribute = property.PropertyInfo.GetPropertyValueAttribute<NameAttribute>(false);

            // Assert
            Assert.IsNotNull(attribute);
            Assert.AreEqual("ColumnString", attribute.Name);
        }
    }
}
