using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // LessThan

        [TestMethod]
        public void TestParseDynamicLessThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] < @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThanOrEqual

        [TestMethod]
        public void TestParseDynamicLessThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
