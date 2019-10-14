using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // All

        [TestMethod]
        public void TestQueryGroupParseExpressionAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAll()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { 1, 2 }).All(p => p != e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt AND [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAllFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.All(p => p != e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] <> @PropertyInt AND [PropertyInt] <> @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAllFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == false, Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyInt = 500
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { @class.PropertyInt, @class.PropertyInt }).All(p => p == e.PropertyInt) == true, Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAllFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false, Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionAllEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAllEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt) == false, Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotAllEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new[] { GetIntValueForParseExpression(), GetIntValueForParseExpression() }).All(p => p == e.PropertyInt), Helper.DbSetting);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
