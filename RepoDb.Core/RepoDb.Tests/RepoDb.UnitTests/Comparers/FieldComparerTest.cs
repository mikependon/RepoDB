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
            var fieldA = new Field("FieldName");

            // Act
            var equal = (Equals(fieldA, "FieldName"));

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestNotEqualsToImproperString()
        {
            // Prepare
            var fieldA = new Field("FieldName");

            // Act
            var equal = (Equals(fieldA, "fieldname"));

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var fieldA = new Field("FieldName");
            var fieldB = new Field("FieldName");

            // Act
            var equal = (fieldA.GetHashCode() == fieldB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityFromString()
        {
            // Prepare
            var fieldA = new Field("FieldName");

            // Act
            var equal = (fieldA.GetHashCode() == "FieldName".GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var fieldA = new Field("FieldName");

            // Act
            var equal = (fieldA.GetHashCode() == "fieldname".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var fieldA = new Field("FieldName");
            var fieldB = new Field("FieldName");

            // Act
            var equal = (fieldA == fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var fieldA = new Field("FieldName");
            var fieldB = new Field("FieldName");

            // Act
            var equal = Equals(fieldA, fieldB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var fieldA = new Field("FieldName");
            var fieldB = new Field("FieldName");
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
            var fieldA = new Field("FieldName");
            var fieldB = new Field("FieldName");
            var list = new List<Field>();

            // Act
            list.Add(fieldA);
            var equal = list.Contains(fieldB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
