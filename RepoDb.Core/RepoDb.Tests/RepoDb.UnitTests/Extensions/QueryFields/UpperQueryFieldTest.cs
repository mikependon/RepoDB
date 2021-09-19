using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class UpperQueryFieldTest
    {
        [TestMethod]
        public void TestUpperQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new UpperQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("UPPER({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestUpperQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new UpperQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("UPPER([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestUpperQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new UpperQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("UPPER([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestUpperQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new UpperQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("UPPER([FieldName]) = @FieldName_1", text);
        }
    }
}
