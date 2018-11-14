using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class QueryGroupEqualityTest
    {
        [TestMethod]
        public void TestGetHashCodeEquality()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 });
            var objB = QueryGroup.Parse(new { Id = 2 });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityWithMultipleFields()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityWithConjunction()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1", Conjunction = Conjunction.And });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2", Conjunction = Conjunction.And });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestGetHashCodeInequalityWithDifferentConjunction()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1", Conjunction = Conjunction.And });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2", Conjunction = Conjunction.Or });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestGetHashCodeEqualityWithChildQueryGroups()
        {
            // Prepare
            var objA = QueryGroup.Parse(
                new
                {
                    Id = 1,
                    QueryGroups = new[] { new { Name = "Name1", Address = "Address1" } }
                });
            var objB = QueryGroup.Parse(
                new
                {
                    Id = 2,
                    QueryGroups = new[] { new { Name = "Name2", Address = "Address2" } }
                });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualOperator()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" });

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestEqualsMethod()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" });

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestArrayListContains()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" });
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
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" });
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" });
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
