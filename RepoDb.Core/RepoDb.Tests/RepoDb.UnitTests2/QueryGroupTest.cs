using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Diagnostics;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class QueryGroupTest
    {
        // Expression

        [TestMethod]
        public void TestSingleExpression()
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
        public void ThrowExceptionIfConjunctionIsNotAConjunctionType()
        {
            // Setup
            var expression = new { Conjunction = "NotAConjunctionType", Field1 = 1 };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod]
        public void TestMultipleExpressions()
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
        public void TestMultipleExpressionsForConjunctionOr()
        {
            // Setup
            var expression = new { Conjunction = Conjunction.Or, Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1 OR [Field2] = @Field2 OR [Field3] = @Field3)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // No Operation

        [TestMethod]
        public void TestNoOperation()
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
        public void TestNoOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = (object)null };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equal

        [TestMethod]
        public void TestEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Equal for IS NULL

        [TestMethod]
        public void TestEqualOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual

        [TestMethod]
        public void TestNotEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <> @Field1)"; // !=

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual of IS NOT NULL

        [TestMethod]
        public void TestNotEqualOperationForIsNotNull()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = (object)null } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NOT NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThan

        [TestMethod]
        public void TestLessThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] < @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThan

        [TestMethod]
        public void TestGreaterThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] > @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThanOrEqual

        [TestMethod]
        public void TestLessThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] <= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThanOrEqual

        [TestMethod]
        public void TestGreaterThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] >= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Like

        [TestMethod]
        public void TestLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Like, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotLike

        [TestMethod]
        public void TestNotLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotLike, Value = 1 } };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] NOT LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Between

        [TestMethod]
        public void TestBetweenOperation()
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
        public void ThrowExceptionIfBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfBetweenOperationValuesLengthIsLessThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfBetweenOperationValuesLengthIsGreaterThan2()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // NotBetween

        [TestMethod]
        public void TestNotBetweenOperation()
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
        public void ThrowExceptionIfNotBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfNotBetweenOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, "2" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfNotBetweenOperationValuesLengthIsNotEqualsTo2()
        {
            // Setup
            var expression1 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1 } } };
            var expression2 = new { Field1 = new { Operation = Operation.NotBetween, Value = new object[] { 1, 2, 3 } } };

            // Act/Assert
            QueryGroup.Parse(expression1);
            QueryGroup.Parse(expression2);
        }

        // In

        [TestMethod]
        public void TestInOperation()
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
        public void ThrowExceptionIfInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // NotIn

        [TestMethod]
        public void TestNotInOperation()
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
        public void ThrowExceptionIfNotInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = "NotAnArray" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfNotInOperationValuesAreNotOnTheSameTypes()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new object[] { 1, "OtherDataType" } } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // All

        [TestMethod]
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
            var actual = QueryGroup.Parse(expression).GetString();
            // Must be inside of another group as the ALL fields must be grouped by itself
            var expected = $"(([Field1] = @Field1 AND [Field1] <> @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfAllOperationValueIsNotAnExpressionOrAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfChildQueryGroupsAreNotAnExpressionsOrAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // Any

        [TestMethod]
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
            var actual = QueryGroup.Parse(expression).GetString();
            // Must be inside of another group as the ANY fields must be grouped by itself
            var expected = $"(([Field1] = @Field1 OR [Field1] <> @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfAnyOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Any, Value = "NotAnExpressionsOrAnArrayOfExpressions" } };

            // Act/Assert
            QueryGroup.Parse(expression);
        }

        // FixParameters

        [TestMethod]
        public void TestFixParametersOnASingleFieldWithMultipleExpression()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new dynamic[]
                {
                    new { Field1 = 2 },
                    new { Field1 = 3 },
                    new { Field1 = 4 }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field1] = @Field1_1) AND ([Field1] = @Field1_2) AND ([Field1] = @Field1_3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Others

        [TestMethod]
        public void TestChildQueryGroupsSingle()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new
                {
                    Field2 = 2,
                    Field3 = 3
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestChildQueryGroupsSingleForConjunctionOr()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new
                {
                    Conjunction = Conjunction.Or,
                    Field2 = 2,
                    Field3 = 3
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 OR [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestChildQueryGroupsMultiple()
        {
            // Setup
            var expression = new
            {
                Field1 = 1,
                QueryGroups = new dynamic[]
                {
                    new
                    {
                        Field2 = 2,
                        Field3 = 3
                    },
                    new
                    {
                        Field4 = 2,
                        Field5 = 3
                    }
                }
            };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = $"([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field3] = @Field3) AND ([Field4] = @Field4 AND [Field5] = @Field5))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

    }
}
