#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionIdentityRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionIdentityRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithDistinctTablesAndARootTable()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"),
                TableSchemaFactory.CreateTable("Second", "Id"));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithATableThatIsDefinedTwice()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"),
                TableSchemaFactory.CreateTable("First", "Id"));
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Duplicate table identity", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "'dbo.First'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithoutARootTable()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.RootTable = null;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "The root table definition is missing", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithARootTableThatIsNotDefined()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.RootTable = new TableInfo("Other", "dbo", "TestDatabase", "TestProvider");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "The root table definition is missing", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithDistinctExternalDependencies()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.ExternalDependencies = new List<TableInfo>
            {
                new TableInfo("Outside1", "dbo", "TestDatabase", "TestProvider"),
                new TableInfo("Outside2", "dbo", "TestDatabase", "TestProvider")
            };
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithAnExternalDependencyThatIsRepeated()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.ExternalDependencies = new List<TableInfo>
            {
                new TableInfo("Outside", "dbo", "TestDatabase", "TestProvider"),
                new TableInfo("Outside", "dbo", "TestDatabase", "TestProvider")
            };
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Invalid external dependency identities", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithAnExternalDependencyThatIsAlsoATable()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.ExternalDependencies = new List<TableInfo> { definition.RootTable };
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Invalid external dependency identities", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionIdentityRulesWithoutExternalDependencies()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"));
            definition.ExternalDependencies = null;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionIdentityRules.Check(definition, Index(definition), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Indexes the tables of the definition by their identity, keeping the first one of a repeated identity.
        /// </summary>
        /// <param name="definition">The definition that has the tables.</param>
        /// <returns>The tables, by their identity.</returns>
        private static IDictionary<TableInfo, TableSchema> Index(SchemaDefinitionInfo definition)
        {
            var index = new Dictionary<TableInfo, TableSchema>();
            foreach (var table in definition.Tables)
            {
                if (!index.ContainsKey(table.Table))
                {
                    index.Add(table.Table, table);
                }
            }
            return index;
        }

        #endregion
    }
}
