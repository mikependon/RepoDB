using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Operations

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForNotEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt != 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] <> @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForGreaterThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt > 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] > @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForGreaterThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt >= 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] >= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForLessThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt < 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] < @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForLessThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt <= 1, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] <= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equals Boolean

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqualEqualsFalse()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1) == false, Helper.DbSetting).GetString();
            var expected = "NOT ([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqualEqualsTrue()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1) == true, Helper.DbSetting).GetString();
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // ExpectedException

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnParseExpressionWithoutProperty()
        {
            QueryGroup.Parse<QueryGroupTestExpressionClass>(e => true, Helper.DbSetting).GetString();
        }
    }
}
