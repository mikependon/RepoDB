using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System.Data;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class ClassMapTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup();
        }

        private static void Setup()
        {
            new ClassMapperTestClassMap();
            new ClassMapperTestWithAttributesClassMap();
        }

        #region SubClasses

        private class ClassMapperTestClass
        {
            public int PrimaryId { get; set; }

            public int RowId { get; set; }

            public string PropertyString { get; set; }
        }

        [Map("[dbo].[Table]")]
        private class ClassMapperTestWithAttributesClass
        {
            [Map("Id"), Primary, Identity]
            public int PrimaryId { get; set; }

            [Map("ColumnString"), TypeMap(DbType.String)]
            public string PropertyString { get; set; }
        }

        private class ClassMapperTestClassMap :
            ClassMap<ClassMapperTestClass>
        {
            public ClassMapperTestClassMap()
            {
                Table("[schema].[Table]");
                Primary(e => e.PrimaryId);
                Identity(e => e.PrimaryId);
                Property(e => e.PropertyString)
                    .Column("ColumnString")
                    .DbType(DbType.String);
            }
        }

        private class ClassMapperTestWithAttributesClassMap :
            ClassMap<ClassMapperTestWithAttributesClass>
        {
            public ClassMapperTestWithAttributesClassMap()
            {
                Table("[schema].[Table]");
                Primary(e => e.PrimaryId);
                Identity(e => e.PrimaryId);
                Property(e => e.PropertyString)
                    .Column("ColumnNVarChar")
                    .DbType(DbType.StringFixedLength);
            }
        }

        #endregion

        #region Methods

        #region Table

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestClassMapMapping()
        {
            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestClass>();
            var expected = "[schema].[Table]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestClassMapMappingWithMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestWithAttributesClass>();
            var expected = "[dbo].[Table]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        //[TestMethod]
        //public void TestClassMapMappingOverride()
        //{
        //    // Setup
        //    ClassMapper.Add<ClassMapperTestClass>("[sc].[Table]", true);

        //    // Act
        //    var actual = ClassMappedNameCache.Get<ClassMapperTestClass>();
        //    var expected = "[sc].[Table]";

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnClassMappingThatIsAlreadyExisting()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithAttributesClass>("[sc].[Table]");
        }

        #endregion

        #region Primary

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestClassMapPrimaryMapping()
        {
            // Act
            var actual = PrimaryCache.Get<ClassMapperTestClass>();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestClassMapPrimaryMappingWithMapAttribute()
        {
            // Act
            var actual = PrimaryCache.Get<ClassMapperTestWithAttributesClass>();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override True
         */

        //[TestMethod]
        //public void TestClassMapPrimaryMappingOverride()
        //{
        //    // Setup
        //    PrimaryMapper.Add<ClassMapperTestClass>(e => e.RowId, true);

        //    // Act
        //    var actual = PrimaryCache.Get<ClassMapperTestClass>();
        //    var expected = "RowId";

        //    // Assert
        //    Assert.AreEqual(expected, actual.GetMappedName());
        //}

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnClassMapPrimaryMappingThatIsAlreadyExisting()
        {
            // Setup
            PrimaryMapper.Add<ClassMapperTestClass>(e => e.RowId);
        }

        #endregion

        #region Identity

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestClassMapIdentityMapping()
        {
            // Act
            var actual = IdentityCache.Get<ClassMapperTestClass>();
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestClassMapIdentityMappingWithMapAttribute()
        {
            // Act
            var actual = IdentityMapper.Get<ClassMapperTestWithAttributesClass>();
            var expected = "Id";

            // Assert
            Assert.AreEqual(expected, actual.GetMappedName());
        }

        /*
         * Override True
         */

        //[TestMethod]
        //public void TestClassMapIdentityMappingOverride()
        //{
        //    // Setup
        //    IdentityMapper.Add<ClassMapperTestClass>(e => e.RowId, true);

        //    // Act
        //    var actual = IdentityCache.Get<ClassMapperTestClass>();
        //    var expected = "RowId";

        //    // Assert
        //    Assert.AreEqual(expected, actual.GetMappedName());
        //}

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnClassMapIdentityMappingThatIsAlreadyExisting()
        {
            // Setup
            IdentityMapper.Add<ClassMapperTestClass>(e => e.RowId);
        }

        #endregion

        #region DbType

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestClassMapDbTypeMapping()
        {
            // Act
            var actual = TypeMapCache.Get<ClassMapperTestClass>(e => e.PropertyString);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestClassMapDbTypeMappingWithMapAttribute()
        {
            // Act
            var actual = TypeMapCache.Get<ClassMapperTestWithAttributesClass>(e => e.PropertyString);
            var expected = DbType.String;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        //[TestMethod]
        //public void TestClassMapDbTypeMappingOverride()
        //{
        //    // Setup
        //    TypeMapper.Add<ClassMapperTestClass>(e => e.PropertyString, DbType.AnsiString, true);

        //    // Act
        //    var actual = TypeMapCache.Get<ClassMapperTestClass>();
        //    var expected = DbType.AnsiString;

        //    // Assert
        //    Assert.AreEqual(expected, actual);
        //}

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnClassMapDbTypeMappingThatIsAlreadyExisting()
        {
            // Setup
            TypeMapper.Add<ClassMapperTestClass>(e => e.PropertyString, DbType.AnsiString);
        }

        #endregion

        #endregion
    }
}
