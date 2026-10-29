#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests
{
    /// <summary>
    /// Contains the tests of the <c>SchemaDefinitionCaseRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionCaseRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesWithNamesThatDifferByMoreThanCase()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Name") { Columns = new[] { "Name" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesWithColumnsThatDifferOnlyByCase()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Name", "name");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "column names (Name, name) differ only by case", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesWithConstraintsThatDifferOnlyByCase()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("pk_child") { Columns = new[] { "Code" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "constraint names (PK_Child, pk_child) differ only by case", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesWithIndexesThatDifferOnlyByCase()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo("ix_child") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "index names (IX_Child, ix_child) differ only by case", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesLeavesTheExactDuplicatesToTheNameRules()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesWithThreeVariantsOfTheSameName()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Name", "NAME", "name");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "(Name, NAME, name)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCaseRulesIgnoresTheEmptyNames()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo(null) { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo(" ") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCaseRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        #endregion
    }
}
