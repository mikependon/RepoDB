using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Between

        [TestMethod]
        public void TestParseDynamicBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new[] { 1, 2 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesLengthIsLessThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfBetweenOperationValuesLengthIsGreaterThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }
    }
}
