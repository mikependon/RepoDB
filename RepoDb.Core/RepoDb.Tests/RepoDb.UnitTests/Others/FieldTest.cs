using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class FieldTest
    {
        [TestMethod]
        public void TestFieldAndStringEquality()
        {
            // Prepare
            var field = new Field("FieldName");

            // Act
            var equal = field.Equals("FieldName");

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldNameAndStringEquality()
        {
            // Prepare
            var field = new Field("FieldName");

            // Act
            var equal = Equals("FieldName", field.Name);

            // Assert
            Assert.IsTrue(equal);
        }

        [TestMethod]
        public void TestFieldFromMethodParsing()
        {
            // Prepare
            var fields = new[] { "Field1", "Field2", "Field3" };

            // Act
            var parsed = Field.From(fields);

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field.Name)));
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsNull()
        {
            // Prepare
            var fields = (string)null;

            // Act/Assert
            Field.From(fields).AsList();
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsEmpty()
        {
            // Prepare
            var fields = new[] { "" };

            // Act/Assert
            Field.From(fields).AsList();
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsWhitespace()
        {
            // Prepare
            var fields = new[] { " " };

            // Act/Assert
            Field.From(fields).AsList();
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterHasNull()
        {
            // Prepare
            var fields = new[] { "Field1", null, "Field3" };

            // Act/Assert
            Field.From(fields).AsList();
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldFromIfAnyOfTheParameterIsEmpty()
        {
            // Prepare
            var fields = new[] { "Field1", "", "Field3" };

            // Act/Assert
            Field.From(fields).AsList();
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterHasWhitespace()
        {
            // Prepare
            var fields = new[] { "Field1", " ", "Field3" };

            // Act/Assert
            Field.From(fields).AsList();
        }
    }
}
