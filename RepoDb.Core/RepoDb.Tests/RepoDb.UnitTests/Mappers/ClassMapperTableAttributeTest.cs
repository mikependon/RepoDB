using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
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

        [Table("[Person]", Schema = "[dbo]")]
        private class ClassMapperTableAttributeTestClass
        {
        }

        [Table("[Person]", Schema = "[dbo]"), Map("[sales].[Person]")]
        private class ClassMapperTableAndMapAttributeCollisionTestClass
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
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");

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
        public void TestClassMapperViaTableMappingOverride()
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
        public void ThrowExceptionOnClassMapperViaTableThatIsAlreadyExisting()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("[sales].[Person]");
        }

        /*
         * Empty/Null
         */

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperViaTableThatIsEmpty()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperViaTableThatIsEmptySpaces()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>("  ");
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnClassMapperViaTableThatIsNull()
        {
            // Setup
            ClassMapper.Add<ClassMapperTableAttributeTestClass>(null);
        }

        /*
         * Collision
         */

        [TestMethod]
        public void TestClassMappingWithTableAndMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTableAndMapAttributeCollisionTestClass>();
            var expected = "[sales].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
