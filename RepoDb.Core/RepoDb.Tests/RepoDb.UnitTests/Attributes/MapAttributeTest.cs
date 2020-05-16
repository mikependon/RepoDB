using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;

namespace RepoDb.UnitTests.Attributes
{
    [TestClass]
    public class MapAttributeTest
    {
        [Map("Name")]
        private class TestMapAttributeUnquotedNameClass
        {
            [Map("PrimaryId")]
            public int Id { get; set; }
        }

        [Map("[dbo].[Name]")]
        private class TestMapAttributeQuotedNameClass
        {
            [Map("[PrimaryId]")]
            public int Id { get; set; }
        }

        /*
         * Unquoted
         */

        [TestMethod]
        public void TestMapAttributeForClass()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestMapAttributeUnquotedNameClass>();
            var expected = "Name";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyViaExpression()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeUnquotedNameClass>(e => e.Id);
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyViaPropertyName()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeUnquotedNameClass>("Id");
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyViaField()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeUnquotedNameClass>(new Field("Id"));
            var expected = "PrimaryId";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
         * Quoted
         */

        [TestMethod]
        public void TestMapAttributeForPropertyWithQuotedName()
        {
            // Act
            var actual = ClassMappedNameCache.Get<TestMapAttributeQuotedNameClass>();
            var expected = "[dbo].[Name]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyWithQuotedNameViaExpression()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeQuotedNameClass>(e => e.Id);
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyWithQuotedNameViaPropertyName()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeQuotedNameClass>("Id");
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMapAttributeForPropertyWithQuotedNameViaField()
        {
            // Act
            var actual = PropertyMappedNameCache.Get<TestMapAttributeQuotedNameClass>(new Field("Id"));
            var expected = "[PrimaryId]";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
