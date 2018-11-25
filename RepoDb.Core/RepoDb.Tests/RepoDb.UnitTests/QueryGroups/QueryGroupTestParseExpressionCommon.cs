using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Name

        [TestMethod]
        public void TestParseExpressionWithNameAtLeft()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1);

            // Act
            var actual = parsed.QueryFields.First().Field.Name;
            var expected = "PropertyInt";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWhereNameHasMapping()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.OtherPropertyString == "ABC");

            // Act
            var actual = parsed.QueryFields.First().Field.Name;
            var expected = "PropertyString";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionWithNameAtRight()
        {
            // Act
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => 1 == e.PropertyInt);
        }

        // Properties

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyInt == 2).GetString();
            var expected = "(([PropertyInt] = @PropertyInt) AND ([PropertyInt] = @PropertyInt_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyInt == 2).GetString();
            var expected = "(([PropertyInt] = @PropertyInt) OR ([PropertyInt] = @PropertyInt_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Groupings

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyString == "A").GetString();
            var expected = "(([PropertyInt] = @PropertyInt) AND ([PropertyString] = @PropertyString))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyString == "A").GetString();
            var expected = "(([PropertyInt] = @PropertyInt) OR ([PropertyString] = @PropertyString))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOrAndSingleFieldForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) && e.PropertyString == "A").GetString();
            var expected = "((([PropertyInt] = @PropertyInt) OR ([PropertyDouble] = @PropertyDouble)) AND ([PropertyString] = @PropertyString))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAndAndSingleFieldForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) || e.PropertyString == "A").GetString();
            var expected = "((([PropertyInt] = @PropertyInt) AND ([PropertyDouble] = @PropertyDouble)) OR ([PropertyString] = @PropertyString))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) && (e.PropertyString == "A" && e.PropertySingle == 1)).GetString();
            var expected = "((([PropertyInt] = @PropertyInt) AND ([PropertyDouble] = @PropertyDouble)) AND (([PropertyString] = @PropertyString) AND ([PropertySingle] = @PropertySingle)))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) || (e.PropertyString == "A" || e.PropertySingle == 1)).GetString();
            var expected = "((([PropertyInt] = @PropertyInt) OR ([PropertyDouble] = @PropertyDouble)) OR (([PropertyString] = @PropertyString) OR ([PropertySingle] = @PropertySingle)))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

    }
}
