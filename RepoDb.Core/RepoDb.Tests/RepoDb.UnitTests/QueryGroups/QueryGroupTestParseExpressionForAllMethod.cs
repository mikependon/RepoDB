using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // All

        [TestMethod]
        public void TestParseExpressionAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.All(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt AND [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAllFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt AND [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAllFromVariableAsNot()
        {
            // Setup
            var list = new int[] { 1, 2 };

            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }.All(p => p == e.PropertyInt)));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAllFromVariableAsBoolean()
        {
            // Setup
            var list = new int[] { 1, 2 };

            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.All(p => p == e.PropertyInt) == false);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAllAsNot()
        {
            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }.All(p => p == e.PropertyInt)));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAllAsBoolean()
        {
            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.All(p => p == e.PropertyInt) == false);
        }
    }
}
