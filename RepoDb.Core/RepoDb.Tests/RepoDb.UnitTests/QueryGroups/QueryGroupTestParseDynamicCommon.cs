using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // No Operation

        [TestMethod]
        public void TestParseDynamicNoOperation()
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
        public void TestParseDynamicNoOperationForIsNull()
        {
            // Setup
            var expression = new { Field1 = (object)null };

            // Act
            var actual = QueryGroup.Parse(expression).GetString();
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // FixParameters

        [TestMethod]
        public void TestParseDynamicFixParametersOnASingleFieldWithMultipleExpression()
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
        public void TestParseDynamicChildQueryGroupsSingle()
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
        public void TestParseDynamicChildQueryGroupsSingleForConjunctionOr()
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
        public void TestParseDynamicChildQueryGroupsMultiple()
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
