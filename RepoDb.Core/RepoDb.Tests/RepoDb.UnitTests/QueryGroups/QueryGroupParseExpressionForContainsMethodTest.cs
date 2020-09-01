using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        #region String

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsNotEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") != true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsNotEqualsTrueAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") != true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsNotEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") != false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsNotEqualsFalseAtProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") != false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(@class.PropertyString) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueFromClassProperty()
        {
            // Setup
            var @class = new QueryGroupTestExpressionClass
            {
                PropertyString = "A"
            };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(@class.PropertyString) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsFalseFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsEqualsTrueFromClassMethod()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains(GetStringValueForParseExpression()) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOrEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT (([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForOrEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") || e.PropertyString.Contains("B")) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAndEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT (([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsWithTwoConditionsForAndEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyString.Contains("A") && e.PropertyString.Contains("B")) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] NOT LIKE @PropertyString) OR ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] NOT LIKE @PropertyString) AND ([PropertyString] LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtLeftAndNotContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) OR ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtLeftAndNotContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndNotContainsAtRightForOr()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") || !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] NOT LIKE @PropertyString) OR ([PropertyString] NOT LIKE @PropertyString_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotStringContainsAtLeftAndNotContainsAtRightForAnd()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !e.PropertyString.Contains("A") && !e.PropertyString.Contains("B"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
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
            var actual = parsed.GetString(m_dbSetting);
            var expected = "(([PropertyString] LIKE @PropertyString) AND ([PropertyString] <> @PropertyString_1 OR [PropertyString] <> @PropertyString_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.MappedPropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.QuotedPropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] LIKE @_PropertyString_)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionStringContainsAtUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.UnorganizedPropertyString.Contains("A"));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] LIKE @Property_____String)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Array

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsFromVariable()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new int[] { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsFromVariableEqualsTrue()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsFromVariableEqualsTrue()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsFromVariableEqualsFalse()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsFromVariableEqualsFalse()
        {
            // Setup
            var list = new int[] { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtMappedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtMappedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] NOT IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtQuotedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtQuotedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new string[] { "A", "B" }).Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new string[] { "A", "B" }).Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] NOT IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionArrayContainsAtUnorganizedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotArrayContainsAtUnorganizedPropertyFromVariables()
        {
            // Setup
            var list = new string[] { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] NOT IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region List

        [TestMethod]
        public void TestQueryGroupParseExpressionListContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<int>() { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotListContains()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new List<int>() { 1, 2 }).Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotListContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new List<int>() { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotListContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new List<int>() { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariable()
        {
            // Setup
            var list = new List<int>() { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionNotListContainsFromVariable()
        {
            // Setup
            var list = new List<int>() { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !list.Contains(e.PropertyInt));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsEqualsTrue()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<int>() { 1, 2 }).Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<int>() { 1, 2 }).Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariableEqualsTrue()
        {
            // Setup
            var list = new List<int>() { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariableEqualsFalse()
        {
            // Setup
            var list = new List<int>() { 1, 2 };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsAtMappedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<string>() { "A", "B" }).Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariablesAtMappedProperty()
        {
            // Setup
            var list = new List<string>() { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.MappedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@PropertyString_In_0, @PropertyString_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsAtQuotedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<string>() { "A", "B" }).Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariablesAtQuotedProperty()
        {
            // Setup
            var list = new List<string>() { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.QuotedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([PropertyString] IN (@_PropertyString__In_0, @_PropertyString__In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsAtUnorganizedProperty()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (new List<string>() { "A", "B" }).Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionListContainsFromVariablesAtUnorganizedProperty()
        {
            // Setup
            var list = new List<string>() { "A", "B" };
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => list.Contains(e.UnorganizedPropertyString));

            // Act
            var actual = parsed.GetString(m_dbSetting);
            var expected = "([Property / . String] IN (@Property_____String_In_0, @Property_____String_In_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
