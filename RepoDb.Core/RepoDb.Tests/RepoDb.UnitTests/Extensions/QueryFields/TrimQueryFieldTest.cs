using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class TrimQueryFieldTest
    {
        [TestMethod]
        public void TestTrimQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new TrimQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("TRIM({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestTrimQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new TrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("TRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestTrimQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new TrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("TRIM([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestTrimQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new TrimQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("TRIM([FieldName]) = @FieldName_1", text);
        }
    }
}
