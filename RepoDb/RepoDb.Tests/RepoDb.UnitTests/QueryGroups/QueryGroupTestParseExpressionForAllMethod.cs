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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p == e.PropertyInt));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p != e.PropertyInt));

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

        [TestMethod]
        public void TestParseExpressionAllFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAllFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAllFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionAllEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAllEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionNotAllEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
