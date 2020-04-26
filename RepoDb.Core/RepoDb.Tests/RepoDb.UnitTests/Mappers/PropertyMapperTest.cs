using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;

namespace RepoDb.UnitTests.Mappers
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
            PropertyMapper.Clear();
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
            var propertyName = "ColumnString";
            PropertyMapper.Add<PropertyMapperTestClass>(propertyName, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(propertyName);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaField()
        {
            // Setup
            var field = new Field("ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(field);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpression()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(e => e.ColumnString);
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
            var propertyName = "PropertyString";
            PropertyMapper.Add<PropertyMapperTestClass>(propertyName, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(propertyName);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(field, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(field);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.PropertyString, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(e => e.PropertyString);
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
            var propertyName = "ColumnString";
            PropertyMapper.Add<PropertyMapperTestClass>(propertyName, "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(propertyName, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(propertyName);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldOverride()
        {
            // Setup
            var field = new Field("ColumnString");
            PropertyMapper.Add<PropertyMapperTestClass>(field, "PropertyString", true);
            PropertyMapper.Add<PropertyMapperTestClass>(field, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(field);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionOverride()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperTestClass>(e => e.ColumnString);
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

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>((string)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>((Field)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(expression: null, columnName: "PropertyText");
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>("Whatever", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("Whatever"), "PropertyText");
        }

        /*
         * Null ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, null);
        }

        /*
         * Empty ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "");
        }

        /*
         * Empty-spaces ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>("ColumnString", "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(new Field("ColumnString"), "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperTestClass>(e => e.ColumnString, "  ");
        }

        #endregion
    }
}
