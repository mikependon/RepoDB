using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Conjunction Operation

        [TestMethod]
        public void TestQueryGroupWithNoConjunction()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
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
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
            }, Conjunction.And);

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
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
            }, Conjunction.Or);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] = @Field1 OR [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        // IsNot Operation

        [TestMethod]
        public void TestQueryGroupWithNoIsNot()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
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
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
            }, false);

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
                new QueryField("Field1", "Value1"),
                new QueryField("Field2", "Value2")
            }, true);

            // Act
            var actual = queryGroup.GetString();
            var expected = "NOT ([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
