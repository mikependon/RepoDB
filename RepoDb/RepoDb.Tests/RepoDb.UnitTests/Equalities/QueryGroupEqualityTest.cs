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
        public void TestQueryGroupHashCodeEquality()
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
        public void TestQueryGroupWithMultipleFieldsHashCodeEquality()
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
        public void TestQueryGroupWithConjunctionHashCodeEquality()
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
        public void TestQueryGroupGetWithDifferentConjunctionHashCodeEquality()
        {
            // Prepare
            var objA = new QueryGroup(new[] { new QueryField("Name1", "Value1") }, Conjunction.And);
            var objB = new QueryGroup(new[] { new QueryField("Name1", "Value1") }, Conjunction.Or);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupWithChildQueryGroupsHashCodeEquality()
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
        public void TestQueryGroupObjectEquality()
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
        public void TestQueryGroupObjectEqualityFromEquals()
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
        public void TestQueryGroupArrayListContainability()
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
        public void TestQueryGroupGenericListContainability()
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
