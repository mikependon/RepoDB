using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;
using Moq;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class OrderFieldEqualityTest
    {
        [TestMethod]
        public void TestOrderFieldHashCodeEquality()
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
        public void TestOrderFieldHashCodeEqualityFromImproperString()
        {
            // Prepare
            var objA = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            var equal = (objA.GetHashCode() == "fieldname".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestOrderFieldObjectEquality()
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
        public void TestOrderFieldObjectEqualityFromEqualsMethod()
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
        public void TestOrderFieldArrayListContainability()
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
        public void TestOrderFieldGenericListContainability()
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
        public void TestOrderFieldIneqaulityByOrder()
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
        public void TestOrderFieldEqualityByOrderFromEqualsMethod()
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
        public void TestOrderFieldArrayListInequalityByOrder()
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
        public void TestOrderFieldGenericListInequalityByOrder()
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
        
        [TestMethod]
        public void TestOrderFieldGetHashCodeInvocationOnCheckNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

            // Act
            if (mockOfFiled.Object != null){}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }
        
        [TestMethod]
        public void TestOrderFieldGetHashCodeInvocationOnCheckNull()
        {
            // Prepare
            var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

            // Act
            if (mockOfFiled.Object == null){}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }
        
        [TestMethod]
        public void TestOrderFieldGetHashCodeInvocationOnEqualsNull()
        {
            // Prepare
            var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);

            // Act
            mockOfFiled.Object.Equals(null);
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestOrderFieldGetHashCodeInvocationOnEqualsNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);
            var otherFiled = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            mockOfFiled.Object.Equals(otherFiled);
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }
        
        [TestMethod]
        public void TestOrderFieldGetHashCodeInvocationOnEqualityNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<OrderField>("OrderFieldName", Order.Ascending);
            var otherFiled = new OrderField("OrderFieldName", Order.Ascending);

            // Act
            if (mockOfFiled.Object == otherFiled) {}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }
    }
}
