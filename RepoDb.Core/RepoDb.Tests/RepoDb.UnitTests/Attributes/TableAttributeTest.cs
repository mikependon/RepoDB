using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class TableAttributeTest
    {
        [Table("Name")]
        private class TestTableAttributeUnquotedNameClass
        {
        }

        [Table("[dbo].[Name]")]
        private class TestTableAttributeQuotedNameClass
        {
        }

        [TestMethod]
        public void TestTableAttributeName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestTableAttributeUnquotedNameClass>();
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestTableAttributeUnquotedName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestTableAttributeQuotedNameClass>();
            var expected = "[dbo].[Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
