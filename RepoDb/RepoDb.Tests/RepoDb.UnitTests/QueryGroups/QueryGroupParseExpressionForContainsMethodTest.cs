using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Contains

        [TestMethod]
        public void TestQueryGroupParseExpressionContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsFromVariableEqualsTrue()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsFromVariableEqualsFalse()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForOrEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT (([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForOrEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForAndEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "NOT (([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsWithTwoConditionsForAndEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsAtLeftAndContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] NOT LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsAtLeftAndContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] NOT LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsAtLeftAndNotContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionContainsAtLeftAndNotContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsAtLeftAndNotContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] NOT LIKE @PropertyString) OR ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotContainsAtLeftAndNotContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] NOT LIKE @PropertyString) AND ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringPropertyContainsAndArrayAnyMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && (new[] { "B", "C" }).Any(p => p != e.PropertyString));

            // Act
            var actual = parsed.GetString();
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] <> @PropertyString_1 OR [PropertyString] <> @PropertyString_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
