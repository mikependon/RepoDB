using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // In

        [TestMethod]
        public void TestParseDynamicParseDynamicInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new[] { 1, 2, 3 } } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnParseDynamicIfInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }
    }
}
