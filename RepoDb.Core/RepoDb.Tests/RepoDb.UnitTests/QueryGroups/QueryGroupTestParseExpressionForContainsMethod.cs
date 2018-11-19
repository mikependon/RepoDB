using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Contains

        [TestMethod]
        public void TestParseExpressionContains()
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
        public void TestParseExpressionNotContains()
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
        public void TestParseExpressionNotContainsEqualsTrue()
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
        public void TestParseExpressionNotContainsEqualsFalse()
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
        public void TestParseExpressionContainsFromVariable()
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
        public void TestParseExpressionNotContainsFromVariable()
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
        public void TestParseExpressionContainsEqualsTrue()
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
        public void TestParseExpressionContainsEqualsFalse()
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
        public void TestParseExpressionContainsFromVariableEqualsTrue()
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
        public void TestParseExpressionContainsFromVariableEqualsFalse()
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
        public void TestParseExpressionContainsAtProperty()
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
        public void TestParseExpressionNotContainsAtProperty()
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
        public void TestParseExpressionContainsEqualsTrueAtProperty()
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
        public void TestParseExpressionContainsEqualsFalseAtProperty()
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
        public void TestParseExpressionContainsFromClassProperty()
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
        public void TestParseExpressionNotContainsFromClassProperty()
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
        public void TestParseExpressionContainsEqualsFalseFromClassProperty()
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
        public void TestParseExpressionContainsEqualsTrueFromClassProperty()
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
        public void TestParseExpressionContainsFromClassMethod()
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
        public void TestParseExpressionNotContainsFromClassMethod()
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
        public void TestParseExpressionContainsEqualsFalseFromClassMethod()
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
        public void TestParseExpressionContainsEqualsTrueFromClassMethod()
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
        public void TestParseExpressionNotContainsEqualsTrueFromClassMethod()
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
        public void TestParseExpressionNotContainsEqualsFalseFromClassMethod()
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
        public void TestParseExpressionContainsWithTwoConditionsForOr()
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
        public void TestParseExpressionContainsWithTwoConditionsForAnd()
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
        public void TestParseExpressionContainsWithTwoConditionsForOrEqualsFalse()
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
        public void TestParseExpressionContainsWithTwoConditionsForOrEqualsTrue()
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
        public void TestParseExpressionContainsWithTwoConditionsForAndEqualsFalse()
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
        public void TestParseExpressionContainsWithTwoConditionsForAndEqualsTrue()
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
        public void TestParseExpressionNotContainsAtLeftAndContainsAtRightForOr()
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
        public void TestParseExpressionNotContainsAtLeftAndContainsAtRightForAnd()
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
        public void TestParseExpressionContainsAtLeftAndNotContainsAtRightForOr()
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
        public void TestParseExpressionContainsAtLeftAndNotContainsAtRightForAnd()
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
        public void TestParseExpressionNotContainsAtLeftAndNotContainsAtRightForOr()
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
        public void TestParseExpressionNotContainsAtLeftAndNotContainsAtRightForAnd()
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
        public void TestParseExpressionStringPropertyContainsAndArrayAnyMethod()
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
