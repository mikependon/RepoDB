using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using RepoDb.UnitTests.Setup;
using System.Collections;
using System.Collections.Generic;

namespace RepoDb.UnitTests.Equalities
{
    [TestClass]
    public class FieldEqualityTest
    {
        [TestMethod]
        public void TestFieldEqualityFromString()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = Equals(objA, "FieldName".AsQuoted(true, Helper.DbSetting));

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldNameCaseSensitivity()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = Equals(objA, "fieldname");

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestFieldHashCodeEquality()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);
            var objB = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == objB.GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldHashCodeEqualityFromLiteralString()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == "FieldName".AsQuoted(true, Helper.DbSetting).GetHashCode());

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldHashCodeEqualityFromImproperString()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = (objA.GetHashCode() == "[fieldname]".GetHashCode());

            // Assert
            Assert.IsFalse(equal);
        }

        [TestMethod]
        public void TestFieldObjectEquality()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);
            var objB = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = (objA == objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldObjectEqualityFromEqualsMethod()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);
            var objB = new Field("FieldName", Helper.DbSetting);

            // Act
            var equal = Equals(objA, objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldFromArrayListContainability()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);
            var objB = new Field("FieldName", Helper.DbSetting);
            var list = new ArrayList();

            // Act
            list.Add(objA);
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldGenericListContainability()
        {
            // Prepare
            var objA = new Field("FieldName", Helper.DbSetting);
            var objB = new Field("FieldName", Helper.DbSetting);
            var list = new List<Field>() { objA };

            // Act
            var equal = list.Contains(objB);

            // Assert
            Assert.IsTrue(equal);
        }
    }
}
