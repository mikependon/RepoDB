using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Fields
{
    [TestClass]
    public partial class FieldTest
    {
        [TestMethod]
        public void TestFieldQuotes()
        {
            // Prepare
            var objA = new Field("FieldName");

            // Act
            var equal = Equals("[FieldName]", objA.Name);

            // Assert
            Assert.IsTrue(equal);
        }

        // From

        [TestMethod]
        public void TestFieldFromMethodParsing()
        {
            // Prepare
            var fields = new[] { "Field1", "Field2", "Field3" };

            // Act
            var parsed = Field.From(fields);

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Select(f => f.AsQuoted()).Contains(field.Name)));
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsNull()
        {
            // Prepare
            var fields = (string)null;

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsEmpty()
        {
            // Prepare
            var fields = new[] { "" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterIsWhitespace()
        {
            // Prepare
            var fields = new[] { " " };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterHasNull()
        {
            // Prepare
            var fields = new[] { "Field1", null, "Field3" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterHasEmpty()
        {
            // Prepare
            var fields = new[] { "Field1", "", "Field3" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnFieldIfTheFromMethodFieldsParameterHasWhitespace()
        {
            // Prepare
            var fields = new[] { "Field1", " ", "Field3" };

            // Act/Assert
            Field.From(fields);
        }
    }
}
