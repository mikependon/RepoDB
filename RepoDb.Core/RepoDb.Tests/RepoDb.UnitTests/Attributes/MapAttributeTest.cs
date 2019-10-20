using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class MapAttributeTest
    {
        [Map("Name")]
        public class TestMapAttributeUnquotedNameClass
        {
        }

        [Map("[dbo].[Name]")]
        public class TestMapAttributeQuotedNameClass
        {
        }

        [TestMethod]
        public void TestMapAttributeName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestMapAttributeUnquotedNameClass>();
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeUnquotedName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestMapAttributeQuotedNameClass>();
            var expected = "[dbo].[Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
