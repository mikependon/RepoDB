using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // EndsWith

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A"), Helper.DbSetting);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.EndsWith("A"), Helper.DbSetting);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A") == true, Helper.DbSetting);

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.EndsWith("A") == false, Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithForMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString.EndsWith("A"), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithForQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.QuotedPropertyString.EndsWith("A"), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionEndsWithForUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.UnorganizedPropertyString.EndsWith("A"), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([Property / . String] LIKE @Property_____String)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
