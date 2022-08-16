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
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithOperationHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithDifferentOperationHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, "Value2", null, ParameterDirection.Output, null);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithSizeHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", 100, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, "Value2", 100, ParameterDirection.Output, null);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldWithTypeHashCodeEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, typeof(string), null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.NotEqual, typeof(string), null, ParameterDirection.Output, null);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldObjectEquality()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldObjectEqualityFromEqualsMethod()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldArrayListContainability()
        {
            // Prepare
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);
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
            var objA = new DirectionalQueryField("FieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var objB = new DirectionalQueryField("FieldName", Operation.Equal, "Value2", null, ParameterDirection.Output, null);
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
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);

            // Act
            if (mockOfFiled.Object != null) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnCheckNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);

            // Act
            if (mockOfFiled.Object == null) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualsNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);

            // Act
            mockOfFiled.Object.Equals(null);

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualsNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var otherFiled = new DirectionalQueryField("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);

            // Act
            mockOfFiled.Object.Equals(otherFiled);

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }

        [TestMethod]
        public void TestDirectionalQueryFieldGetHashCodeInvocationOnEqualityNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<DirectionalQueryField>("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);
            var otherFiled = new DirectionalQueryField("DirectionalQueryFieldName", Operation.Equal, "Value1", null, ParameterDirection.Output, null);

            // Act
            if (mockOfFiled.Object == otherFiled) { }

            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }
    }
}
