using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;
using System;

namespace RepoDb.Schema.MariaDb.UnitTests.Composers
{
    /// <summary>
    /// Covers supported values and boundary cases.
    /// </summary>
    [TestClass]
    public class MariaDbSchemaComposerCoverageTest
    {
        #region Public Methods

        /// <summary>
        /// Preserves case-sensitive enum and set literals.
        /// </summary>
        [TestMethod]
        public void ComposeTypeNamePreservesNativeEnumMembersForEnumOneTwoInMixedCase()
        {
            // Setup
            string declaration = "enum('One','Two')";
            var composer = new MariaDbSchemaComposer();
            var column = new ColumnInfo
            {
                Field = new DbField("Value",
                    false,
                    false,
                    true,
                    typeof(string),
                    50,
                    null,
                    null,
                    declaration)
            };

            // Act
            var actual = composer.ComposeTypeName(column);

            // Assert
            Assert.AreEqual(declaration,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Preserves case-sensitive enum and set literals.
        /// </summary>
        [TestMethod]
        public void ComposeTypeNamePreservesNativeEnumMembersForSetOneTwoInMixedCase()
        {
            // Setup
            string declaration = "set('One','Two')";
            var composer = new MariaDbSchemaComposer();
            var column = new ColumnInfo
            {
                Field = new DbField("Value",
                    false,
                    false,
                    true,
                    typeof(string),
                    50,
                    null,
                    null,
                    declaration)
            };

            // Act
            var actual = composer.ComposeTypeName(column);

            // Assert
            Assert.AreEqual(declaration,
                actual,
                StringComparer.Ordinal);
        }

        /// <summary>
        /// Escapes apostrophes in column comments.
        /// </summary>
        [TestMethod]
        public void ComposeColumnEscapesCommentApostrophe()
        {
            // Setup
            var composer = new MariaDbSchemaComposer();
            var column = new ColumnInfo
            {
                Field = new DbField("Value",
                    false,
                    false,
                    true,
                    typeof(string),
                    50,
                    null,
                    null,
                    "varchar")
            };
            column.Comment = "owner's column";

            // Act
            var actual = composer.ComposeAddColumn("Items",
                column);

            // Assert
            StringAssert.Contains(actual,
                "owner''s column",
                StringComparison.Ordinal);
        }

        #endregion
    }
}
