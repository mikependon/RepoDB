using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionXmlReaderCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForWrongRootElement()
        {
            // Setup
            string content = "\u003cwrong type=\"object\" /\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForNullRoot()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"null\" /\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForScalarTextInCollection()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003etext\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForNestedElementInScalar()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"string\"\u003e\u003cy /\u003e\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForNonemptyNullValue()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"null\"\u003etext\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForInvalidBooleanLiteral()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"boolean\"\u003eTrue\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForFractionalInteger()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"integer\"\u003e1.5\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForObjectInInteger()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"integer\"\u003e{}\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForUnknownValueType()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"unknown\" /\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForDuplicateProperty()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"null\" /\u003e\u003cx type=\"null\" /\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForInvalidArrayElement()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx type=\"array\"\u003e\u003cwrong type=\"null\" /\u003e\u003c/x\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects malformed typed XML structures.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidTypedXmlForNamespacedProperty()
        {
            // Setup
            string content = "\u003crepoDbSchema type=\"object\"\u003e\u003cx xmlns=\"urn:test\" type=\"null\" /\u003e\u003c/repoDbSchema\u003e";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Bounds recursive XML collection parsing.
        /// </summary>
        [TestMethod]
        public void ParseRejectsExcessiveNesting()
        {
            // Setup
            var content = "<repoDbSchema type=\"object\">" + string.Concat(Enumerable.Repeat("<x type=\"object\">",
                65)) + string.Concat(Enumerable.Repeat("</x>",
                65)) + "</repoDbSchema>";

            // Act
            Action action = () => SchemaDefinitionXmlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        #endregion
    }
}
