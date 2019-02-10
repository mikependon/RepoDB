using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
