using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class MapAttributeTest
    {
        [Map("Name")]
        public class TestMapAttributeNameClass
        {
        }

        [TestMethod]
        public void TestMapAttributeName()
        {
            // Act
            var actual = DataEntityExtension.GetMappedName<TestMapAttributeNameClass>();
            var expected = "Name".AsQuoted(true);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeUnquotedName()
        {
            // Act
            var actual = DataEntityExtension.GetMappedName<TestMapAttributeNameClass>(false);
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
