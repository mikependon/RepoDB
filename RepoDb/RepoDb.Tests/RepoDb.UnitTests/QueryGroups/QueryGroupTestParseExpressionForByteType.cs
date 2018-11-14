using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Byte

        [TestMethod]
        public void TestParseExpressionByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == new[] { byte.Parse("0") }).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionPassedByteArray()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == Encoding.Unicode.GetBytes("Test")).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteVariable()
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
        public void TestParseExpressionByteClassProperty()
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
        public void TestParseExpressionByteMethodCall()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyBytes == GetBytesValueForParseExpression()).GetString();
            var expected = "([PropertyBytes] = @PropertyBytes)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionByteVariableMethodCall()
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
