using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Extensions.QueryFields;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Extensions.QueryFields
{
    [TestClass]
    public class LenQueryFieldTest
    {
        [TestMethod]
        public void TestLenQueryFieldConstructor()
        {
            // Prepare
            var functionalQueryField = new LenQueryField("FieldName", Operation.NotEqual, 5);

            // Assert
            Assert.AreEqual("FieldName", functionalQueryField.Field.Name);
            Assert.AreEqual(Operation.NotEqual, functionalQueryField.Operation);
            Assert.AreEqual(5, functionalQueryField.Parameter.Value);
            Assert.AreEqual("LEN({0})", functionalQueryField.Format);
        }

        [TestMethod]
        public void TestLenQueryFieldGetString()
        {
            // Prepare
            var functionalQueryField = new LenQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEN([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLenQueryFieldGetStringWithFirstIndex()
        {
            // Prepare
            var functionalQueryField = new LenQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(0, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEN([FieldName]) = @FieldName", text);
        }

        [TestMethod]
        public void TestLenQueryFieldGetStringWithIndex()
        {
            // Prepare
            var functionalQueryField = new LenQueryField("FieldName", Operation.Equal, 5);

            // Act
            var text = functionalQueryField.GetString(1, new CustomDbSetting());

            // Assert
            Assert.AreEqual("LEN([FieldName]) = @FieldName_1", text);
        }
    }
}
