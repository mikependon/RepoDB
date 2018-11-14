using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // GreaterThan

        [TestMethod]
        public void TestParseDynamicGreaterThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] > @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThanOrEqual

        [TestMethod]
        public void TestParseDynamicGreaterThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] >= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
