using NUnit.Framework;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class QueryGroupTest
    {
        // Expression

        [Test]
        public void Test01_SingleExpression()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] = @Field1)", queryGroup.GetString());
        }

        [Test]
        public void Test02_ThrowExceptionIfConjuectionIsNotAConjunctionType()
        {
            // Setup
            var expression = new { Conjunction = "NotAConjunctionType", Field1 = 1 };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void Test03_MultipleExpressions()
        {
            // Setup
            var expression = new { Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] = @Field1 AND [Field2] = @Field2 AND [Field3] = @Field3)", queryGroup.GetString());
        }

        [Test]
        public void Test04_MultipleExpressionsForConjunctionOr()
        {
            // Setup
            var expression = new { Conjunction = Conjunction.Or, Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] = @Field1 OR [Field2] = @Field2 OR [Field3] = @Field3)", queryGroup.GetString());
        }

        // No Operation

        [Test]
        public void TestNoOperation()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] = @Field1)", queryGroup.GetString());
        }

        // Equal

        [Test]
        public void TestEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] = @Field1)", queryGroup.GetString());
        }

        // NotEqual

        [Test]
        public void TestNotEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] <> @Field1)", queryGroup.GetString()); // !=
        }

        // LessThan

        [Test]
        public void TestLessThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThan, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] < @Field1)", queryGroup.GetString());
        }

        // GreaterThan

        [Test]
        public void TestGreaterThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] > @Field1)", queryGroup.GetString());
        }

        // LessThanOrEqual

        [Test]
        public void TestLessThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] <= @Field1)", queryGroup.GetString());
        }

        // GreaterThanOrEqual

        [Test]
        public void TestGreaterThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] >= @Field1)", queryGroup.GetString());
        }

        // Like

        [Test]
        public void TestLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Like, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] LIKE @Field1)", queryGroup.GetString());
        }

        // NotLike

        [Test]
        public void TestNotLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotLike, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual("([Field1] NOT LIKE @Field1)", queryGroup.GetString());
        }

        // Between

        [Test]
        public void TestBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual($"([Field1] BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, "2" } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfBetweenOperationValuesLengthIsNotEqualsTo2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        // NotBetween

        [Test]
        public void TestNotBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual($"([Field1] NOT BETWEEN @Field1_BetweenLeft AND @Field1_BetweenRight)", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfNotBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfNotBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, "2" } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfNotBetweenOperationValuesLengthIsNotEqualsTo2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        // In

        [Test]
        public void TestInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new[] { 1, 2, 3 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual($"([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new object[] { 1, "2" } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        // NotIn

        [Test]
        public void TestNotInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new[] { 1, 2, 3 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.AreEqual($"([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfNotInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        [Test]
        public void ThrowExceptionIfNotInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new object[] { 1, "2" } } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        // All

        [Test]
        public void TestAllOperation()
        {
            // Setup
            var expression = new
            {
                Field1 = new
                {
                    Operation = Operation.All,
                    Value = new[]
                    {
                        new { Operation = Operation.Equal, Value = 1 },
                        new { Operation = Operation.NotEqual, Value = 1 }
                    }
                }
            };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            // Must be inside of another group as the ALL fields must be grouped by itself
            Assert.AreEqual($"(([Field1] = @Field1 AND [Field1] <> @Field1_1))", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfAllOperationValueIsNotAnExpressionOrAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }

        // Any

        [Test]
        public void TestAnyOperation()
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
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            // Must be inside of another group as the OR fields must be grouped by itself
            Assert.AreEqual($"(([Field1] = @Field1 OR [Field1] <> @Field1_1))", queryGroup.GetString());
        }

        [Test]
        public void ThrowExceptionIfAnyOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Any, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            Assert.Throws<InvalidQueryExpressionException>(() => QueryGroup.Parse(expression));
        }
    }
}
