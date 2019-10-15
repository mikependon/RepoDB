using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.UnitTests.Setup;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class QueryFieldEqualityTest
    {
        [TestMethod]
        public void TestQueryFieldHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", "Value2", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldWithOperationHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldWithDifferentOperationHashCodeEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.NotEqual, "Value2", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestQueryFieldObjectEquality()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldObjectEqualityFromEqualsMethod()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldArrayListContainability()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", Helper.DbSetting);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestQueryFieldGenericListContainability()
        {
            // Prepare
            var objA = new QueryField("FieldName", Operation.Equal, "Value1", Helper.DbSetting);
            var objB = new QueryField("FieldName", Operation.Equal, "Value2", Helper.DbSetting);
            var list = new List<QueryField>();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
