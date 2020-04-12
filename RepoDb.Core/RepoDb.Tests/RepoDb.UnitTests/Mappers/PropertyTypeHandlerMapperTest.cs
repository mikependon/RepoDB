using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using RepoDb.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class PropertyTypeHandlerMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyTypeHandlerCache.Flush();
            PropertyTypeHandlerMapper.Flush();
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

        private class PropertyTypeHandlerMapperTestClass
        {
            public string ColumnString { get; set; }
            [PropertyHandler(typeof(TextPropertyHandler))]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region Type Level

        [TestMethod]
        public void PropertyTypeHandlerMapperTypeMapping()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<string, StringPropertyHandler>(stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<string, StringPropertyHandler>();
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperTypeMappingWithoutMapping()
        {
            // Act
            var actual = PropertyTypeHandlerMapper.Get<DateTime, object>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region Property Level

        /*
         * No PropertyHandlerAttribute
         */

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyName()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyName = "ColumnString";
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaField()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var field = new Field("ColumnString");
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaExpression()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaClassProperty()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var classProperty = PropertyCache.Get<PropertyTypeHandlerMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(classProperty, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<StringPropertyHandler>(classProperty);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyInfo()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyInfo = typeof(PropertyTypeHandlerMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(propertyInfo, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<StringPropertyHandler>(propertyInfo);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With PropertyHandlerAttribute
         */

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyNameWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyName = "PropertyString";
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyTypeHandlerCache.Get<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaFieldWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var field = new Field("PropertyString");
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyTypeHandlerCache.Get<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaExpressionWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.PropertyString, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyTypeHandlerCache.Get<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaClassPropertyWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var classProperty = PropertyCache.Get<PropertyTypeHandlerMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "PropertyString");
            PropertyTypeHandlerMapper.Add(classProperty, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<StringPropertyHandler>(classProperty);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyTypeHandlerCache.Get<TextPropertyHandler>(classProperty);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyInfoWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyInfo = typeof(PropertyTypeHandlerMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "PropertyString");
            PropertyTypeHandlerMapper.Add(propertyInfo, stringPropertyHandler);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<StringPropertyHandler>(propertyInfo);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyTypeHandlerCache.Get<TextPropertyHandler>(propertyInfo);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        /*
         * Override
         */

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyNameOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var propertyName = "ColumnString";
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(propertyName, textPropertyHandler, true);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, object>(propertyName);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaFieldOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var field = new Field("ColumnString");
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(field, textPropertyHandler, true);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, object>(field);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaExpressionOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(e => e.ColumnString, textPropertyHandler, true);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<PropertyTypeHandlerMapperTestClass, object>(e => e.ColumnString);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaClassPropertyOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var classProperty = PropertyCache.Get<PropertyTypeHandlerMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(classProperty, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add(classProperty, textPropertyHandler, true);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<object>(classProperty);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void PropertyTypeHandlerMapperPropertyMappingViaPropertyInfoOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var propertyInfo = typeof(PropertyTypeHandlerMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(propertyInfo, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add(propertyInfo, textPropertyHandler, true);

            // Act
            var actual = PropertyTypeHandlerMapper.Get<object>(propertyInfo);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var propertyName = "ColumnString";
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(propertyName, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var field = new Field("ColumnString");
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(field, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, TextPropertyHandler>(e => e.ColumnString, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaClassPropertyThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var classProperty = PropertyCache.Get<PropertyTypeHandlerMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(classProperty, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add(classProperty, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyInfoThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var propertyInfo = typeof(PropertyTypeHandlerMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add(propertyInfo, stringPropertyHandler);
            PropertyTypeHandlerMapper.Add(propertyInfo, textPropertyHandler);
        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>((string)null, stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>((Field)null, stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(expression: null, propertyHandler: stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaClassPropertyThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add((ClassProperty)null, stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyInfoThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add((PropertyInfo)null, stringPropertyHandler);
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>("Whatever", stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsIsMissing()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(new Field("Whatever"), stringPropertyHandler);
        }

        /*
         * Null PropertyHandler
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameWithNullPropertyHandler()
        {
            // Setup
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldWithNullPropertyHandler()
        {
            // Setup
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionWithNullPropertyHandler()
        {
            // Setup
            PropertyTypeHandlerMapper.Add<PropertyTypeHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaClassPropertyWithNullPropertyHandler()
        {
            // Setup
            var classProperty = PropertyCache.Get<PropertyTypeHandlerMapperTestClass>()
                .First(p => p.PropertyInfo.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add<StringPropertyHandler>(classProperty, null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyInfoWithNullPropertyHandler()
        {
            // Setup
            var propertyInfo = typeof(PropertyTypeHandlerMapperTestClass)
                .GetProperties()
                .First(p => p.Name == "ColumnString");
            PropertyTypeHandlerMapper.Add<StringPropertyHandler>(propertyInfo, null);
        }

        #endregion

        #endregion
    }
}
