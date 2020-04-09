using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class PropertyMapperTest
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
            PropertyMapper.Flush();
        }

        #region SubClasses

        private class PropertyMapperTestClass
        {
            public string ColumnString { get; set; }
            [Map("PropertyText")]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyName()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaField()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpression()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaClassProperty()
        {
            // Setup
            var classProperty = PropertyCache.Get<PropertyMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyMapper.Add(classProperty, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get(classProperty.PropertyInfo);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyInfo()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add(propertyInfo, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyNameWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>("PropertyString", "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("PropertyString"), "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.PropertyString, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaClassPropertyWithMapAttribute()
        {
            // Setup
            var classProperty = PropertyCache.Get<PropertyMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "PropertyString");
            PropertyMapper.Add(classProperty, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get(classProperty.PropertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyInfoWithMapAttribute()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            PropertyMapper.Add(propertyInfo, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyNameOverride()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldOverride()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionOverride()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaClassPropertyOverride()
        {
            // Setup
            var classProperty = PropertyCache.Get<PropertyMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyMapper.Add(classProperty, "PropertyString");
            PropertyMapper.Add(classProperty, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get(classProperty.PropertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyInfoOverride()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add(propertyInfo, "PropertyString");
            PropertyMapper.Add(propertyInfo, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get(propertyInfo);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaClassPropertyThatIsAlreadyExisting()
        {
            // Setup
            var classProperty = PropertyCache.Get<PropertyMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyMapper.Add(classProperty, "PropertyString");
            PropertyMapper.Add(classProperty, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyInfoThatIsAlreadyExisting()
        {
            // Setup
            var propertyInfo = typeof(PropertyMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyMapper.Add(propertyInfo, "PropertyString");
            PropertyMapper.Add(propertyInfo, "PropertyText");
        }

        /*
         * Missing Property
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(propertyName: null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(field: null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(expression: null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaClassPropertyThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add((ClassProperty)null, "PropertyString");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyInfoThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add((PropertyInfo)null, "PropertyString");
        }

        #endregion
    }
}
