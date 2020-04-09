using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Exceptions;
using System;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class ClassMapperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ClassMappedNameCache.Flush();
            ClassMapper.Flush();
        }

        #region SubClasses

        private class ClassMapperTestClass
        {
        }

        [Map("[dbo].[Person]")]
        private class ClassMapperTestWithMapAttribute
        {
        }

        #endregion

        #region Methods

        /* 
         * No MapAttribute
         */

        [TestMethod]
        public void TestClassMapperMapping()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestClass>("[sales].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestClass>();
            var expected = "[sales].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * With MapAttribute
         */

        [TestMethod]
        public void TestClassMapperMappingWithMapAttribute()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("[sales].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestWithMapAttribute>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override True
         */

        [TestMethod]
        public void TestClassMapperMappingOverride()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestClass>("[sales].[Person]");
            ClassMapper.Add<ClassMapperTestClass>("[hr].[Person]", true);

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestClass>();
            var expected = "[hr].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override False
         */

        [TestMethod, ExpectedException(typeof(MappingExistsException))]
        public void ThrowExceptionOnClassMapperThatIsAlreadyExisting()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("[sales].[Person]");
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("[sales].[Person]");
        }

        /*
         * Empty/Null
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsEmpty()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsEmptySpaces()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsNull()
        {
            // Setup
            ClassMapper.Add<ClassMapperTestWithMapAttribute>(null);
        }

        #endregion
    }
}
