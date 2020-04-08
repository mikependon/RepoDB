using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class ClassMapperTest
    {
        private class ClassMapperTestClass
        {
        }

        [Map("[dbo].[Person]")]
        private class ClassMapperTestWithMapAttribute
        {
        }

        [TestMethod]
        public void TestClassMapperMapping()
        {
            // Setyp
            ClassMapper.Add<ClassMapperTestClass>("[edibles].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestClass>();
            var expected = "[edibles].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestClassMapperMappingWithMapAttribute()
        {
            // Setyp
            ClassMapper.Add<ClassMapperTestWithMapAttribute>("[edibles].[Person]");

            // Act
            var actual = ClassMappedNameCache.Get<ClassMapperTestWithMapAttribute>();
            var expected = "[dbo].[Person]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
