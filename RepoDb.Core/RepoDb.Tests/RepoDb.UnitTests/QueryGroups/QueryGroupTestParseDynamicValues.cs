using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Equal

        [TestMethod]
        public void TestParseDynamicValueForEqualOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = 1 });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotEqual
        [TestMethod]
        public void TestParseDynamicValueForNotEqualOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.Equal, Value = 1 } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThan
        [TestMethod]
        public void TestParseDynamicValueForLessThanOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.LessThan, Value = 1 } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThanOrEqual
        [TestMethod]
        public void TestParseDynamicValueForLessThanOrEqualOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.LessThanOrEqual, Value = 1 } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // GreaterThan
        [TestMethod]
        public void TestParseDynamicValueForGreaterThanOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.GreaterThan, Value = 1 } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // LessThanOrEqual
        [TestMethod]
        public void TestParseDynamicValueForGreaterThanOrEqualOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.GreaterThanOrEqual, Value = 1 } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = 1;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Like
        [TestMethod]
        public void TestParseDynamicValueForLikeOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.Like, Value = "A%" } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // NotLike
        [TestMethod]
        public void TestParseDynamicValueForNotLikeOperation()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.NotLike, Value = "A%" } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;
            var expected = "A%";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // Between
        [TestMethod]
        public void TestParseDynamicValueForBetweenOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.Between, Value = new[] { value1, value2 } } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual(value1, ((Array)actual).GetValue(0));
            Assert.AreEqual(value2, ((Array)actual).GetValue(1));
        }

        // Between
        [TestMethod]
        public void TestParseDynamicValueForNotBetweenOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.NotBetween, Value = new[] { value1, value2 } } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual(value1, ((Array)actual).GetValue(0));
            Assert.AreEqual(value2, ((Array)actual).GetValue(1));
        }

        // In
        [TestMethod]
        public void TestParseDynamicValueForInOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.In, Value = new[] { value1, value2 } } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual(value1, ((Array)actual).GetValue(0));
            Assert.AreEqual(value2, ((Array)actual).GetValue(1));
        }

        // NotIn
        [TestMethod]
        public void TestParseDynamicValueNotInOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new { Field1 = new { Operation = Operation.NotIn, Value = new[] { value1, value2 } } });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.IsTrue(actual is Array);
            Assert.AreEqual(value1, ((Array)actual).GetValue(0));
            Assert.AreEqual(value2, ((Array)actual).GetValue(1));
        }

        // Any
        [TestMethod]
        public void TestParseDynamicValueForAnyOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new
            {
                Field1 = new
                {
                    Operation = Operation.Any,
                    Value = new[]
                    {
                        new { Operation = Operation.Equal, Value = value1 },
                        new { Operation = Operation.Equal, Value = value2 }
                    }
                }
            });

            // Act
            var queryGroup = parsed.QueryGroups.First();
            var actual1 = queryGroup.QueryFields.First().Parameter.Value;
            var actual2 = queryGroup.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual(value1, actual1);
            Assert.AreEqual(value2, actual2);
        }

        // All
        [TestMethod]
        public void TestParseDynamicValueForAllOperation()
        {
            // Setup
            var value1 = 1;
            var value2 = 100;
            var parsed = QueryGroup.Parse(new
            {
                Field1 = new
                {
                    Operation = Operation.All,
                    Value = new[]
                    {
                        new { Operation = Operation.LessThan, Value = value1 },
                        new { Operation = Operation.GreaterThan, Value = value2 }
                    }
                }
            });

            // Act
            var queryGroup = parsed.QueryGroups.First();
            var actual1 = queryGroup.QueryFields.First().Parameter.Value;
            var actual2 = queryGroup.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual(value1, actual1);
            Assert.AreEqual(value2, actual2);
        }
    }
}
