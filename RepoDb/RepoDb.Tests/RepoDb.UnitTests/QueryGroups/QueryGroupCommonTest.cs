using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public partial class QueryGroupTest
    {
        #region Constructor

        [TestMethod]
        public void TestQueryGroupConstructorForQueryField()
        {
            // Setup
            var queryField = new QueryField("Field1", Operation.Equal, 1);

            // Act
            var queryGroup = new QueryGroup(queryField);

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryGroup()
        {
            // Setup
            var queryField = new QueryField("Field1", Operation.Equal, 1);

            // Act
            var queryGroup = new QueryGroup(new QueryGroup(queryField));

            // Assert
            Assert.AreEqual(null, queryGroup.QueryFields);
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryFields()
        {
            // Setup
            var queryField = new QueryField("Field1", Operation.Equal, 1);

            // Act
            var queryGroup = new QueryGroup(queryField.AsEnumerable());

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryGroups()
        {
            // Setup
            var queryField = new QueryField("Field1", Operation.Equal, 1);

            // Act
            var queryGroup = new QueryGroup(new QueryGroup(queryField).AsEnumerable());

            // Assert
            Assert.AreEqual(null, queryGroup.QueryFields);
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryFieldAndQueryGroup()
        {
            // Setup
            var queryFieldA = new QueryField("Field1", Operation.Equal, 1);
            var queryFieldB = new QueryField("Field2", Operation.Equal, 2);

            // Act
            var queryGroup = new QueryGroup(queryFieldA, new QueryGroup(queryFieldB));

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryFieldsAndQueryGroup()
        {
            // Setup
            var queryFieldA = new QueryField("Field1", Operation.Equal, 1);
            var queryFieldB = new QueryField("Field2", Operation.Equal, 2);

            // Act
            var queryGroup = new QueryGroup(queryFieldA.AsEnumerable(), new QueryGroup(queryFieldB));

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryFieldAndQueryGroups()
        {
            // Setup
            var queryFieldA = new QueryField("Field1", Operation.Equal, 1);
            var queryFieldB = new QueryField("Field2", Operation.Equal, 2);

            // Act
            var queryGroup = new QueryGroup(queryFieldA, new QueryGroup(queryFieldB).AsEnumerable());

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        [TestMethod]
        public void TestQueryGroupConstructorForQueryFieldsAndQueryGroups()
        {
            // Setup
            var queryFieldA = new QueryField("Field1", Operation.Equal, 1);
            var queryFieldB = new QueryField("Field2", Operation.Equal, 2);

            // Act
            var queryGroup = new QueryGroup(queryFieldA.AsEnumerable(), new QueryGroup(queryFieldB).AsEnumerable());

            // Assert
            Assert.AreEqual(1, queryGroup.QueryFields.Count());
            Assert.AreEqual(1, queryGroup.QueryGroups.Count());
        }

        #endregion

        #region Operations

        [TestMethod]
        public void TestQueryGroupEqual()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.Equal, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupNotEqual()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotEqual, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] <> @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupLessThan()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.LessThan, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] < @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupGreaterThan()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.GreaterThan, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] > @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupLessThanOrEqual()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.LessThanOrEqual, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] <= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupGreaterThanOrEqual()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.GreaterThanOrEqual, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] >= @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupLike()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.Like, "A"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupNotLike()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotLike, "A"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupBetween()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.Between, "A"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] BETWEEN @Field1_Left AND @Field1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupNotBetween()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotBetween, "A"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT BETWEEN @Field1_Left AND @Field1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupIn()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.In, new[] { 1, 2, 3 }));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupNotIn()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotIn, new[] { 1, 2, 3 }));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Parent

        [TestMethod]
        public void TestQueryGroupWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.Equal, 2)
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field1", Operation.Equal, 2)
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field1] = @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Conjunction

        [TestMethod]
        public void TestQueryGroupWithNoConjunction()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithConjunctionAnd()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            },
            Conjunction.And);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            },
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 OR [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region IsNot

        [TestMethod]
        public void TestQueryGroupWithNoIsNot()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithIsNotAsFalse()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            },
            false);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithIsNotAsTrue()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, "Value1"),
                new QueryField("Field2", Operation.Equal, "Value2")
            },
            true);

            // Act
            var actual = queryGroup.GetString();
            var expected = "NOT ([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Children

        [TestMethod]
        public void TestQueryGroupWithChildQueryGroupWithTwoFields()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.Equal, 2)
            });
            var queryGroup = new QueryGroup(childQueryGroup);

            // Act
            var actual = queryGroup.GetString();
            var expected = "(([Field1] = @Field1 AND [Field2] = @Field2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithChildQueryGroupWithTwoIdenticalFields()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field1", Operation.Equal, 2)
            });
            var queryGroup = new QueryGroup(childQueryGroup);

            // Act
            var actual = queryGroup.GetString();
            var expected = "(([Field1] = @Field1 AND [Field1] = @Field1_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldAndWithChildQueryGroupWithTwoFields()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field2", Operation.Equal, 2),
                new QueryField("Field3", Operation.Equal, 3)
            });
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable());

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldAndWithChildQueryGroupWithTwoIdenticalFields()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field2", Operation.Equal, 2),
                new QueryField("Field2", Operation.Equal, 3)
            });
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable());

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND ([Field2] = @Field2 AND [Field2] = @Field2_1))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldAndWithChildQueryGroupWithTwoFieldsAllAreIdentitical()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 2),
                new QueryField("Field1", Operation.Equal, 3)
            });
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable());

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND ([Field1] = @Field1_1 AND [Field1] = @Field1_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldWithConjunctionOrAndWithChildQueryGroupWithTwoFields()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field2", Operation.Equal, 2),
                new QueryField("Field3", Operation.Equal, 3)
            });
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable(),
                Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 OR ([Field2] = @Field2 AND [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldAndWithChildQueryGroupWithTwoFieldsAndWithConjunctionOr()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field2", Operation.Equal, 2),
                new QueryField("Field3", Operation.Equal, 3)
            },
            Conjunction.Or);
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable());

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND ([Field2] = @Field2 OR [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSingleFieldWithConjunctionOrAndWithChildQueryGroupWithTwoFieldsAndWithConjunctionOr()
        {
            // Setup
            var childQueryGroup = new QueryGroup(new[]
            {
                new QueryField("Field2", Operation.Equal, 2),
                new QueryField("Field3", Operation.Equal, 3)
            },
            Conjunction.Or);
            var queryGroup = new QueryGroup(new QueryField("Field1", 1).AsEnumerable(),
                childQueryGroup.AsEnumerable(),
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 OR ([Field2] = @Field2 OR [Field3] = @Field3))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Combinations

        [TestMethod]
        public void TestQueryGroupWithEqualAndNotEqual()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.NotEqual, 1)
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] <> @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndNotEqualIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field1", Operation.NotEqual, 1)
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field1] <> @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndBetween()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.Between, new [] { 1, 2 })
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] BETWEEN @Field2_Left AND @Field2_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndBetweenIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field1", Operation.Between, new [] { 1, 2 })
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field1] BETWEEN @Field1_1_Left AND @Field1_1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndLike()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.Like, "A")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] LIKE @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndLikeIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.Like, "A")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] LIKE @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndIn()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field2", Operation.In, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field2] IN (@Field2_In_0, @Field2_In_1, @Field2_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithEqualAndInIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Equal, 1),
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 AND [Field1] IN (@Field1_1_In_0, @Field1_1_In_1, @Field1_1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region NonAlphaNumericChars

        [TestMethod]
        public void TestQueryGroupWithSpace()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field 1", Operation.Equal, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field 1] = @Field_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupWithSpaces()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Date Of Birth", Operation.Equal, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Date Of Birth] = @Date_Of_Birth)";

            // Assert
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestQueryGroupWithInvalidChars()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Date.Of.Birth/BirthDay", Operation.Equal, 1));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Date.Of.Birth/BirthDay] = @Date_Of_Birth_BirthDay)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
