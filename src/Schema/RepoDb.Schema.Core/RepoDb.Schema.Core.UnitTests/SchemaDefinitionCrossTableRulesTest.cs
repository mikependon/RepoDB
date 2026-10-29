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
    /// Contains the tests of the <c>SchemaDefinitionCrossTableRules</c> class.
    /// </summary>
    [TestClass]
    public class SchemaDefinitionCrossTableRulesTest
    {
        #region Public Methods

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithDistinctNames()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_First");
            var second = CreateTable("Second", "dbo", "PK_Second");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTheSameConstraintNameInTwoTables()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_Shared");
            var second = CreateTable("Second", "dbo", "PK_Shared");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "constraint name 'PK_Shared' is used by more than one table (dbo.First, dbo.Second)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTheSameNameForDifferentKindsOfConstraints()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_First");
            var second = CreateTable("Second", "dbo", "PK_Second");
            first.CheckConstraints.Add(new CheckConstraintInfo("Shared") { Expression = "[Id] > 0" });
            TableSchemaFactory.AddForeignKey(second, "Shared", first.Table, new[] { "Id" }, new[] { "Id" });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "'Shared'", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesIgnoresTheCaseOfTheConstraintNames()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_Shared");
            var second = CreateTable("Second", "dbo", "pk_shared");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTheSameConstraintNameInDifferentSchemas()
        {
            // Setup
            var first = CreateTable("First", "sales", "PK_Shared");
            var second = CreateTable("Second", "hr", "PK_Shared");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTheSameConstraintNameInTwoTablesWithoutASchema()
        {
            // Setup
            var first = CreateTable("First", null, "PK_Shared");
            var second = CreateTable("Second", null, "PK_Shared");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "(First, Second)", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesCountsAConstraintNameOncePerTable()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_Shared");
            first.UniqueConstraints.Add(new UniqueConstraintInfo("PK_Shared") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first }, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesDoesNotCheckTheIndexNames()
        {
            // Setup
            var first = CreateTable("First", "dbo", "PK_First");
            var second = CreateTable("Second", "dbo", "PK_Second");
            first.Indexes.Add(new IndexInfo("IX_Shared") { Columns = new[] { "Id" } });
            second.Indexes.Add(new IndexInfo("IX_Shared") { Columns = new[] { "Id" } });
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTablesThatDifferOnlyByCase()
        {
            // Setup
            var first = CreateTable("Orders", "dbo", "PK_Orders");
            var second = CreateTable("ORDERS", "dbo", "PK_ORDERS_2");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Errors.Count);
            Assert.AreEqual(1, findings.Warnings.Count);
            StringAssert.Contains(findings.Warnings[0], "tables (dbo.Orders, dbo.ORDERS) differ only by case", StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesWithTablesThatDifferOnlyByCaseInDifferentSchemas()
        {
            // Setup
            var first = CreateTable("Orders", "sales", "PK_Orders");
            var second = CreateTable("ORDERS", "hr", "PK_ORDERS_2");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(0, findings.Warnings.Count);
        }

        [TestMethod]
        public void TestSchemaDefinitionCrossTableRulesIgnoresTheCaseOfTheSchemaName()
        {
            // Setup
            var first = CreateTable("First", "DBO", "PK_Shared");
            var second = CreateTable("Second", "dbo", "PK_Shared");
            var findings = new DefinitionFindings();

            // Act
            SchemaDefinitionCrossTableRules.Check(new[] { first, second }, findings);

            // Assert
            Assert.AreEqual(1, findings.Warnings.Count);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a table that has a column Id and a primary key.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The schema of the table, or <c>null</c>.</param>
        /// <param name="primaryKeyName">The name of the primary key.</param>
        /// <returns>The table.</returns>
        private static TableSchema CreateTable(string name,
            string schema,
            string primaryKeyName)
        {
            var table = new TableSchema(new TableInfo(name, schema, "TestDatabase", "TestProvider"));
            table.Columns.Add(TableSchemaFactory.CreateColumn("Id", typeof(int), 1));
            TableSchemaFactory.SetPrimaryKey(table, primaryKeyName, "Id");
            return table;
        }

        #endregion
    }
}
