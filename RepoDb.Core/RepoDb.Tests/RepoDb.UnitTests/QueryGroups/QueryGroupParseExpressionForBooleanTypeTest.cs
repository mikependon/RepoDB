using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Boolean

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == true).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanVariable()
        {
            // Setup
            var value = true;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyBoolean = true
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value.PropertyBoolean).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == GetBooleanValueForParseExpression()).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanVariableMethodCall()
        {
            // Setup
            var value = GetBooleanValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanNegation()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyBoolean).GetString(m_dbSetting);
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanNegationWithOtherPropertyForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != "A" && !e.PropertyBoolean).GetString(m_dbSetting);
            var expected = "(([PropertyString] <> @PropertyString) AND ([PropertyBoolean] = @PropertyBoolean))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionBooleanNegationWithOtherPropertyForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString != "A" || !e.PropertyBoolean).GetString(m_dbSetting);
            var expected = "(([PropertyString] <> @PropertyString) OR ([PropertyBoolean] = @PropertyBoolean))";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
