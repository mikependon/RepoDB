using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests.Enumerations
{
    [TestClass]
    public class OrderTextTest
    {
        private TextAttribute GetOrderTextAttribute(Order order)
        {
            return typeof(Order)
                .GetMembers()
                .First(member => member.Name.ToLowerInvariant() == order.ToString().ToLowerInvariant())
                .GetCustomAttribute<TextAttribute>();
        }

        [TestMethod]
        public void TestOrderAscendingText()
        {
            // Prepare
            var operation = Order.Ascending;

            // Act
            var parsed = GetOrderTextAttribute(operation);

            // Assert
            Assert.AreEqual("ASC", parsed.Text);
        }

        [TestMethod]
        public void TestOrderDescendingText()
        {
            // Prepare
            var operation = Order.Descending;

            // Act
            var parsed = GetOrderTextAttribute(operation);

            // Assert
            Assert.AreEqual("DESC", parsed.Text);
        }
    }
}
