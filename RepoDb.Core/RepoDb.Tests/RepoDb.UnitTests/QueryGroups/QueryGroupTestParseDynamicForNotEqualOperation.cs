using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // NotEqual

        [TestMethod]
        public void TestParseDynamicNotEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <> @Field1)"; // !=

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual of IS NOT NULL

        [TestMethod]
        public void TestParseDynamicNotEqualOperationForIsNotNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NOT NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
