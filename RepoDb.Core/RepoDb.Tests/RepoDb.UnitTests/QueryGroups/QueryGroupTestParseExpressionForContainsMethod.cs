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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt));

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => !(new int[] { 1, 2 }.Contains(e.PropertyInt)));

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt) == true);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] IN (@PropertyInt_In_0, @PropertyInt_In_1))";

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
        public void TestParseExpressionContainsEqualsFalse()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => new int[] { 1, 2 }.Contains(e.PropertyInt) == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

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
            var expected = "([PropertyInt] NOT IN (@PropertyInt_In_0, @PropertyInt_In_1))";

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
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyString.Contains("A") == false);

            // Act
            var actual = parsed.GetString();
            var expected = "([PropertyString] NOT LIKE @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionContainsFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionNotContainsFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsFalseFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsTrueFromClassProperty()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionContainsFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionNotContainsFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsFalseFromClassMethod()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestParseExpressionContainsEqualsTrueFromClassMethod()
        {
            throw new NotImplementedException();
        }
    }
}
