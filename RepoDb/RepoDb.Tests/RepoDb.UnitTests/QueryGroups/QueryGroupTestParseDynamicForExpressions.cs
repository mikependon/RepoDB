using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    [TestClass]
    public partial class QueryGroupTest
    {
        // Expression

        [TestMethod]
        public void TestParseDynamicSingleExpression()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicOnParseDynamicIfConjunctionIsNotAConjunctionType()
        {
            // Setup
            var expression = new { Conjunction = "NotAConjunctionType", Field1 = 1 };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod]
        public void TestParseDynamicMultipleExpressions()
        {
            // Setup
            var expression = new { Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2 AND [Field3] = @Field3)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParseDynamicMultipleExpressionsForConjunctionOr()
        {
            // Setup
            var expression = new { Conjunction = Conjunction.Or, Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1 OR [Field2] = @Field2 OR [Field3] = @Field3)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
