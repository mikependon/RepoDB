using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LeftQueryFieldTest
    {
        [TestMethod]
        public void TestLeftQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new LeftQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("LEFT({0}, 5)", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestLeftQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new LeftQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEFT([FieldName], 5) = @FieldName", text);
        }

        [TestMethod]
        public void TestLeftQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new LeftQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEFT([FieldName], 5) = @FieldName", text);
        }

        [TestMethod]
        public void TestLeftQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new LeftQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEFT([FieldName], 5) = @FieldName_1", text);
        }
    }
}
