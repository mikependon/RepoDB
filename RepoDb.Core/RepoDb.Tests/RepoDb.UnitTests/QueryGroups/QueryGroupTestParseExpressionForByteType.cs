using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Byte

        [TestMethod]
        public void TestQueryGroupParseExpressionByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == new[] { byte.Parse("0") }).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionPassedByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == Encoding.Unicode.GetBytes("Test")).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionByteVariable()
        {
            // Setup
            var value = new[] { byte.Parse("0") };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == value).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionByteClassProperty()
        {
            // Setup
            var value = new QueryGroupTestExpressionClass
            {
                PropertyBytes = new[] { byte.Parse("0") }
            };

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == value.PropertyBytes).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionByteMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == GetBytesValueForParseExpression()).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionByteVariableMethodCall()
        {
            // Setup
            var value = GetBytesValueForParseExpression();

            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == value).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
