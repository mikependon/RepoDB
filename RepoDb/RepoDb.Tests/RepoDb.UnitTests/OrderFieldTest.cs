using NUnit.Framework;
using RepoDb.Enumerations;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class OrderFieldTest
    {
        [Test]
        public void TestAscending()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending };

            // Acts
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.IsNotNull(orderField);
            Assert.AreEqual(1, orderField.Count());
            Assert.AreEqual("Id", orderField.First().Name);
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
        }

        [Test]
        public void TestDescending()
        {
            // Prepare
            var orderBy = new { Id = Order.Descending };

            // Acts
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.IsNotNull(orderField);
            Assert.AreEqual(1, orderField.Count());
            Assert.AreEqual("Id", orderField.First().Name);
            Assert.AreEqual(Order.Descending, orderField.First().Order);
        }

        [Test]
        public void TestAscendingAndDescending()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending, Value = Order.Descending };

            // Acts
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.IsNotNull(orderField);
            Assert.AreEqual(2, orderField.Count());
            Assert.AreEqual("Id", orderField.First().Name);
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
            Assert.AreEqual("Value", orderField.Last().Name);
            Assert.AreEqual(Order.Descending, orderField.Last().Order);
        }
    }
}
