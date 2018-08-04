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
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (fieldA.GetHashCode() == fieldB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (fieldA.GetHashCode() == "fieldname".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (fieldA == fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = Equals(fieldA, fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Ascending);
            var list = new ArrayList();

            // Act
            list.Add(fieldA);
            var equal = list.Contains(fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGenericListContains()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Ascending);
            var list = new List<OrderField>();

            // Act
            list.Add(fieldA);
            var equal = list.Contains(fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualOperatorInequalityByOrder()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Descending);

            // Act
            var equal = (fieldA == fieldB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualsMethodInequalityByOrder()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Descending);

            // Act
            var equal = Equals(fieldA, fieldB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestArrayListContainsInequalityByOrder()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Descending);
            var list = new ArrayList();

            // Act
            list.Add(fieldA);
            var equal = list.Contains(fieldB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGenericListContainsInequalityByOrder()
        {
            // Prepare
            var fieldA = new OrderField("OrderFieldName", Order.Ascending);
            var fieldB = new OrderField("OrderFieldName", Order.Descending);
            var list = new List<OrderField>();

            // Act
            list.Add(fieldA);
            var equal = list.Contains(fieldB);

            // Assert
            Assert.IsFalse(equal);
        }
    }
}
