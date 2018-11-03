using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Comparers
{
    [TestClass]
    public class FieldComparerTest
    {
        [TestMethod]
        public void TestEqualsToString()
        {
            // Prepare
            var objA = new Field("FieldName");

            // Act
            var equal = Equals(objA, "FieldName");

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestNotEqualsToImproperString()
        {
            // Prepare
            var objA = new Field("FieldName");

            // Act
            var equal = Equals(objA, "fieldname");

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var objA = new Field("FieldName");
            var objB = new Field("FieldName");

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityFromString()
        {
            // Prepare
            var objA = new Field("FieldName");

            // Act
            var equal = (objA.GetHashCode() == "FieldName".GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var objA = new Field("FieldName");

            // Act
            var equal = (objA.GetHashCode() == "fieldname".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var objA = new Field("FieldName");
            var objB = new Field("FieldName");

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var objA = new Field("FieldName");
            var objB = new Field("FieldName");

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var objA = new Field("FieldName");
            var objB = new Field("FieldName");
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
            var objA = new Field("FieldName");
            var objB = new Field("FieldName");
            var list = new List<Field>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
