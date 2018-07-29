using NUnit.Framework;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class OrderTextTest
    {
        private TextAttribute GetOrderTextAttribute(Order order)
        {
            return typeof(Order)
                .GetTypeInfo()
                .GetMembers()
                .First(member => member.Name.ToLower() == order.ToString().ToLower())
                .GetCustomAttribute<TextAttribute>();
        }

        [Test]
        public void TestAscending()
        {
            // Prepare
            var operation = Order.Ascending;

            // Act
            var parsed = GetOrderTextAttribute(operation);

            // Assert
            Assert.AreEqual("ASC", parsed.Text);
        }

        [Test]
        public void TestDescending()
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
