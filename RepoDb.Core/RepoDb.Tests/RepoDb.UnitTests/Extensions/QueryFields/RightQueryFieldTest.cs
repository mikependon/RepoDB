using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class RightQueryFieldTest
    {
        [TestMethod]
        public void TestRightQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new RightQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("RIGHT({0}, 5)", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestRightQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new RightQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RIGHT([FieldName], 5) = @FieldName", text);
        }

        [TestMethod]
        public void TestRightQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new RightQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RIGHT([FieldName], 5) = @FieldName", text);
        }

        [TestMethod]
        public void TestRightQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new RightQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("RIGHT([FieldName], 5) = @FieldName_1", text);
        }
    }
}
