using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.Setup;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        [TestMethod]
        public void TestQueryGroupParseDynamicWithNullField()
        {
            // Setup
            var expression = new { Field1 = (object)null };

            // Act
            var actual = QueryGroup.Parse(expression).GetString(Helper.DbSetting);
            var expected = "([Field1] IS NULL)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseDynamicWithSingleField()
        {
            // Setup
            var expression = new { Field1 = 1 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString(Helper.DbSetting);
            var expected = "([Field1] = @Field1)";

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseDynamicWithMultipleFields()
        {
            // Setup
            var expression = new { Field1 = 1, Field2 = 2 };

            // Act
            var actual = QueryGroup.Parse(expression).GetString(Helper.DbSetting);
            var expected = "([Field1] = @Field1 AND [Field2] = @Field2)";

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
