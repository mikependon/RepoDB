using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions.QueryFields;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LiteralQuerFieldTest
	{
        [TestMethod]
        public void TestLiteralQueryFieldConstructor()
        {
            // Prepare
            var literalQueryField = new LiteralQueryField("[Id] BETWEEN 10 AND 100");

            // Assert
            Assert.AreEqual("fake", literalQueryField.Field.Name);
            Assert.AreEqual("[Id] BETWEEN 10 AND 100", literalQueryField.Literal);
        }

        [TestMethod]
        public void TestLiteralQueryFieldGetString()
        {
            // Prepare
            var literalQueryField = new LiteralQueryField("[Id] BETWEEN 10 AND 100");

            // Assert
            Assert.AreEqual("[Id] BETWEEN 10 AND 100", literalQueryField.GetString(0, null));
        }
    }
}

