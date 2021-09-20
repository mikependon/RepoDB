using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes.Parameter;
using System.Collections.Generic;
using System.Data;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class PropertyValueAttributeMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyValueAttributeMapper.Clear();
        }

        #region SubClasses

        private class PropertyValueAttributeClass
        {
            public string PropertyString { get; set; }

            [
                Name("Column3"),
                DbType(DbType.AnsiStringFixedLength),
                Direction(ParameterDirection.InputOutput),
                IsNullable(true),
                Precision(100),
                Scale(2),
                Size(256)
            ]
            public string MappedPropertyString { get; set; }
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
         * No MapAttribute
         */

        [TestMethod]
        public void TestPropertyValueAttributeMapperViaPropertyName()
        {
            // Prepare
            var propertyName = "PropertyString";
            var attributes = GetPropertyValueAttributes();

            // Act
            PropertyValueAttributeMapper.Add<PropertyValueAttributeClass>(propertyName,
                attributes);
            var actual = PropertyValueAttributeMapper.Get<PropertyValueAttributeClass>(propertyName);

            // Assert
            Assert.AreEqual(attributes, actual);
        }

        [TestMethod]
        public void TestPropertyValueAttributeMapperViaField()
        {
            // Prepare
            var field = new Field("PropertyString");
            var attributes = GetPropertyValueAttributes();

            // Act
            PropertyValueAttributeMapper.Add<PropertyValueAttributeClass>(field,
                attributes);
            var actual = PropertyValueAttributeMapper.Get<PropertyValueAttributeClass>(field);

            // Assert
            Assert.AreEqual(attributes, actual);
        }

        [TestMethod]
        public void TestPropertyValueAttributeMapperViaExpression()
        {
            // Prepare
            var attributes = GetPropertyValueAttributes();

            // Act
            PropertyValueAttributeMapper.Add<PropertyValueAttributeClass>(e => e.PropertyString,
                attributes);
            var actual = PropertyValueAttributeMapper.Get<PropertyValueAttributeClass>(e => e.PropertyString);

            // Assert
            Assert.AreEqual(attributes, actual);
        }

        [TestMethod]
        public void TestPropertyValueAttributeMapperViaPropertyInfo()
        {
            // Prepare
            var classProperty = PropertyCache.Get<PropertyValueAttributeClass>("PropertyString");
            var attributes = GetPropertyValueAttributes();

            // Act
            PropertyValueAttributeMapper.Add(classProperty.PropertyInfo,
                attributes);
            var actual = PropertyValueAttributeMapper.Get(classProperty.PropertyInfo);

            // Assert
            Assert.AreEqual(attributes, actual);
        }

        #endregion
    }
}
