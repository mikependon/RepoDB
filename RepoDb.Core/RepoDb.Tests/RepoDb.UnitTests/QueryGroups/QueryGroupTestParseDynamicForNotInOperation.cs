using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // NotIn

        [TestMethod]
        public void TestParseDynamicNotInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new[] { 1, 2, 3 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfNotInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }
    }
}
