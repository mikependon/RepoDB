using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LowerQueryFieldTest
    {
        [TestMethod]
        public void TestLowerQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new LowerQueryField("FieldName", Operation.NotEqual, "Value");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("LOWER({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestLowerQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new LowerQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LOWER([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLowerQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new LowerQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LOWER([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLowerQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new LowerQueryField("FieldName", Operation.Equal, "Value");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LOWER([FieldName]) = @FieldName_1", text);
        }
    }
}
