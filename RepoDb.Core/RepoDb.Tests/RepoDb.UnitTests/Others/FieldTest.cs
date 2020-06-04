using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Extensions;
using System;
using System.Linq;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public partial class FieldTest
    {
        #region SubClasses

        private class FieldTestClass
        {
            public int Field1 { get; set; }
            public string Field2 { get; set; }
            public DateTime Field3 { get; set; }
        }

        #endregion

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
        public void TestFieldFromMethod()
        {
            // Prepare
            var fields = new[] { "Field1", "Field2", "Field3" };

            // Act
            var parsed = Field.From(fields);

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field.Name)));
        }

        [TestMethod]
        public void TestFieldParseMethodForObject()
        {
            // Prepare
            var obj = new FieldTestClass();
            var fields = obj
                .GetType()
                .GetProperties()
                .AsFields()
                .AsList();

            // Act
            var parsed = Field.Parse(obj);

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field)));
        }

        [TestMethod]
        public void TestFieldParseMethodForEntity()
        {
            // Prepare
            var fields = PropertyCache
                .Get<FieldTestClass>()
                .AsFields()
                .AsList();

            // Act
            var parsed = Field.Parse<FieldTestClass>();

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field)));
        }

        [TestMethod]
        public void TestFieldParseMethodForDynamic()
        {
            // Prepare
            var obj = new { Field1 = 1, Field2 = "Field2", Field3 = DateTime.UtcNow };
            var fields = obj
                .GetType()
                .GetProperties()
                .AsFields()
                .AsList();

            // Act
            var parsed = Field.Parse(obj);

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field)));
        }

        [TestMethod]
        public void TestFieldParseMethodForExpression()
        {
            // Prepare
            var field = new Field("Field1", typeof(int));

            // Act
            var parsed = Field.Parse<FieldTestClass>(e => e.Field1)?.FirstOrDefault();

            // Assert
            Assert.AreEqual(field, parsed);
        }

        [TestMethod]
        public void TestFieldParseMethodForExpressionMultiple()
        {
            // Prepare
            var fields = new[]
            {
                new Field("Field1", typeof(int)),
                new Field("Field2", typeof(string)),
                new Field("Field3", typeof(DateTime))
            };

            // Act
            var parsed = Field.Parse<FieldTestClass>(e => new
            {
                e.Field1,
                e.Field2,
                e.Field3
            });

            // Assert
            Assert.AreEqual(3, parsed.Count());
            Assert.IsTrue(parsed.All(field => fields.Contains(field)));
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
