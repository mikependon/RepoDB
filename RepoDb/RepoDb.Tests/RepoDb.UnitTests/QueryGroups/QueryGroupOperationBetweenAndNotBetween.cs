using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        #region Between

        [TestMethod]
        public void TestQueryGroupOperationBetween()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.Between, new[] { 1, 100 }, Helper.DbSetting), Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] BETWEEN @Field1_Left AND @Field1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationBetweenWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Between, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field2", Operation.Between, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] BETWEEN @Field1_Left AND @Field1_Right AND [Field2] BETWEEN @Field2_Left AND @Field2_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationBetweenWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Between, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field1", Operation.Between, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] BETWEEN @Field1_Left AND @Field1_Right AND [Field1] BETWEEN @Field1_1_Left AND @Field1_1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationBetweenWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.Between, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field1", Operation.Between, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Conjunction.Or,
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] BETWEEN @Field1_Left AND @Field1_Right OR [Field1] BETWEEN @Field1_1_Left AND @Field1_1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region NotBetween

        [TestMethod]
        public void TestQueryGroupOperationNotBetween()
        {
            // Setup
            var queryGroup = new QueryGroup(new QueryField("Field1", Operation.NotBetween, new[] { 1, 100 }, Helper.DbSetting), Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT BETWEEN @Field1_Left AND @Field1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotBetweenWithTwoFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotBetween, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field2", Operation.NotBetween, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT BETWEEN @Field1_Left AND @Field1_Right AND [Field2] NOT BETWEEN @Field2_Left AND @Field2_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotBetweenWithTwoIdenticalFields()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotBetween, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field1", Operation.NotBetween, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT BETWEEN @Field1_Left AND @Field1_Right AND [Field1] NOT BETWEEN @Field1_1_Left AND @Field1_1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupOperationNotBetweenWithTwoIdenticalFieldsWithConjunctionOr()
        {
            // Setup
            var queryGroup = new QueryGroup(new[]
            {
                new QueryField("Field1", Operation.NotBetween, new[] { 1, 100 }, Helper.DbSetting),
                new QueryField("Field1", Operation.NotBetween, new[] { 500, 1000 }, Helper.DbSetting)
            },
            Conjunction.Or,
            Helper.DbSetting);

            // Act
            var actual = queryGroup.GetString();
            var expected = "([Field1] NOT BETWEEN @Field1_Left AND @Field1_Right OR [Field1] NOT BETWEEN @Field1_1_Left AND @Field1_1_Right)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion
    }
}
