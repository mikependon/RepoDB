using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // DateTime

        [TestMethod]
        public void TestParseExpressionDateTimeConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == DateTime.UtcNow).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeVariable()
        {
            // Setup
            var value = DateTime.UtcNow;

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyDateTime = DateTime.UtcNow
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value.PropertyDateTime).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == GetDateTimeValueForParseExpression()).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionDateTimeVariableMethodCall()
        {
            // Setup
            var value = GetDateTimeValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyDateTime == value).GetString();
            var expected = "([PropertyDateTime] = @PropertyDateTime)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
