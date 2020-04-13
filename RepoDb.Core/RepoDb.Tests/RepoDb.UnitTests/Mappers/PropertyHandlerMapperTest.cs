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
    public partial class PropertyHandlerMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            PropertyHandlerCache.Flush();
            PropertyHandlerMapper.Clear();
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

        private class PropertyHandlerMapperTestClass
        {
            public string ColumnString { get; set; }
            [PropertyHandler(typeof(TextPropertyHandler))]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region Type Level

        [TestMethod]
        public void TestPropertyHandlerMapperTypeMapping()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<string, StringPropertyHandler>(stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<string, StringPropertyHandler>();
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperTypeMappingWithoutMapping()
        {
            // Act
            var actual = PropertyHandlerMapper.Get<DateTime, object>();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region Property Level

        /*
         * No PropertyHandlerAttribute
         */

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaPropertyName()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyName = "ColumnString";
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaField()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var field = new Field("ColumnString");
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(field);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaExpression()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString);
            var expected = stringPropertyHandler;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With PropertyHandlerAttribute
         */

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaPropertyNameWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var propertyName = "PropertyString";
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyHandlerCache.Get<PropertyHandlerMapperTestClass, TextPropertyHandler>(propertyName);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaFieldWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var field = new Field("PropertyString");
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyHandlerCache.Get<PropertyHandlerMapperTestClass, TextPropertyHandler>(field);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaExpressionWithPropertyHandlerAttribute()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.PropertyString, stringPropertyHandler);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(actual);

            // Act
            var textHandler = PropertyHandlerCache.Get<PropertyHandlerMapperTestClass, TextPropertyHandler>(e => e.PropertyString);

            // Assert
            Assert.IsNotNull(textHandler);
        }

        /*
         * Override
         */

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaPropertyNameOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var propertyName = "ColumnString";
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(propertyName, textPropertyHandler, true);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, object>(propertyName);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaFieldOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var field = new Field("ColumnString");
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(field, textPropertyHandler, true);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, object>(field);
            var expected = (actual is TextPropertyHandler);

            // Assert
            Assert.IsTrue(expected);
        }

        [TestMethod]
        public void TestPropertyHandlerMapperPropertyMappingViaExpressionOverride()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(e => e.ColumnString, textPropertyHandler, true);

            // Act
            var actual = PropertyHandlerMapper.Get<PropertyHandlerMapperTestClass, object>(e => e.ColumnString);
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
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(propertyName, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(propertyName, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            var field = new Field("ColumnString");
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(field, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(field, textPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            var textPropertyHandler = new TextPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, stringPropertyHandler);
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, TextPropertyHandler>(e => e.ColumnString, textPropertyHandler);
        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>((string)null, stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>((Field)null, stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionThatIsNull()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(expression: null, propertyHandler: stringPropertyHandler);
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>("Whatever", stringPropertyHandler);
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldThatIsIsMissing()
        {
            // Setup
            var stringPropertyHandler = new StringPropertyHandler();
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(new Field("Whatever"), stringPropertyHandler);
        }

        /*
         * Null PropertyHandler
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaPropertyNameWithNullPropertyHandler()
        {
            // Setup
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaFieldWithNullPropertyHandler()
        {
            // Setup
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyTypeHandlerMapperViaExpressionWithNullPropertyHandler()
        {
            // Setup
            PropertyHandlerMapper.Add<PropertyHandlerMapperTestClass, StringPropertyHandler>(e => e.ColumnString, null);
        }

        #endregion

        #endregion
    }
}
