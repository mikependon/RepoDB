using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void TestParseExpressionWithNameAtRight()
        {
            // Setup
            var parsed = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => 1 == e.PropertyInt);

            // Act
            var actual = parsed.QueryFields.First().Field.Name;
            var expected = "PropertyInt";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Properties

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyInt == 2).GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleSameFieldsForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyInt == 2).GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyInt] = @PropertyInt_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Groupings

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 && e.PropertyString == "A").GetString();
            var expected = "([PropertyInt] = @PropertyInt AND [PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1 || e.PropertyString == "A").GetString();
            var expected = "([PropertyInt] = @PropertyInt OR [PropertyString] = @PropertyString)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForOrAndSingleFieldForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) && e.PropertyString == "A").GetString();
            var expected = "([PropertyString] = @PropertyString AND ([PropertyInt] = @PropertyInt OR [PropertyDouble] = @PropertyDouble))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithSingleGroupForAndAndSingleFieldForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) || e.PropertyString == "A").GetString();
            var expected = "([PropertyString] = @PropertyString OR ([PropertyInt] = @PropertyInt AND [PropertyDouble] = @PropertyDouble))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForAnd()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 && e.PropertyDouble == 2) && (e.PropertyString == "A" && e.PropertySingle == 1)).GetString();
            var expected = "(([PropertyInt] = @PropertyInt AND [PropertyDouble] = @PropertyDouble) AND ([PropertyString] = @PropertyString AND [PropertySingle] = @PropertySingle))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseExpressionWithDoubleGroupForOr()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1 || e.PropertyDouble == 2) || (e.PropertyString == "A" || e.PropertySingle == 1)).GetString();
            var expected = "(([PropertyInt] = @PropertyInt OR [PropertyDouble] = @PropertyDouble) OR ([PropertyString] = @PropertyString OR [PropertySingle] = @PropertySingle))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

    }
}
