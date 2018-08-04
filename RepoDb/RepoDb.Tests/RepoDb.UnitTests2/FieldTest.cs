using NUnit.Framework;
using System;
using System.Linq;

namespace RepoDb.UnitTests
{
    [TestFixture]
    public class FieldTest
    {
        // From

        [Test]
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

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterIsNull()
        {
            // Prepare
            var fields = (string)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterIsEmpty()
        {
            // Prepare
            var fields = new[] { "" };

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterIsWhitespace()
        {
            // Prepare
            var fields = new[] { " " };

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterHasNull()
        {
            // Prepare
            var fields = new[] { "Field1", null, "Field3" };

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterHasEmpty()
        {
            // Prepare
            var fields = new[] { "Field1", "", "Field3" };

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        [Test]
        public void ThrowExceptionIfFromMethodFieldsParameterHasWhitespace()
        {
            // Prepare
            var fields = new[] { "Field1", " ", "Field3" };

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.From(fields));
        }

        // Parse

        [Test]
        public void TestParseMethod()
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

        [Test]
        public void ThrowExceptionIfParseMethodObjParameterIsNull()
        {
            // Prepare
            var obj = (object)null;

            // Act/Assert
            Assert.Throws<NullReferenceException>(() => Field.Parse(obj));
        }

        [Test]
        public void ThrowExceptionIfParseMethodObjParameterIsNotDynamic()
        {
            // Prepare
            var obj = "NotADynamic";

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Field.Parse(obj));
        }

        [Test]
        public void ThrowExceptionIfParseMethodObjParameterHasNoProperty()
        {
            // Prepare
            var obj = new { };

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() => Field.Parse(obj));
        }
    }
}
