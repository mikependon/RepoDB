using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Single

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == 1).GetString(m_dbSetting);
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleVariable()
        {
            // Setup
            var value = 1;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value).GetString(m_dbSetting);
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertySingle = 1
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value.PropertySingle).GetString(m_dbSetting);
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == GetSingleValueForParseExpression()).GetString(m_dbSetting);
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleVariableMethodCall()
        {
            // Setup
            var value = GetSingleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value).GetString(m_dbSetting);
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
