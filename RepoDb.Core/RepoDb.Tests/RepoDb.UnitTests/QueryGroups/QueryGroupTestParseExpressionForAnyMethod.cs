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
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAnyFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAnyFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAnyEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAnyEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAnyEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
