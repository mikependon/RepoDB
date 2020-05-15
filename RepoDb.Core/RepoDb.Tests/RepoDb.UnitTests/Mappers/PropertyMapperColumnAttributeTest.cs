using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class PropertyMapperColumnAttributeTest
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

        private class PropertyMapperColumnAttributeClass
        {
            public string ColumnString { get; set; }
            [Column("PropertyText")]
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
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(propertyName, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(propertyName);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaField()
        {
            // Setup
            var field = new Field("ColumnString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(field);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpression()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(e => e.ColumnString);
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }


        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaPropertyNameWithColumnAttribute()
        {
            // Setup
            var propertyName = "PropertyString";
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(propertyName, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(propertyName);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldWithMapAttribute()
        {
            // Setup
            var field = new Field("PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(field, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(field);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionWithMapAttribute()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.PropertyString, "ColumnText");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(e => e.PropertyString);
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
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(propertyName, "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(propertyName, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(propertyName);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaFieldOverride()
        {
            // Setup
            var field = new Field("ColumnString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(field, "PropertyString", true);
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(field, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(field);
            var expected = "PropertyText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaExpressionOverride()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyText", true);

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAttributeClass>(e => e.ColumnString);
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
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyText");
        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>((string)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>((Field)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(expression: null, columnName: "PropertyText");
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameThatIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("Whatever", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaFieldThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("Whatever"), "PropertyText");
        }

        /*
         * Null ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, null);
        }

        /*
         * Empty ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "");
        }

        /*
         * Empty-spaces ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaPropertyNameWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaFieldWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaExpressionWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "  ");
        }

        #endregion
    }
}
