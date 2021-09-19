using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class RightTrimQueryFieldTest
    {
        [TestMethod]
        public void TestRightTrimQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new RightTrimQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("RTRIM({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestRightTrimQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new RightTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RTRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestRightTrimQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new RightTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RTRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestRightTrimQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new RightTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RTRIM([FieldName]) = @FieldName_1", text);
        }
    }
}
