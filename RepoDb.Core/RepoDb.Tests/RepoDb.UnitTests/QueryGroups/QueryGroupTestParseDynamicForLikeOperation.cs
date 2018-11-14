using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Like

        [TestMethod]
        public void TestParseDynamicLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Like, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
