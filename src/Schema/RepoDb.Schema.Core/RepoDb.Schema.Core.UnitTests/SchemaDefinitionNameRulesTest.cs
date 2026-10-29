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
    /// Contains the tests of the <c>SchemaDefinitionNameRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionNameRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionNameRulesWithDistinctNames()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Child") { Columns = new[] { "Id" } });
            table.CheckConstraints.Add(new CheckConstraintInfo("CK_Child") { Expression = "[Id] > 0" });
            TableSchemaFactory.AddForeignKey(table, "FK_Child", new TableInfo("Parent", "dbo", "TestDatabase", "TestProvider"), new[] { "ParentId" }, new[] { "Id" });
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "ParentId" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesWithTheSameNameForTwoUniqueConstraints()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Code");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Same") { Columns = new[] { "Id" } });
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Same") { Columns = new[] { "Code" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "constraint name 'UQ_Same' is used more than once", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesWithTheSameNameForDifferentKindsOfConstraints()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(table, "SameName", "Id");
            table.CheckConstraints.Add(new CheckConstraintInfo("SameName") { Expression = "[Id] > 0" });
            TableSchemaFactory.AddForeignKey(table, "SameName", new TableInfo("Parent", "dbo", "TestDatabase", "TestProvider"), new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "'SameName'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesWithTheSameNameForTwoIndexes()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Code");
            table.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo("IX_Same") { Columns = new[] { "Code" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "index name 'IX_Same' is used more than once", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesWithAnIndexNamedLikeTheKeyConstraints()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Code");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Child") { Columns = new[] { "Code" } });
            table.Indexes.Add(new IndexInfo("PK_Child") { Columns = new[] { "Code" } });
            table.Indexes.Add(new IndexInfo("UQ_Child") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(2, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "'PK_Child'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[1], "'UQ_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesIsCaseSensitive()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("ix_child") { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionNameRulesIgnoresTheEmptyNames()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo(null) { Columns = new[] { "Id" } });
            table.Indexes.Add(new IndexInfo(null) { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionNameRules.Check(table, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        #endregion
    }
}
