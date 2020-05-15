using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests.Mappers
{
    [TestClass]
    public partial class ClassMapperTableAttributeTest
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
            ClassMapper.Clear();
        }

        #region SubClasses

        [Table("[dbo].[Person]")]
        private class ClassMapperTableAttributeTestClass
        {
        }

        #endregion

        #region Methods

        /*
         * TableAttribute
         */

        [TestMethod]
        public void TestClassMappingWithTableAttribute()
        {
            // Setup
            ClassMapper.Add <ClassMapperTableAttributeTestClass>("[sales].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTableAttributeTestClass>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Override - does not override.
         */

        [TestMethod]
        public void TestClassMapperMappingOverride()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[hr].[Person]", true);

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTableAttributeTestClass>();
            var expected = "[dbo].[Person]";

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
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");
        }

        /*
         * Empty/Null
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsEmpty()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsEmptySpaces()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperThatIsNull()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>(null);
        }

        #endregion
    }
}
