using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // NotLike

        [TestMethod]
        public void TestParseDynamicNotLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotLike, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] NOT LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
