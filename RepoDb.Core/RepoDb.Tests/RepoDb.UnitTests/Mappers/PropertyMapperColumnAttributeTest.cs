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

        private class PropertyMapperColumnAndMapAttributeCollisionClass
        {
            public string ColumnString { get; set; }
            [Column("PropertyText"), Map("ColumnText")]
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        /*
         * No MapAttribute
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaColumnAttributeViaPropertyName()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaField()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaExpression()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaPropertyNameWithColumnAttribute()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaFieldWithMapAttribute()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaExpressionWithMapAttribute()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaPropertyNameOverride()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaFieldOverride()
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
        public void TestPropertyMapperMappingViaColumnAttributeViaExpressionOverride()
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
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyString");
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyText");
        }

        /*
         * Null Properties
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>((string)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>((Field)null, "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionThatIsNull()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(expression: null, columnName: "PropertyText");
        }

        /*
         * Missing Properties
         */

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("Whatever", "PropertyText");
        }

        [TestMethod, ExpectedException(typeof(PropertyNotFoundException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsIsMissing()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("Whatever"), "PropertyText");
        }

        /*
         * Null ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), null);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithNullTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, null);
        }

        /*
         * Empty ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithEmptyTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "");
        }

        /*
         * Empty-spaces ColumnName
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithEmptySpacesTargetColumnName()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "  ");
        }

        /*
         * Collision
         */

        [TestMethod]
        public void TestPropertyMapperMappingViaColumnAndMapAttributeViaExpression()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAndMapAttributeCollisionClass>(e => e.PropertyString);
            var expected = "ColumnText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaColumnAndMapAttributeViaPropertyName()
        {
            // Setup
            var propertyName = "PropertyString";

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAndMapAttributeCollisionClass>(propertyName);
            var expected = "ColumnText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestPropertyMapperMappingViaColumnAndMapAttributeViaField()
        {
            // Setup
            var field = new Field("PropertyString");

            // Act
            var actual = PropertyMappedNameCache.Get<PropertyMapperColumnAndMapAttributeCollisionClass>(field);
            var expected = "ColumnText";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
