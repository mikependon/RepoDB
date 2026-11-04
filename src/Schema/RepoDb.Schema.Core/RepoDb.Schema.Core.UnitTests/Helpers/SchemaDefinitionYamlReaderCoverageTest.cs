using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionYamlReaderCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForEmptyDocument()
        {
            // Setup
            string content = "";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForSequenceRoot()
        {
            // Setup
            string content = "- null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForInconsistentIndentation()
        {
            // Setup
            string content = "a: null\nb: null\n a: null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForMissingNestedCollection()
        {
            // Setup
            string content = "a:\nb: null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForDuplicateProperty()
        {
            // Setup
            string content = "a: []\na: []";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForInvalidPropertyName()
        {
            // Setup
            string content = "bad-key: null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForMixedSequenceAndMapping()
        {
            // Setup
            string content = "a:\n  - null\n  b: null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        /// <summary>
        /// Rejects invalid roots, indentation, keys and sequence items.
        /// </summary>
        [TestMethod]
        public void ParseRejectsInvalidYamlStructureForInvalidSequenceItem()
        {
            // Setup
            string content = "a:\n  -- null";

            // Act
            Action action = () => SchemaDefinitionYamlReader.Parse(content);

            // Assert
            Assert.ThrowsExactly<InvalidDataException>(action);
        }

        #endregion
    }
}
