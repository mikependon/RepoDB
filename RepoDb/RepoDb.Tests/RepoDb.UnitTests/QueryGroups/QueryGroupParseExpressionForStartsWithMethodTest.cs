using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // StartsWith

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A"));

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStartsWithAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.StartsWith("A"));

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A") == true);

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.StartsWith("A") == false);

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithForMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString.StartsWith("A"));

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithForQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.QuotedPropertyString.StartsWith("A"));

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStartsWithForUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.UnorganizedPropertyString.StartsWith("A"));

            // Act
            var actual = parsed.GetString(Helper.DbSetting);
            var expected = "([Property / . String] LIKE @Property_____String)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
