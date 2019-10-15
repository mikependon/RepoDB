using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.UnitTests.Setup;
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
            var objA = QueryGroup.Parse(new { Id = 1 }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2 }, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), false, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == false, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2") == true, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" }, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.Or, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, true, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, false, Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1", Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupHashCodeEqualityWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == false, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name2") == true, Helper.DbSetting);

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
            var objA = QueryGroup.Parse(new { Id = 1 }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2 }, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), false, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => !e.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Name.Contains("Name2") == false, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" }, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.Or, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, true, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, false, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1", Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2", Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2") == false, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsFalse(equal);
        }

        #endregion

        #region Equals(Object, Object)

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1 }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2 }, Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFields()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFieldsWithSameConjuntion()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFieldsWithDifferentConjuntion()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), false, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForDynamics()
        {
            // Prepare
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" }, Helper.DbSetting);

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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForQueryFieldsWithSameConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForQueryFieldsWithDifferentConjunction()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.Or, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForQueryFieldsWithSameIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, true, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForQueryFieldsWithDifferentIsNot()
        {
            // Prepare
            var objA = new QueryGroup(new[]
            {
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, false, Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForExpressions()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name == "Name1", Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name == "Name2", Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && !e.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2"), Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryGroupObjectEqualityViaEqualMethodWithMultipleFieldsForExpressionsWithDifferentBooleanValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(e => e.Id == 1 && e.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(e => e.Id == 2 && e.Name.Contains("Name2") == false, Helper.DbSetting);

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
            var objA = QueryGroup.Parse(new { Id = 1 }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2 }, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2, Helper.DbSetting);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupArrayListContainabilityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"), Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" }, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.Or, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, true, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name == "Name1", Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name == "Name2", Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && !c.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2"), Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2") == false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse(new { Id = 1 }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2 }, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), Conjunction.Or, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
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
            var objA = new QueryGroup(new QueryField("Id", 1, Helper.DbSetting).AsEnumerable(), true, Helper.DbSetting);
            var objB = new QueryGroup(new QueryField("Id", 2, Helper.DbSetting).AsEnumerable(), false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2, Helper.DbSetting);
            var list = new List<QueryGroup>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryGroupGenericListContainabilityForExpressionsWithDifferentUnaryValue()
        {
            // Prepare
            var objA = QueryGroup.Parse<EntityClass>(c => !c.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2"), Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Name.Contains("Name2") == false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse(new { Id = 1, Name = "Name1" }, Helper.DbSetting);
            var objB = QueryGroup.Parse(new { Id = 2, Name = "Name2" }, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, Conjunction.And, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, Conjunction.Or, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, true, Helper.DbSetting);
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
                new QueryField("Id", 1, Helper.DbSetting),
                new QueryField("Name", "Name1", Helper.DbSetting)
            }, true, Helper.DbSetting);
            var objB = new QueryGroup(new[]
            {
                new QueryField("Id", 2, Helper.DbSetting),
                new QueryField("Name", "Name2", Helper.DbSetting)
            }, false, Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name == "Name1", Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name == "Name2", Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && !c.Name.Contains("Name1"), Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2"), Helper.DbSetting);
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
            var objA = QueryGroup.Parse<EntityClass>(c => c.Id == 1 && c.Name.Contains("Name1") == true, Helper.DbSetting);
            var objB = QueryGroup.Parse<EntityClass>(c => c.Id == 2 && c.Name.Contains("Name2") == false, Helper.DbSetting);
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
