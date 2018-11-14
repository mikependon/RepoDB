using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Double

        [TestMethod]
        public void TestParseExpressionDoubleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == 1.0).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleVariable()
        {
            // Setup
            var value = 1.0;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyDouble = 1.0
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value.PropertyDouble).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == GetDoubleValueForParseExpression()).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDoubleVariableMethodCall()
        {
            // Setup
            var value = GetDoubleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDouble == value).GetString();
            var expected = "([PropertyDouble] = @PropertyDouble)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
