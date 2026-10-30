#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionStructureValidator</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionStructureValidatorTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorWithASoundDefinition()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentId" }, new[] { "Id" });
            var definition = TableSchemaFactory.CreateDefinition(child, parent);

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorCollectsTheProblemsOfTheDefinitionAndOfEveryTable()
        {
            // Setup
            var first = TableSchemaFactory.CreateTable("First", "Id");
            TableSchemaFactory.SetPrimaryKey(first, "PK_First", "Missing");
            var second = TableSchemaFactory.CreateTable("Second", "Id");
            second.Indexes.Add(new IndexInfo("IX_Second") { Columns = new[] { "AlsoMissing" } });
            var definition = TableSchemaFactory.CreateDefinition(first, second);
            definition.RootTable = new TableInfo("Other", "dbo", "TestDatabase", "TestProvider");

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(3, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "The root table definition is missing", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[1], "Table 'dbo.First'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[2], "Table 'dbo.Second'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorDoesNotFailOnATableThatIsDefinedTwice()
        {
            // Setup
            var definition = TableSchemaFactory.CreateDefinition(TableSchemaFactory.CreateTable("First", "Id"),
                TableSchemaFactory.CreateTable("First", "Id"));

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Duplicate table identity", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorUsesTheExternalDependenciesToResolveTheForeignKeys()
        {
            // Setup
            var outside = new TableInfo("Outside", "dbo", "TestDatabase", "TestProvider");
            var child = TableSchemaFactory.CreateTable("Child", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", outside, new[] { "ParentId" }, new[] { "Id" });
            var definition = TableSchemaFactory.CreateDefinition(child);
            definition.ExternalDependencies = new List<TableInfo> { outside };

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorChecksTheExternalFlagsOfAVersionOneDefinition()
        {
            // Setup
            var second = TableSchemaFactory.CreateTable("Second", "Id");
            var first = TableSchemaFactory.CreateTable("First", "SecondId");
            TableSchemaFactory.AddForeignKey(first, "FK_First", second.Table, new[] { "SecondId" }, new[] { "Id" });
            var definition = TableSchemaFactory.CreateDefinition(first, second);
            definition.FormatVersion = 1;
            definition.Definition = new JsonObject
            {
                ["tables"] = new JsonArray
                {
                    new JsonObject { ["foreignKeys"] = new JsonArray { new JsonObject { ["references"] = new JsonObject { ["external"] = true } } } },
                    new JsonObject { ["foreignKeys"] = new JsonArray() }
                }
            };

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "external flag disagrees", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionStructureValidatorDoesNotRunTheStrictChecks()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Empty");
            var definition = TableSchemaFactory.CreateDefinition(table);

            // Act
            var findings = SchemaDefinitionStructureValidator.Validate(definition);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        #endregion
    }
}
