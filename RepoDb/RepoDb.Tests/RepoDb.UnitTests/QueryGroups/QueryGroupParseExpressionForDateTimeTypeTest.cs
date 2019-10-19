using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // DateTime

        [TestMethod]
        public void TestQueryGroupParseExpressionDateTimeConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == DateTime.UtcNow).GetString(Helper.DbSetting);
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDateTimeVariable()
        {
            // Setup
            var value = DateTime.UtcNow;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value).GetString(Helper.DbSetting);
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDateTimeClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyDateTime = DateTime.UtcNow
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value.PropertyDateTime).GetString(Helper.DbSetting);
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDateTimeMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == GetDateTimeValueForParseExpression()).GetString(Helper.DbSetting);
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionDateTimeVariableMethodCall()
        {
            // Setup
            var value = GetDateTimeValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value).GetString(Helper.DbSetting);
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
