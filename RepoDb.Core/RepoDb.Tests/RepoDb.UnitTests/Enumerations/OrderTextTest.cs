using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OrderTextTest
    {
        [TestMethod]
        public void TestOrderAscendingText()
        {
            // Prepare
            var operation = Order.Ascending;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("ASC", text);
        }

        [TestMethod]
        public void TestOrderDescendingText()
        {
            // Prepare
            var operation = Order.Descending;

            // Act
            var text = operation.GetText();

            // Assert
            Assert.AreEqual("DESC", text);
        }
    }
}
