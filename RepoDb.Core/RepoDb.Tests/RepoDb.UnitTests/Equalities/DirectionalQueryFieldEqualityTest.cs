using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;
using Moq;
using System.Data;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class DirectionalQueryFieldEqualityTest
    {
        [TestMethod]
        public void TestDirectionalQueryFieldHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", "Value2", ParameterDirection.Output);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithOperationHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", ParameterDirection.Output);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithDifferentOperationHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, "Value2", ParameterDirection.Output);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithSizeHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", 100, ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, "Value2", 100, ParameterDirection.Output);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithTypeHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, typeof(string), ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, typeof(string), ParameterDirection.Output);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldObjectEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", ParameterDirection.Output);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldObjectEqualityFromEqualsMethod()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", ParameterDirection.Output);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldArrayListContainability()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", ParameterDirection.Output);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGenericListContainability()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", ParameterDirection.Output);
            var list = new List<DirectionalQueryField>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnCheckNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);

            // Act
            if (mockOfFiled.Object != null) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnCheckNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);

            // Act
            if (mockOfFiled.Object == null) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualsNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);

            // Act
            mockOfFiled.Object.Equals(null);

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualsNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var otherFiled = new DirectionalQueryField("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);

            // Act
            mockOfFiled.Object.Equals(otherFiled);

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualityNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);
            var otherFiled = new DirectionalQueryField("DirectionalQueryFieldName", Operation.Equal, "Value1", ParameterDirection.Output);

            // Act
            if (mockOfFiled.Object == otherFiled) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }
    }
}
