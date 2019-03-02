using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // EndsWith

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A"));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotEndsWithAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.EndsWith("A"));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A") == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A") == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
