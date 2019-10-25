using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        #region In

        [TestMethod]
        public void TestQueryGroupOperationIn()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.In, new[] { 1, 2, 3 }));

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationInWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 }),
                new QueryField("Field2", Operation.In, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) AND [Field2] IN (@Field2_In_0, @Field2_In_1, @Field2_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationInWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 }),
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) AND [Field1] IN (@Field1_1_In_0, @Field1_1_In_1, @Field1_1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationInWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 }),
                new QueryField("Field1", Operation.In, new [] { 1, 2, 3 })
            },
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) OR [Field1] IN (@Field1_1_In_0, @Field1_1_In_1, @Field1_1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region NotIn

        [TestMethod]
        public void TestQueryGroupOperationNotIn()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotIn, new[] { 1, 2, 3 }));

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotInWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotIn, new [] { 1, 2, 3 }),
                new QueryField("Field2", Operation.NotIn, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) AND [Field2] NOT IN (@Field2_In_0, @Field2_In_1, @Field2_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotInWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotIn, new [] { 1, 2, 3 }),
                new QueryField("Field1", Operation.NotIn, new [] { 1, 2, 3 })
            });

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) AND [Field1] NOT IN (@Field1_1_In_0, @Field1_1_In_1, @Field1_1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotInWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotIn, new [] { 1, 2, 3 }),
                new QueryField("Field1", Operation.NotIn, new [] { 1, 2, 3 })
            },
            Conjunction.Or);

            // Act
            var actual = queryGroup.GetString(m_dbSetting);
            var expected = "([Field1] NOT IN (@Field1_In_0, @Field1_In_1, @Field1_In_2) OR [Field1] NOT IN (@Field1_1_In_0, @Field1_1_In_1, @Field1_1_In_2))";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
