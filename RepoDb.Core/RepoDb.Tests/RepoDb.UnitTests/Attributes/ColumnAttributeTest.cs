using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class ColumnAttributeTest
    {
        private class TestColumnAttributeUnquotedNameClass
        {
            [Column("PrimaryId")]
            public int Id { get; set; }
        }

        private class TestColumnAttributeQuotedNameClass
        {
            [Column("[PrimaryId]")]
            public int Id { get; set; }
        }

        /*
         * Unquoted
         */

        [TestMethod]
        public void TestColumnAttributeViaExpression()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>(e => e.Id);
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnAttributeViaPropertyName()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>("Id");
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnAttributeViaField()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeUnquotedNameClass>(new Field("Id"));
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Quoted
         */

        [TestMethod]
        public void TestColumnAttributeQuotedNameViaExpression()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>(e => e.Id);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnAttributeQuotedNameViaPropertyName()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>("Id");
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnAttributeQuotedNameViaField()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestColumnAttributeQuotedNameClass>(new Field("Id"));
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
