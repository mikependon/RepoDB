using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Any

        [TestMethod]
        public void TestParseDynamicAnyOperation()
        {
            // Setup
            var expression = new
            {
                Field1 = new
                {
                    Operation = Operation.Any,
                    Value = new[]
                    {
                        new { Operation = Operation.Equal, Value = 1 },
                        new { Operation = Operation.NotEqual, Value = 1 }
                    }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            // Must be inside of another group as the ANY fields must be grouped by itself
            var expected = $"(([Field1] = @Field1 OR [Field1] <> @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfAnyOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Any, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }
    }
}
