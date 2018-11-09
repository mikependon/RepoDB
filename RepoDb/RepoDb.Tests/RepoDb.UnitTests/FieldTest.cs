using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Exceptions;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestClass]
    public class FieldTest
    {
        // From

        [TestMethod]
        public void TestFromMethod()
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
        public void ThrowExceptionIfFromMethodFieldsParameterIsNull()
        {
            // Prepare
            var fields = (string)null;

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfFromMethodFieldsParameterIsEmpty()
        {
            // Prepare
            var fields = new[] { "" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfFromMethodFieldsParameterIsWhitespace()
        {
            // Prepare
            var fields = new[] { " " };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfFromMethodFieldsParameterHasNull()
        {
            // Prepare
            var fields = new[] { "Field1", null, "Field3" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfFromMethodFieldsParameterHasEmpty()
        {
            // Prepare
            var fields = new[] { "Field1", "", "Field3" };

            // Act/Assert
            Field.From(fields);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfFromMethodFieldsParameterHasWhitespace()
        {
            // Prepare
            var fields = new[] { "Field1", " ", "Field3" };

            // Act/Assert
            Field.From(fields);
        }

        // Parse

        public class FieldTestClass
        {
            public int Id { get; set; }
        }

        [TestMethod]
        public void TestParseExpressionMethod()
        {
            // Act
            var parsed = Field.Parse<FieldTestClass>(p => p.Id);

            // Assert
            Assert.AreEqual("Id", parsed.Name);
        }

        [TestMethod]
        public void TestParseDynamicMethod()
        {
            // Prepare
            var obj = new { Field1 = "Field1", Field2 = "Field2" };

            // Act
            var parsed = Field.Parse(obj);

            // Assert
            Assert.AreEqual(2, parsed.Count());
            var fields = parsed.ToList();
            Assert.AreEqual("Field1", fields[0].Name);
            Assert.AreEqual("Field2", fields[1].Name);
        }

        [TestMethod, ExpectedException(typeof(InvalidQueryExpressionException))]
        public void ThrowExceptionIfParseExpressionMethodHasNoPropertyName()
        {
            // Act/Assert
            Field.Parse<FieldTestClass>(p => 1);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionIfParseDynamicMethodObjParameterIsNull()
        {
            // Prepare
            var obj = (object)null;

            // Act/Assert
            Field.Parse(obj);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfParseDynamicMethodObjParameterIsNotDynamic()
        {
            // Prepare
            var obj = "NotADynamic";

            // Act/Assert
            Field.Parse(obj);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void ThrowExceptionIfParseDynamicMethodObjParameterHasNoProperty()
        {
            // Prepare
            var obj = new { };

            // Act/Assert
            Field.Parse(obj);
        }
    }
}
