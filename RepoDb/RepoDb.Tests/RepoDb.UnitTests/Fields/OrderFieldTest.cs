using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.UnitTests.Setup;
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
        public void TestOrderFieldQuotes()
        {
            // Prepare
            var objA = new OrderField("FieldName", Order.Ascending, Helper.DbSetting);

            // Act
            var equal = Equals("[FieldName]", objA.Name);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionForAscending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Ascending, Helper.DbSetting);

            // Assert
            Assert.AreEqual(Order.Ascending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionForDescending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Descending, Helper.DbSetting);

            // Assert
            Assert.AreEqual(Order.Descending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithAscendingOrder()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending };

            // Act
            var orderField = OrderField.Parse(orderBy, Helper.DbSetting);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithDescendingOrder()
        {
            // Prepare
            var orderBy = new { Id = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy, Helper.DbSetting);

            // Assert
            Assert.AreEqual(Order.Descending, orderField.First().Order);
        }

        [TestMethod]
        public void TestOrderFieldParseForDynamicObjectWithAscendingAndDescendingOrders()
        {
            // Prepare
            var orderBy = new { Id = Order.Ascending, Value = Order.Descending };

            // Act
            var orderField = OrderField.Parse(orderBy, Helper.DbSetting);

            // Assert
            Assert.AreEqual(Order.Ascending, orderField.First().Order);
            Assert.AreEqual(Order.Descending, orderField.Last().Order);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionOnOrderFieldIfTheParseLinqExpressionHasNoProperty()
        {
            // Act/Assert
            OrderField.Parse<OrderFieldTestClass>(p => "A", Order.Ascending, Helper.DbSetting);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionOnOrderFieldIfTheParseDynamicObjectFieldValueIsNotAnOrderType()
        {
            // Prepare
            var orderBy = new { Id = "NotAnOrderType" };

            // Act/Assert
            OrderField.Parse(orderBy, Helper.DbSetting);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnOrderFieldIfTheParseDynamicObjectIsNull()
        {
            // Prepare
            var orderBy = (object)null;

            // Act/Assert
            OrderField.Parse(orderBy, Helper.DbSetting);
        }
    }
}
