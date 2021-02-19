using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class OrderFieldTest
    {
        #region SubClasses

        public class OrderFieldTestClass
        {
            public int Id { get; set; }
        }

        private class OrderFieldTestMappedClass
        {
            public int Id { get; set; }
            [Map("PropertyText")]
            public string PropertyString { get; set; }
        }

        #endregion

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

        // Ascending

        [TestMethod]
        public void TestOrderFieldForAscending()
        {
            // Act
            var parsed = OrderField.Ascending<OrderFieldTestClass>(p => p.Id);

            // Assert
            Assert.AreEqual(Order.Ascending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldAscendingFromMappedProperty()
        {
            // Act
            var parsed = OrderField.Ascending<OrderFieldTestMappedClass>(p => p.PropertyString);

            // Assert
            Assert.AreEqual("PropertyText", parsed.Name);
            Assert.AreEqual(Order.Ascending, parsed.Order);
        }

        // Descending

        [TestMethod]
        public void TestOrderFieldForDescending()
        {
            // Act
            var parsed = OrderField.Descending<OrderFieldTestClass>(p => p.Id);

            // Assert
            Assert.AreEqual(Order.Descending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFielDecendingFromMappedProperty()
        {
            // Act
            var parsed = OrderField.Descending<OrderFieldTestMappedClass>(p => p.PropertyString);

            // Assert
            Assert.AreEqual("PropertyText", parsed.Name);
            Assert.AreEqual(Order.Descending, parsed.Order);
        }

        // Parse (Ascending)

        [TestMethod]
        public void TestOrderFieldParseExpressionForAscending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Ascending);

            // Assert
            Assert.AreEqual(Order.Ascending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionFromMappedPropertyForAscending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestMappedClass>(p => p.PropertyString, Order.Ascending);

            // Assert
            Assert.AreEqual("PropertyText", parsed.Name);
            Assert.AreEqual(Order.Ascending, parsed.Order);
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

        // Parse (Descending)

        [TestMethod]
        public void TestOrderFieldParseExpressionForDescending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestClass>(p => p.Id, Order.Descending);

            // Assert
            Assert.AreEqual(Order.Descending, parsed.Order);
        }

        [TestMethod]
        public void TestOrderFieldParseExpressionFromMappedPropertyForDescending()
        {
            // Act
            var parsed = OrderField.Parse<OrderFieldTestMappedClass>(p => p.PropertyString, Order.Descending);

            // Assert
            Assert.AreEqual("PropertyText", parsed.Name);
            Assert.AreEqual(Order.Descending, parsed.Order);
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

        // Exceptions

        [TestMethod, ExpectedException(typeof(InvalidExpressionException))]
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
