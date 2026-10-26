#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Sqlite.Microsoft.IntegrationTests
{
    [TestClass]
    public class SaveDefinitionAsYamlTest : SaveDefinitionTestBase
    {
        #region Methods

        [TestMethod]
        [DataRow(RelatedTableBehavior.TableOnly, "Invoice")]
        [DataRow(RelatedTableBehavior.Parents, "Customer,Invoice")]
        [DataRow(RelatedTableBehavior.Children, "Invoice,InvoiceLine")]
        [DataRow(RelatedTableBehavior.ParentsAndChildren, "Customer,Invoice,InvoiceLine")]
        public void TestSaveDefinitionAsYamlIncludesRequestedRelatedTables(RelatedTableBehavior behavior,
            string expected)
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename, behavior);
            var content = File.ReadAllText(Filename);
            var actual = string.Join(",", Regex.Matches(content, "(?m)^    table:\r?\n(?:      [^\r\n]*\r?\n)*?      name: \"(?<name>[^\"]+)\"\\r?$")
                .Cast<Match>().Select(match => match.Groups["name"].Value));

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlWritesFormatIdentity()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "format: \"RepoDB.Schema\"");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlReadsNumericPrecision()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "precision: 18");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlReadsNumericScale()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "scale: 4");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlReadsUnicodeMetadata()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "unicode: true");
        }

        [TestMethod]
        public void TestSaveDefinitionAsYamlReadsCollation()
        {
            // Setup
            var connection = Connection;

            // Act
            connection.SaveDefinitionAsYaml("Invoice", Filename);
            var actual = File.ReadAllText(Filename);

            // Assert
            StringAssert.Contains(actual, "collation: \"NOCASE\"");
        }

        #endregion
    }
}
