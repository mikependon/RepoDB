using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Any

        [TestMethod]
        public void TestParseExpressionAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAnyFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Any(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionNotAnyFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsFalseFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsTrueFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionAnyFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionNotAnyFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsFalseFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsTrueFromClassMethod()
        {
            throw new NotImplementedException();
        }
        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAnyFromVariableAsNot()
        {
            // Setup
            var list = new int[] { 1, 2 };

            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }.Any(p => p == e.PropertyInt)));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAnyFromVariableAsBoolean()
        {
            // Setup
            var list = new int[] { 1, 2 };

            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p == e.PropertyInt) == false);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAnyAsNot()
        {
            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }.Any(p => p == e.PropertyInt)));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionForAnyAsBoolean()
        {
            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p == e.PropertyInt) == false);
        }
    }
}
