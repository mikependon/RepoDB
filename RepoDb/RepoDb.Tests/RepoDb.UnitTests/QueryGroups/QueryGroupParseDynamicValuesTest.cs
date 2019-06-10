using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        [TestMethod]
        public void TestQueryGroupParseDynamicValueForNullField()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = (object)null });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual((object)null, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseDynamicValueForSingleField()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = 1 });

            // Act
            var actual = parsed.QueryFields.First().Parameter.Value;

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestQueryGroupParseDynamicValueForMultipleFields()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = 1, Field2 = 2 });

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual(1, actual1);
            Assert.AreEqual(2, actual2);
        }

        [TestMethod]
        public void TestQueryGroupParseDynamicValueForEnums()
        {
            // Setup
            var parsed = QueryGroup.Parse(new { Field1 = Direction.West, Field2 = Direction.East });

            // Act
            var actual1 = parsed.QueryFields.First().Parameter.Value;
            var actual2 = parsed.QueryFields.Last().Parameter.Value;

            // Assert
            Assert.AreEqual(Direction.West, actual1);
            Assert.AreEqual(Direction.East, actual2);
        }
    }
}
