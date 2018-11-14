using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Guid

        [TestMethod]
        public void TestParseExpressionGuidConstant()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyGuid == Guid.NewGuid()).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidVariable()
        {
            // Setup
            var value = Guid.NewGuid();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyGuid == value).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyGuid = Guid.NewGuid()
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyGuid == value.PropertyGuid).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyGuid == GetGuidValueForParseExpression()).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionGuidVariableMethodCall()
        {
            // Setup
            var value = GetGuidValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyGuid == value).GetString();
            var expected = "([PropertyGuid] = @PropertyGuid)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
