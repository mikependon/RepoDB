using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        #region Like

        [TestMethod]
        public void TestQueryGroupOperationLike()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.Like, "A%"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationLikeWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Like, "A%"),
                new QueryField("Field2", Operation.Like, "B%")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] LIKE @Field1 AND [Field2] LIKE @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationLikeWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Like, "A%"),
                new QueryField("Field1", Operation.Like, "B%")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] LIKE @Field1 AND [Field1] LIKE @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationLikeWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Like, "A%"),
                new QueryField("Field1", Operation.Like, "B%")
            },
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] LIKE @Field1 OR [Field1] LIKE @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region NotLike

        [TestMethod]
        public void TestQueryGroupOperationNotLike()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotLike, "A%"));

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT LIKE @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotLikeWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotLike, "A%"),
                new QueryField("Field2", Operation.NotLike, "B%")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT LIKE @Field1 AND [Field2] NOT LIKE @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotLikeWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotLike, "A%"),
                new QueryField("Field1", Operation.NotLike, "B%")
            });

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT LIKE @Field1 AND [Field1] NOT LIKE @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotLikeWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotLike, "A%"),
                new QueryField("Field1", Operation.NotLike, "B%")
            },
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT LIKE @Field1 OR [Field1] NOT LIKE @Field1_1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
