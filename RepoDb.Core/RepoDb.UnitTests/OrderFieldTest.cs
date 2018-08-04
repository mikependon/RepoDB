using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class OrderFieldTest
    {
        [TestMethod]
        public void TestAscending()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
        }

        [TestMethod]
        public void TestDescending()
        {
            // Prepare
            var orderBy = new { Id = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Descending, orderField.First().Order);
        }

        [TestMethod]
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

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfTheFieldValueIsNotAnOrderType()
        {
            // Prepare
            var orderBy = new { Id = "NotAnOrderType" };

            // Act/Assert
            OrderField.Parse(orderBy);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfTheObjectIsNull()
        {
            // Prepare
            var orderBy = (object)null;

            // Act/Assert
            OrderField.Parse(orderBy);
        }
    }
}
