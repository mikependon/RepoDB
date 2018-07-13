using NUnit.Framework;
using RepoDb.Enumerations;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class QueryGroupTest
    {
        #region

        [Test]
        public void Test01_SingleExpression()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.IsNotNull(queryGroup);
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
            Assert.AreEqual(0, queryGroup.QueryGroups.Count());
        }

        [Test]
        public void Test02_MultipleExpressions()
        {
            // Setup
            var expression = new { Field1 = 1, Field2 = 2, Field3 = 3 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            Assert.IsNotNull(queryGroup);
            Assert.AreEqual(3, queryGroup.QueryFields.Count());
            Assert.AreEqual(0, queryGroup.QueryGroups.Count());
        }

        [Test]
        public void TestNoOperation()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Equal, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Equal, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Equal, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestNotEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.NotEqual, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestLessThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThan, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.LessThan, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestGreaterThanOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.GreaterThan, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestLessThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.LessThanOrEqual, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestGreaterThanOrEqualOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.GreaterThanOrEqual, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Like, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Like, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestNotLikeOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotLike, Value = 1 } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.NotLike, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Between, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestNotBetweenOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.NotBetween, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.In, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

        [Test]
        public void TestNotInOperation()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = new[] { 1, 2 } } };

            // Act
            var queryGroup = QueryGroup.Parse(expression);

            // Assert
            var queryFields = queryGroup.QueryFields.ToList();
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.NotIn, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
        }

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
            var queryFields = queryGroup.QueryFields.ToList();
            var queryGroups = queryGroup.QueryGroups.ToList();
            Assert.AreEqual(0, queryFields.Count());
            Assert.AreEqual(1, queryGroups.Count());
            queryFields = queryGroups[0].QueryFields.ToList(); // Specialized for All
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Equal, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name);
            Assert.AreEqual("Field1", queryFields[1].Field.Name); // @Field1 
            Assert.AreEqual(Operation.NotEqual, queryFields[1].Operation);
            Assert.AreEqual("Field1_1", queryFields[1].Parameter.Name); // @Field1_1
        }

        [Test]
        public void TestAnyOperation()
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
            var queryFields = queryGroup.QueryFields.ToList();
            var queryGroups = queryGroup.QueryGroups.ToList();
            Assert.AreEqual(0, queryFields.Count());
            Assert.AreEqual(1, queryGroups.Count());
            queryFields = queryGroups[0].QueryFields.ToList(); // Specialized for Any
            Assert.AreEqual("Field1", queryFields[0].Field.Name);
            Assert.AreEqual(Operation.Equal, queryFields[0].Operation);
            Assert.AreEqual("Field1", queryFields[0].Parameter.Name); // @Field1
            Assert.AreEqual("Field1", queryFields[1].Field.Name);
            Assert.AreEqual(Operation.NotEqual, queryFields[1].Operation);
            Assert.AreEqual("Field1_1", queryFields[1].Parameter.Name); // @Field_1
        }

        #endregion

        #region Values

        [Test]
        public void ThrowExceptionIfBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Between, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        [Test]
        public void ThrowExceptionIfNotBetweenOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotBetween, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        [Test]
        public void ThrowExceptionIfInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.In, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        [Test]
        public void ThrowExceptionIfNotInOperationValueIsNotAnArray()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.NotIn, Value = "NotAnArray" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        [Test]
        public void ThrowExceptionIfAllOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.All, Value = "NotAnArrayOfExpressions" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        [Test]
        public void ThrowExceptionIfAnyOperationValueIsNotAnArrayOfExpressions()
        {
            // Setup
            var expression = new { Field1 = new { Operation = Operation.Any, Value = "NotAnArrayOfExpressions" } };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                QueryGroup.Parse(expression);
            });
        }

        #endregion
    }
}
