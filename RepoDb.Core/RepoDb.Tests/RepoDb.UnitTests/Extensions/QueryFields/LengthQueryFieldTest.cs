using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LengthQueryFieldTest
    {
        [TestMethod]
        public void TestLengthQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new LengthQueryField("FieldName", Operation.NotEqual, 5);

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual(5, functionalQueryField.Parameter.Value);
            Assert.AreEqual("LENGTH({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestLengthQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new LengthQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LENGTH([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLengthQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new LengthQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LENGTH([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLengthQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new LengthQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LENGTH([FieldName]) = @FieldName_1", text);
        }
    }
}
