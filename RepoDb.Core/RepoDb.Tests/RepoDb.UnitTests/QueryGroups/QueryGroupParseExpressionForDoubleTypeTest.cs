using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Double

        [TestMethod]
        public void TestQueryGroupParseExpressionDoubleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == 1.0).GetString(m_dbSetting);
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDoubleVariable()
        {
            // Setup
            var value = 1.0;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value).GetString(m_dbSetting);
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDoubleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyDouble = 1.0
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value.PropertyDouble).GetString(m_dbSetting);
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDoubleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == GetDoubleValueForParseExpression()).GetString(m_dbSetting);
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDoubleVariableMethodCall()
        {
            // Setup
            var value = GetDoubleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value).GetString(m_dbSetting);
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
