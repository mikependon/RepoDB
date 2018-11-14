using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Equal

        [TestMethod]
        public void TestParseDynamicEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equal for IS NULL

        [TestMethod]
        public void TestParseDynamicEqualOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
