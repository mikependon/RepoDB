using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Attributes.Parameter;
using RepoDb.Exceptions;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class FluentMapperTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Table("[schema].[Table]")
                .Primary(e => e.PrimaryId)
                .Identity(e => e.PrimaryId)
                .Column(e => e.PropertyString, "ColumnString")
                .DbType(e => e.PropertyString, DbType.String)
                .PropertyValueAttributes(e => e.PropertyString, GetPropertyValueAttributes());

            FluentMapper
                .Entity<FluentMapperTestWithAttributesClass>()
                .Table("[schema].[Table]")
                .Primary(e => e.PrimaryId)
                .Identity(e => e.PrimaryId)
                .Column(e => e.PropertyString, "ColumnNVarChar")
                .DbType(e => e.PropertyString, DbType.StringFixedLength)
                .PropertyValueAttributes(e => e.PropertyString, GetPropertyValueAttributes());
        }

        [TestCleanup]
        public void Cleanup()
        {
            ClassMapper.Clear();
            PropertyMapper.Clear();
            PrimaryMapper.Clear();
            IdentityMapper.Clear();
            TypeMapper.Clear();
            //PropertyHandlerMapper.Clear();
            PropertyValueAttributeMapper.Clear();

            ClassMappedNameCache.Flush();
            PropertyCache.Flush();
            PrimaryCache.Flush();
            IdentityCache.Flush();
            TypeMapCache.Flush();
            //PropertyHandlerMapper.Clear();
            PropertyValueAttributeCache.Flush();
        }

        #region SubClasses

        private class FluentMapperTestClass
        {
            public int PrimaryId { get; set; }

            public int RowId { get; set; }

            public string PropertyString { get; set; }
        }

        [Map("[dbo].[Table]")]
        private class FluentMapperTestWithAttributesClass
        {
            [Map("Id"), Primary, Identity]
            public int PrimaryId { get; set; }

            [Map("ColumnString"), TypeMap(DbType.String)]
            [
                Name("ColumnDecimal"),
                DbType(DbType.Decimal),
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
                new NameAttribute("ColumnString"),
                new DbTypeAttribute(DbType.StringFixedLength),
                new DirectionAttribute(ParameterDirection.ReturnValue),
                new SizeAttribute(512),
                // Same Values
                new IsNullableAttribute(true),
                new PrecisionAttribute(100),
                new ScaleAttribute(2)
            };

        private IEnumerable<PropertyValueAttribute> GetPropertyValueAttributesForOverriding() =>
            new PropertyValueAttribute[]
            {
                // Different Values
                new NameAttribute("OverridingColumnString"),
                new DbTypeAttribute(DbType.AnsiString),
                new DirectionAttribute(ParameterDirection.InputOutput),
                new SizeAttribute(1024),
                // Same Values
                new IsNullableAttribute(false),
                new PrecisionAttribute(200),
                new ScaleAttribute(4)
            };

        #endregion

        #region Methods

        #region Table

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapTableMapping()
        {
            // Act
            var actual = ClassMappedNameCache.Get<FluentMapperTestClass>();
            var expected = "[schema].[Table]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapTableMappingWithMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<FluentMapperTestWithAttributesClass>();
            var expected = "[dbo].[Table]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapTableMappingOverride()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Table("[sc].[Table]", true);

            // Act
            var actual = ClassMapper.Get<FluentMapperTestClass>();
            var expected = "[sc].[Table]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapTableMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestWithAttributesClass>()
                .Table("[sc].[Table]");
        }

        #endregion

        #region Primary

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapPrimaryMapping()
        {
            // Act
            var actual = PrimaryCache.Get<FluentMapperTestClass>();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapPrimaryMappingWithMapAttribute()
        {
            // Act
            var actual = PrimaryCache.Get<FluentMapperTestWithAttributesClass>();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapPrimaryMappingOverride()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Primary(e => e.RowId, true);

            // Act
            var actual = PrimaryCache.Get<FluentMapperTestClass>();
            var expected = "RowId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapPrimaryMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Primary(e => e.RowId);
        }

        #endregion

        #region Identity

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapIdentityMapping()
        {
            // Act
            var actual = IdentityCache.Get<FluentMapperTestClass>();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapIdentityMappingWithMapAttribute()
        {
            // Act
            var actual = IdentityMapper.Get<FluentMapperTestWithAttributesClass>();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapIdentityMappingOverride()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Identity(e => e.RowId, true);

            // Act
            var actual = IdentityCache.Get<FluentMapperTestClass>();
            var expected = "RowId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapIdentityMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Identity(e => e.RowId);
        }

        #endregion

        #region Column

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapColumnMapping()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<FluentMapperTestClass>(e => e.PropertyString);
            var expected = "ColumnString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapColumnMappingWithMapAttribute()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<FluentMapperTestWithAttributesClass>(e => e.PropertyString);
            var expected = "ColumnString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapColumnMappingOverride()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Column(e => e.PropertyString, "ColumnStringOverriden", true);

            // Act
            var actual = PropertyMapper.Get<FluentMapperTestClass>(e => e.PropertyString);
            var expected = "ColumnStringOverriden";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapColumnMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestWithAttributesClass>()
                .Column(e => e.PropertyString, "ColumnStringOverriden");
        }

        #endregion

        #region DbType

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapDbTypeMapping()
        {
            // Act
            var actual = TypeMapCache.Get<FluentMapperTestClass>(e => e.PropertyString);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapDbTypeMappingWithMapAttribute()
        {
            // Act
            var actual = TypeMapCache.Get<FluentMapperTestWithAttributesClass>(e => e.PropertyString);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapDbTypeMappingOverride()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .DbType(e => e.PropertyString, DbType.AnsiString, true);

            // Act
            var actual = TypeMapCache.Get<FluentMapperTestClass>(e => e.PropertyString);
            var expected = DbType.AnsiString;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapDbTypeMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .DbType(e => e.PropertyString, DbType.AnsiString);
        }

        #endregion

        // PropertyHandler

        #region PropertyValueAttribute

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapPropertyValueAttributesMapping()
        {
            // Act
            var actual = PropertyValueAttributeCache
                .Get<FluentMapperTestClass>(e => e.PropertyString, true);

            // Assert
            Assert.AreEqual(7, actual.Count());
        }

        [TestMethod]
        public void TestFluentMapPropertyValueAttributesMappingWithIncludeMappingFalse()
        {
            // Act
            var actual = PropertyValueAttributeCache
                .Get<FluentMapperTestClass>(e => e.PropertyString, false);

            // Assert
            Assert.AreEqual(0, actual.Count());
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestFluentMapPropertyValueAttributesMappingWithMapAttribute()
        {
            // Act
            var actual = PropertyValueAttributeCache
                .Get<FluentMapperTestWithAttributesClass>(e => e.PropertyString, true);

            // Assert
            Assert.AreEqual(12, actual.Count());
        }

        [TestMethod]
        public void TestFluentMapPropertyValueAttributesMappingWithMapAttributeWithIncludeMappingFalse()
        {
            // Act
            var actual = PropertyValueAttributeCache
                .Get<FluentMapperTestWithAttributesClass>(e => e.PropertyString, false);

            // Assert
            Assert.AreEqual(8, actual.Count());
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestFluentMapPropertyValueAttributesMappingOverride()
        {
            // Prepare
            var attributes = GetPropertyValueAttributesForOverriding().ToList();

            // Setup
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .PropertyValueAttributes(e => e.PropertyString, attributes, true);

            // Act
            var actual = PropertyValueAttributeCache
                .Get<FluentMapperTestClass>(e => e.PropertyString, true).ToList();

            // Assert
            for (var i = 0; i < attributes.Count; i++)
            {
                Assert.AreEqual(attributes[i], actual[i]);
            }
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnFluentMapPropertyValueAttributesMappingThatIsAlreadyExisting()
        {
            // Setup
            FluentMapper
                .Entity<FluentMapperTestWithAttributesClass>()
                .PropertyValueAttributes(e => e.PropertyString, GetPropertyValueAttributes());
        }

        #endregion

        #endregion
    }
}
