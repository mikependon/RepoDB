#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionFormatRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionFormatRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionFormatRulesWithTheFormatOfASchemaDefinition()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionFormatRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        [DataRow("RepoDB.Schema")]
        [DataRow("RepoDb.Schema")]
        [DataRow("repodb.schema.definition")]
        [DataRow("REPODB.SCHEMA.DEFINITION")]
        [DataRow("RepoDb.Schema.Definition ")]
        [DataRow(" RepoDb.Schema.Definition")]
        [DataRow("Other")]
        [DataRow("")]
        public void TestSchemaDefinitionFormatRulesWithAnotherFormat(string format)
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            definition.Format = format;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionFormatRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "The file is not a RepoDB schema definition", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "must be 'RepoDb.Schema.Definition'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], $"but it is '{format}'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionFormatRulesWithoutAFormat()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("Child", "Id"));
            definition.Format = null;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionFormatRules.Check(definition, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "but it is missing", StringComparison.Ordinal);
        }

        #endregion
    }
}
