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
    /// Contains the tests of the <c>SchemaDefinitionTableRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionTableRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAConsistentTable()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            TableSchemaFactory.SetPrimaryKey(parent, "PK_Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id", "ParentId");
            TableSchemaFactory.SetPrimaryKey(child, "PK_Child", "Id");
            child.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Child") { Columns = new[] { "ParentId" } });
            child.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "ParentId" }, IncludedColumns = new[] { "Id" } });
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(parent, child), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithRepeatedColumnNames()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Id");
            table.Columns[1].Ordinal = 2;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Duplicate column name", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "Table 'dbo.Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        [DataRow(0, 2)]
        [DataRow(1, 1)]
        [DataRow(-1, 2)]
        public void TestSchemaDefinitionTableRulesWithInvalidColumnOrdinals(int firstOrdinal,
            int secondOrdinal)
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id", "Name");
            table.Columns[0].Ordinal = firstOrdinal;
            table.Columns[1].Ordinal = secondOrdinal;
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Invalid or duplicate column ordinal", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAPrimaryKeyOnAColumnThatDoesNotExist()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "A key or index contains invalid column references", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "primary key 'PK_Child'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "(Missing)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAPrimaryKeyWithoutColumns()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "primary key 'PK_Child' has no columns", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAPrimaryKeyThatRepeatsAColumn()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Id", "Id");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "primary key 'PK_Child' repeats the columns (Id)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAUniqueConstraintOnAColumnThatDoesNotExist()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.UniqueConstraints.Add(new UniqueConstraintInfo("UQ_Child") { Columns = new[] { "Missing" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "unique constraint 'UQ_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAnIndexOnAColumnThatDoesNotExist()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Missing" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "index 'IX_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAnIndexThatIncludesAColumnThatDoesNotExist()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" }, IncludedColumns = new[] { "Missing" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "included columns of the index 'IX_Child'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesAllowsAnIndexWithoutIncludedColumns()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "Id" }, IncludedColumns = new string[0] });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAForeignKeyOnAColumnThatDoesNotExist()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "Missing" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(parent, child), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "foreign key 'FK_Child'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "(Missing)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAForeignKeyWhoseColumnCountsDiffer()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "A", "B");
            var child = TableSchemaFactory.CreateTable("Child", "X");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "X" }, new[] { "A", "B" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(parent, child), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "Foreign-key column counts differ", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "has 1 column(s) but references 2", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAForeignKeyReferencingAColumnThatTheParentDoesNotHave()
        {
            // Setup
            var parent = TableSchemaFactory.CreateTable("Parent", "Id");
            var child = TableSchemaFactory.CreateTable("Child", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", parent.Table, new[] { "ParentId" }, new[] { "Missing" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(parent, child), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "referenced columns of the foreign key 'FK_Child'", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "(Missing)", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "the table 'dbo.Parent' does not have", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAForeignKeyToATableThatIsNeitherDefinedNorExternal()
        {
            // Setup
            var child = TableSchemaFactory.CreateTable("Child", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", new TableInfo("Lost", "dbo", "TestDatabase", "TestProvider"), new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(child), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(1, findings.Errors.Count);
            StringAssert.Contains(findings.Errors[0], "A foreign-key target is missing from the definition and external dependencies", StringComparison.Ordinal);
            StringAssert.Contains(findings.Errors[0], "'dbo.Lost'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesWithAForeignKeyToAnExternalTable()
        {
            // Setup
            var outside = new TableInfo("Outside", "dbo", "TestDatabase", "TestProvider");
            var child = TableSchemaFactory.CreateTable("Child", "ParentId");
            TableSchemaFactory.AddForeignKey(child, "FK_Child", outside, new[] { "ParentId" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(child, Index(child), new HashSet<TableInfo> { outside }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionTableRulesReportsEveryProblemOfTheTable()
        {
            // Setup
            var table = TableSchemaFactory.CreateTable("Child", "Id");
            TableSchemaFactory.SetPrimaryKey(table, "PK_Child", "Missing");
            table.Indexes.Add(new IndexInfo("IX_Child") { Columns = new[] { "AlsoMissing" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionTableRules.Check(table, Index(table), new HashSet<TableInfo>(), findings);

            // Assert
            Assert.AreEqual(2, findings.Errors.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Indexes the tables by their identity, as the structure validator does.
        /// </summary>
        /// <param name="tables">The tables of the definition.</param>
        /// <returns>The tables, by their identity.</returns>
        private static IDictionary<TableInfo, TableSchema> Index(params TableSchema[] tables)
        {
            var index = new Dictionary<TableInfo, TableSchema>();
            foreach (var table in tables)
            {
                index.Add(table.Table, table);
            }
            return index;
        }

        #endregion
    }
}
