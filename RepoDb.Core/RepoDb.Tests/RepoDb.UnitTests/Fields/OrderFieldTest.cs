using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Fields
{
    [TestClass]
    public class OrderFieldTest
    {
        public class OrderFieldTestClass
        {
            public int Id { get; set; }
        }

        [TestMethod]
        public void TestOrderFieldNameAndStringEquality()
        {
            // Prepare
            var orderField = new OrderField("FieldName", Order.Ascending);

            // Act
            var equal = Equals("FieldName", orderField.Name);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionForAscending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Ascending);

            // Assert
            Assert.AreEqual(Order.Ascending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionForDescending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Descending);

            // Assert
            Assert.AreEqual(Order.Descending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithAscendingOrder()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithDescendingOrder()
        {
            // Prepare
            var orderBy = new { Id = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Descending, orderField.First().Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithAscendingAndDescendingOrders()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending, Value = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
            Assert.AreEqual(Order.Descending, orderField.Last().Order);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnOrderFieldIfTheParseLinqExpressionHasNoProperty()
        {
            // Act/Assert
            OrderField.Parse<OrderFieldTestClass>(p => "A", Order.Ascending);
        }

        [TestMethod, ExpectedException(typeof(InvalidTypeException))]
        public void ThrowExceptionOnOrderFieldIfTheParseDynamicObjectFieldValueIsNotAnOrderType()
        {
            // Prepare
            var orderBy = new { Id = "NotAnOrderType" };

            // Act/Assert
            OrderField.Parse(orderBy);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnOrderFieldIfTheParseDynamicObjectIsNull()
        {
            // Prepare
            var orderBy = (object)null;

            // Act/Assert
            OrderField.Parse(orderBy);
        }
    }
}
