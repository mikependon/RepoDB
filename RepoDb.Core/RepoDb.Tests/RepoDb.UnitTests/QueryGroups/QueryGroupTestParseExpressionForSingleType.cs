using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Single

        [TestMethod]
        public void TestParseExpressionSingleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == 1).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleVariable()
        {
            // Setup
            var value = 1;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertySingle = 1
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value.PropertySingle).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == GetSingleValueForParseExpression()).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionSingleVariableMethodCall()
        {
            // Setup
            var value = GetSingleValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
