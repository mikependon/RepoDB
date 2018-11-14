using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class ParameterEqualityTest
    {
        [TestMethod]
        public void TestEqualsToString()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());

            // Act
            var equal = Equals(objA, "ParameterName");

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestNotEqualsToImproperString()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());

            // Act
            var equal = Equals(objA, "Parametername");

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());
            var objB = new Parameter("ParameterName", new object());

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityFromString()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());

            // Act
            var equal = (objA.GetHashCode() == "ParameterName".GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());

            // Act
            var equal = (objA.GetHashCode() == "Parametername".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());
            var objB = new Parameter("ParameterName", new object());

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());
            var objB = new Parameter("ParameterName", new object());

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var objA = new Parameter("ParameterName", new object());
            var objB = new Parameter("ParameterName", new object());
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
            var objA = new Parameter("ParameterName", new object());
            var objB = new Parameter("ParameterName", new object());
            var list = new List<Parameter>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
