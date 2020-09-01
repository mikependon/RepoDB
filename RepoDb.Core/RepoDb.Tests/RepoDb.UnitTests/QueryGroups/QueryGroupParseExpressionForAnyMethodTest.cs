using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Any

        [TestMethod]
        public void TestQueryGroupParseExpressionAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAny()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Any(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAnyFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Any(p => p != e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAnyFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).Any(p => p == e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAnyFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAnyEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAnyEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAnyEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).Any(p => p == e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt OR [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
