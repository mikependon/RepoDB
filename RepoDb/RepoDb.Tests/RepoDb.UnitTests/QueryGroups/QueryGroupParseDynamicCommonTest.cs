using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests
{
    [TestClass]
    public partial class QueryGroupTest
    {
        // No Operation

        [TestMethod]
        public void TestQueryGroupParseDynamicNoOperation()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // No Operation for IS NULL

        [TestMethod]
        public void TestQueryGroupParseDynamicNoOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = (object)null };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
