using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class ClassMappingTest
    {
        private class Fruit
        {
        }

        [Map("[dbo].[Fruit]")]
        private class Whatever
        {
        }

        [TestMethod]
        public void TestWithoutMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<Fruit>();
            var expected = "Fruit";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttribute()
        {
            // Act
            var actual = ClassMappedNameCache.Get<Whatever>();
            var expected = "[dbo].[Fruit]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithClassMapperMapping()
        {
            // Setyp
            ClassMapper.Add<Fruit>("[edibles].[Fruit]");

            // Act
            var actual = ClassMappedNameCache.Get<Fruit>();
            var expected = "[edibles].[Fruit]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestWithMapAttributeAndWithClassMapperMapping()
        {
            // Setyp
            ClassMapper.Add<Whatever>("[edibles].[Fruit]");

            // Act
            var actual = ClassMappedNameCache.Get<Whatever>();
            var expected = "[dbo].[Fruit]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
