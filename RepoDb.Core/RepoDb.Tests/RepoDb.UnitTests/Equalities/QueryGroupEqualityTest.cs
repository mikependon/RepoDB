using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class QueryGroupEqualityTest
    {
        private class EntityClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #region HashCode == HashCode

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForDynamics()
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
        public void TestQueryGroupHashCodeEqualityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable());
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable());

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForDynamics()
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
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            });
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            });

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2");

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        #endregion

        #region Object == Object

        [TestMethod]
        public void TestQueryGroupObjectEqualityForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 });
            var objB = QueryGroup.Parse(new { Id = 2 });

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable());
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable());

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForDynamics()
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
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            });
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            });

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2");

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        #endregion

        #region Equals(Object, Object)

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 });
            var objB = QueryGroup.Parse(new { Id = 2 });

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable());
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable());

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForDynamics()
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
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            });
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            });

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2");

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        #endregion

        #region ArrayList.Contains

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 });
            var objB = QueryGroup.Parse(new { Id = 2 });
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable());
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable());
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForDynamics()
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
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            });
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            });
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name == "Name2");
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        #endregion

        #region List<T>.Contains

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 });
            var objB = QueryGroup.Parse(new { Id = 2 });
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable());
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable());
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForDynamics()
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

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            });
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            });
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name == "Name2");
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        #endregion
    }
}
