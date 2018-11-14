using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Boolean

        [TestMethod]
        public void TestParseExpressionBooleanConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == true).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanVariable()
        {
            // Setup
            var value = true;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyBoolean = true
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value.PropertyBoolean).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == GetBooleanValueForParseExpression()).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionBooleanVariableMethodCall()
        {
            // Setup
            var value = GetBooleanValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBoolean == value).GetString();
            var expected = "([PropertyBoolean] = @PropertyBoolean)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
