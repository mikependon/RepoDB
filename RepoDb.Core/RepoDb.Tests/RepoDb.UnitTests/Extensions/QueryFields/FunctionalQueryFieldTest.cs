using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class FunctionalQueryFieldTest
    {
        [TestMethod]
        public void TestFunctionalQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new FunctionalQueryField("FieldName", Operation.NotEqual, "Value", "FUNC({0})");

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual("Value", functionalQueryField.Parameter.Value);
            Assert.AreEqual("FUNC({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestFunctionalQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new FunctionalQueryField("FieldName", Operation.Equal, "Value", "FUNC({0})");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("FUNC([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestFunctionalQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new FunctionalQueryField("FieldName", Operation.Equal, "Value", "FUNC({0})");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("FUNC([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestFunctionalQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new FunctionalQueryField("FieldName", Operation.Equal, "Value", "FUNC({0})");

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("FUNC([FieldName]) = @FieldName_1", text);
        }

        [TestMethod]
        public void TestFunctionalQueryFieldGetStringWithMultipleFunctions()
        {
            // Prepare
            var functionalQueryField = new FunctionalQueryField("FieldName", Operation.Equal, "Value", "FUNC1({0}) = @Value AND FUNC2({0})");

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("FUNC1([FieldName]) = @Value AND FUNC2([FieldName]) = @FieldName", text);
        }
    }
}
