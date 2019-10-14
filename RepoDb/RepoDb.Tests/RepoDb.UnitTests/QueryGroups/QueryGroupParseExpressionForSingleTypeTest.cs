using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Single

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == 1, Helper.DbSetting).GetString();
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
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value, Helper.DbSetting).GetString();
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
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value.PropertySingle, Helper.DbSetting).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionSingleMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == GetSingleValueForParseExpression(), Helper.DbSetting).GetString();
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
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertySingle == value, Helper.DbSetting).GetString();
            var expected = "([PropertySingle] = @PropertySingle)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
