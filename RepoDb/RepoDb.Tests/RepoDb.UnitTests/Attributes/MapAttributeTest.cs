using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Extensions;
using RepoDb.UnitTests.Setup;

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
            var actual = ClassMappedNameCache.Get<TestMapAttributeNameClass>(Helper.DbSetting);
            var expected = "Name".AsQuoted(true, Helper.DbSetting);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeUnquotedName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestMapAttributeNameClass>(false, Helper.DbSetting);
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
