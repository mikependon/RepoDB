#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionExternalFlagRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionExternalFlagRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesWithFlagsThatAgree()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: false, externalFlag: true);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesWithAnInternalReferenceFlaggedAsExternal()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: true, externalFlag: true);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "A foreign-key external flag disagrees with the included tables", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "'FK_Internal' should be internal", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesWithAnExternalReferenceFlaggedAsInternal()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: false, externalFlag: false);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "'FK_External' should be external", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesWithAMissingFlag()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: false, externalFlag: true);
            definition.Definition["tables"][0]["foreignKeys"][0]["references"].AsObject().Remove("external");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesWithAFlagThatIsNotABoolean()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: false, externalFlag: true);
            definition.Definition["tables"][0]["foreignKeys"][1]["references"]["external"] = "yes";
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesIsSkippedForTheLaterVersions()
        {
            // Setup
            var definition = CreateDefinition(version: 2, internalFlag: true, externalFlag: false);
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesIsSkippedWithoutTheDocument()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: true, externalFlag: false);
            definition.Definition = null;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionExternalFlagRulesIsSkippedWhenTheDocumentDoesNotMatchTheTables()
        {
            // Setup
            var definition = CreateDefinition(version: 1, internalFlag: true, externalFlag: false);
            definition.Definition["tables"].AsArray().Add(new JsonObject());
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionExternalFlagRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a definition of two tables, where the first one has a foreign key to the second one (internal) and a foreign key to a table that is not defined (external),
        /// together with the document that has the flags of the two foreign keys.
        /// </summary>
        /// <param name="version">The version of the format of the definition.</param>
        /// <param name="internalFlag">The external flag that the document has for the reference to the table of the definition.</param>
        /// <param name="externalFlag">The external flag that the document has for the reference to the table that is not defined.</param>
        /// <returns>The definition.</returns>
        private static SchemaDefinitionInfo CreateDefinition(int version,
            bool internalFlag,
            bool externalFlag)
        {
            var second = TableSchemaFactory.CreateTable("Second", "Id");
            var first = TableSchemaFactory.CreateTable("First", "Id", "SecondId", "OutsideId");
            TableSchemaFactory.AddForeignKey(first, "FK_Internal", second.Table, new[] { "SecondId" }, new[] { "Id" });
            TableSchemaFactory.AddForeignKey(first, "FK_External", new TableInfo("Outside", "dbo", "TestDatabase", "TestProvider"), new[] { "OutsideId" }, new[] { "Id" });
            var definition = TableSchemaFactory.CreateDefinition(first, second);
            definition.FormatVersion = version;
            definition.Definition = new JsonObject
            {
                ["tables"] = new JsonArray
                {
                    new JsonObject
                    {
                        ["foreignKeys"] = new JsonArray
                        {
                            new JsonObject { ["references"] = new JsonObject { ["external"] = internalFlag } },
                            new JsonObject { ["references"] = new JsonObject { ["external"] = externalFlag } }
                        }
                    },
                    new JsonObject { ["foreignKeys"] = new JsonArray() }
                }
            };
            return definition;
        }

        #endregion
    }
}
