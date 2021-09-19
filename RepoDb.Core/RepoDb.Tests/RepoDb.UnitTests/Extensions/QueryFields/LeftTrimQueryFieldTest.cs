using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LeftTrimQueryFieldTest
    {
        [TestMethod]
        public void TestLeftTrimQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new LeftTrimQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("LTRIM({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestLeftTrimQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new LeftTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LTRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLeftTrimQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new LeftTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LTRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLeftTrimQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new LeftTrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LTRIM([FieldName]) = @FieldName_1", text);
        }
    }
}
