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
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
