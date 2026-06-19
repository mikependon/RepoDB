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

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyString");
            Assert.Throws<MappingExistsException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "PropertyText"));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyString");
            Assert.Throws<MappingExistsException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "PropertyText"));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionThatIsAlreadyExisting()
        {
            // Setup
            PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyString");
            Assert.Throws<MappingExistsException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "PropertyText"));
        }

        /*
         * Null Properties
         */

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsNull()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>((string)null, "PropertyText"));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsNull()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>((Field)null, "PropertyText"));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionThatIsNull()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(expression: null, columnName: "PropertyText"));
        }

        /*
         * Missing Properties
         */

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameThatIsMissing()
        {
            // Setup
            Assert.Throws<PropertyNotFoundException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>("Whatever", "PropertyText"));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldThatIsIsMissing()
        {
            // Setup
            Assert.Throws<PropertyNotFoundException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("Whatever"), "PropertyText"));
        }

        /*
         * Null ColumnName
         */

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithNullTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", null));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithNullTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), null));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithNullTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, null));
        }

        /*
         * Empty ColumnName
         */

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithEmptyTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", ""));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithEmptyTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), ""));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithEmptyTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, ""));
        }

        /*
         * Empty-spaces ColumnName
         */

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaPropertyNameWithEmptySpacesTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>("ColumnString", "  "));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaFieldWithEmptySpacesTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(new Field("ColumnString"), "  "));
        }

        [TestMethod]
        public void ThrowExceptionOnPropertyMapperViaColumnAttributeViaExpressionWithEmptySpacesTargetColumnName()
        {
            // Setup
            Assert.Throws<NullReferenceException>(() => PropertyMapper.Add<PropertyMapperColumnAttributeClass>(e => e.ColumnString, "  "));
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
