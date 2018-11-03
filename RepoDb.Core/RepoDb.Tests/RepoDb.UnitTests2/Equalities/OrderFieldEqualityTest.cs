using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Comparers
{
    [TestClass]
    public class OrderFieldComparerTest
    {
        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (objA.GetHashCode() == "fieldname".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Ascending);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGenericListContains()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Ascending);
            var list = new List<OrderField>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualOperatorInequalityByOrder()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Descending);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualsMethodInequalityByOrder()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Descending);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestArrayListContainsInequalityByOrder()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Descending);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGenericListContainsInequalityByOrder()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);
            var objB = new OrderField("OrderFieldName", Order.Descending);
            var list = new List<OrderField>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }
    }
}
