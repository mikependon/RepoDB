using NUnit.Framework;
using RepoDb.Enumerations;
using System;
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

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
        }

        [Test]
        public void TestDescending()
        {
            // Prepare
            var orderBy = new { Id = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Descending, orderField.First().Order);
        }

        [Test]
        public void TestAscendingAndDescending()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending, Value = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
            Assert.AreEqual(Order.Descending, orderField.Last().Order);
        }

        [Test]
        public void ThrowExceptionIfTheFieldValueIsNotAnOrderType()
        {
            // Prepare
            var orderBy = new { Id = "NotAnOrderType" };

            // Act/Assert
            Assert.Throws(typeof(InvalidOperationException), () => OrderField.Parse(orderBy));
        }

        [Test]
        public void ThrowExceptionIfTheObjectIsNull()
        {
            // Prepare
            var orderBy = (object)null;

            // Act/Assert
            Assert.Throws(typeof(NullReferenceException), () => OrderField.Parse(orderBy));
        }
    }
}
