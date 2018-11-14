using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // NotBetween

        [TestMethod]
        public void TestParseDynamicParseDynamicNotBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new[] { 1, 2 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] NOT BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotBetweenOperationValuesLengthIsNotEqualsTo2()
        {
            // Setup
            var expression1 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1 } } };
            var expression2 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression1);
            QueryGroup.Parse(expression2);
        }
    }
}
