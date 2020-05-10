using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System.Data;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class FluentMapperTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup();
        }

        private static void Setup()
        {
            FluentMapper
                .Entity<FluentMapperTestClass>()
                .Table("[schema].[Table]")
                .Primary(e => e.PrimaryId)
                .Identity(e => e.PrimaryId)
                .Column(e => e.PropertyString, "ColumnString")
                .DbType(e => e.PropertyString, DbType.String);

            FluentMapper
                .Entity<FluentMapperTestWithAttributesClass>()
                .Table("[schema].[Table]")
                .Primary(e => e.PrimaryId)
                .Identity(e => e.PrimaryId)
                .Column(e => e.PropertyString, "ColumnNVarChar")
                .DbType(e => e.PropertyString, DbType.StringFixedLength);
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
            public string PropertyString { get; set; }
        }

        #endregion

        #region Methods

        #region Table

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestFluentMapMapping()
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
        public void TestFluentMapMappingWithMapAttribute()
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
        public void TestFluentMapMappingOverride()
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
        public void ThrowExceptionOnFluentMappingThatIsAlreadyExisting()
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

        ///*
        // * Override True
        // */

        //[TestMethod]
        //public void TestFluentMapPrimaryMappingOverride()
        //{
        //    // Setup
        //    FluentMapper
        //        .Entity<FluentMapperTestClass>()
        //        .Primary(e => e.RowId, true);

        //    // Act
        //    var actual = PrimaryCache.Get<FluentMapperTestClass>();
        //    var expected = "RowId";

        //    // Assert
        //    Assert.AreEqual(expected, actual.GetMappedName());
        //}

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

        ///*
        // * Override True
        // */

        //[TestMethod]
        //public void TestFluentMapIdentityMappingOverride()
        //{
        //    // Setup
        //    FluentMapper
        //        .Entity<FluentMapperTestClass>()
        //        .Identity(e => e.RowId, true);

        //    // Act
        //    var actual = IdentityCache.Get<FluentMapperTestClass>();
        //    var expected = "RowId";

        //    // Assert
        //    Assert.AreEqual(expected, actual.GetMappedName());
        //}

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

        ///*
        // * Override True
        // */

        //[TestMethod]
        //public void TestFluentMapDbTypeMappingOverride()
        //{
        //    // Setup
        //    FluentMapper
        //        .Entity<FluentMapperTestClass>()
        //        .DbType(e => e.PropertyString, DbType.AnsiString, true);

        //    // Act
        //    var actual = TypeMapCache.Get<FluentMapperTestClass>();
        //    var expected = DbType.AnsiString;

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

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

        #endregion
    }
}
