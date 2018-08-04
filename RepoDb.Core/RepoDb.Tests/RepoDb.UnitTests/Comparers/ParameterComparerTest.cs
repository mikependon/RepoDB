using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Comparers
{
    [TestClass]
    public class ParameterComparerTest
    {
        [TestMethod]
        public void TestEqualsToString()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());

            // Act
            var equal = (Equals(ParameterA, "ParameterName"));

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestNotEqualsToImproperString()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());

            // Act
            var equal = (Equals(ParameterA, "Parametername"));

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());
            var ParameterB = new Parameter("ParameterName", new object());

            // Act
            var equal = (ParameterA.GetHashCode() == ParameterB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityFromString()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());

            // Act
            var equal = (ParameterA.GetHashCode() == "ParameterName".GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityFromImproperString()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());

            // Act
            var equal = (ParameterA.GetHashCode() == "Parametername".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());
            var ParameterB = new Parameter("ParameterName", new object());

            // Act
            var equal = (ParameterA == ParameterB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());
            var ParameterB = new Parameter("ParameterName", new object());

            // Act
            var equal = Equals(ParameterA, ParameterB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());
            var ParameterB = new Parameter("ParameterName", new object());
            var list = new ArrayList();

            // Act
            list.Add(ParameterA);
            var equal = list.Contains(ParameterB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGenericListContains()
        {
            // Prepare
            var ParameterA = new Parameter("ParameterName", new object());
            var ParameterB = new Parameter("ParameterName", new object());
            var list = new List<Parameter>();

            // Act
            list.Add(ParameterA);
            var equal = list.Contains(ParameterB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
