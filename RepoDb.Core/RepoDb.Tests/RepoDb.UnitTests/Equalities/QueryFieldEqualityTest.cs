using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;
using Moq;
using System.Data;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class QueryFieldEqualityTest
    {
        [TestMethod]
        public void TestQueryFieldHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", "Value1");
            var objB = new QueryField("FieldName", "Value2");

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldWithOperationHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.Equal, "Value2");

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldWithDifferentOperationHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.NotEqual, "Value2");

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryFieldObjectEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.Equal, "Value2");

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldObjectEqualityFromEqualsMethod()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.Equal, "Value2");

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldArrayListContainability()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.Equal, "Value2");
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldGenericListContainability()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1");
            var objB = new QueryField("FieldName", Operation.Equal, "Value2");
            var list = new List<QueryField>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
        
        [TestMethod]
        public void TestQueryFieldGetHashCodeInvocationOnCheckNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<QueryField>("QueryFieldName", Operation.Equal, "Value1");

            // Act
            if (mockOfFiled.Object != null){}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }
        
        [TestMethod]
        public void TestQueryFieldGetHashCodeInvocationOnCheckNull()
        {
            // Prepare
            var mockOfFiled = new Mock<QueryField>("QueryFieldName", Operation.Equal, "Value1");

            // Act
            if (mockOfFiled.Object == null){}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }
        
        [TestMethod]
        public void TestQueryFieldGetHashCodeInvocationOnEqualsNull()
        {
            // Prepare
            var mockOfFiled = new Mock<QueryField>("QueryFieldName", Operation.Equal, "Value1");

            // Act
            mockOfFiled.Object.Equals(null);
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Never);
        }

        [TestMethod]
        public void TestQueryFieldGetHashCodeInvocationOnEqualsNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<QueryField>("QueryFieldName", Operation.Equal, "Value1");
            var otherFiled = new QueryField("QueryFieldName", Operation.Equal, "Value1");

            // Act
            mockOfFiled.Object.Equals(otherFiled);
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }
        
        [TestMethod]
        public void TestQueryFieldGetHashCodeInvocationOnEqualityNotNull()
        {
            // Prepare
            var mockOfFiled = new Mock<QueryField>("QueryFieldName", Operation.Equal, "Value1");
            var otherFiled = new QueryField("QueryFieldName", Operation.Equal, "Value1");

            // Act
            if (mockOfFiled.Object == otherFiled) {}
            
            // Assert
            mockOfFiled.Verify(x => x.GetHashCode(), Times.Once);
        }

        [TestMethod]
        public void TestQueryFieldWithDbTypeHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", DbType.StringFixedLength);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", DbType.StringFixedLength);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldWithDbTypeObjectEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", DbType.StringFixedLength);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", DbType.StringFixedLength);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
