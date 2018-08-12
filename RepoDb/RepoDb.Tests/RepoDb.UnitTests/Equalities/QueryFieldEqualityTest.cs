using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Comparers
{
    [TestClass]
    public class QueryFieldComparerTest
    {
        [TestMethod]
        public void TestGetHashCodeEquality()
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
        public void TestGetHashCodeEqualityWithOperation()
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
        public void TestGetHashCodeInequalityWithDifferentOperation()
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
        public void TestEqualOperator()
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
        public void TestEqualsMethod()
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
        public void TestArrayListContains()
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
        public void TestGenericListContains()
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
    }
}
