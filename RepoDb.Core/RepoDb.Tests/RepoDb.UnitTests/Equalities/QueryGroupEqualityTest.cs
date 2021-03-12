using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
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
            public int Value { get; set; }
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
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.Or);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), true);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), false);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupHashCodeEqualityForCollidedExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2"));

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == false);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2") == true);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.And);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.Or);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, true);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, false);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
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

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2"));

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == false);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2") == true);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupObjectEqualityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.And);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), true);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), false);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupObjectEqualityForCollidedExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => !e.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name2"));

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name2") == false);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.And);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.Or);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, true);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, false);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
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

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2"));

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2") == false);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        #endregion

        #region Equals(Object, Object)

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForDynamics()
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
        public void TestQueryGroupObjectEqualityViaEqualsMethodForQueryFields()
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
        public void TestQueryGroupObjectEqualityViaEqualsMethodForQueryFieldsWithSameConjuntion()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.And);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForQueryFieldsWithDifferentConjuntion()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), true);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), false);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForExpressions()
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
        public void TestQueryGroupObjectEqualityViaEqualsMethodForCollidedExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"));

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForDynamics()
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
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForQueryFields()
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
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.And);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.Or);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, true);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, false);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1");
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2");

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2"));

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualsMethodWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2") == false);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupArrayListContainabilityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.And);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), true);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), false);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupArrayListContainabilityForCollidedExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"));
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.And);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.Or);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, true);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, false);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && !c.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2"));
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityWithMultipleMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2") == false);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupGenericListContainabilityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.And);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), Conjunction.And);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), Conjunction.Or);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), true);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1).AsEnumerable(), true);
            var objB = new QueryGroup(new QueryField("Id", 2).AsEnumerable(), false);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupGenericListContainabilityForCollidedExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Value != 1);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id != 1 && c.Value == 1);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"));
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.And);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, Conjunction.And);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, Conjunction.Or);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, true);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1),
                new QueryField("Name", "Name1")
            }, true);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2),
                new QueryField("Name", "Name2")
            }, false);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
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

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && !c.Name.Contains("Name1"));
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2"));
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityWithMultipleMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name.Contains("Name1") == true);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2") == false);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsFalse(equal);
        }

        #endregion
    }
}
